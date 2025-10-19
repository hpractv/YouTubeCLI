using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using YouTubeCLI.Models;
using YouTubeCLI.Utilities;

namespace YouTubeCLI.Libraries
{
    public enum PrivacyEnum { Private, Public, Unlisted }

    public class YouTubeLibrary
    {
        private string _user;
        private string _clientSecretsFile;
        private static YouTubeService _service;
        private static UserCredential _credential;
        private FileDataStore _fileStore;

        private const string _broadcastPart = "id,snippet,contentDetails,status,statistics";
        private const string _streamPart = "id,snippet,cdn,contentDetails,status";
        private const string _videoPart = "id,contentDetails,fileDetails,liveStreamingDetails,localizations,player,processingDetails,recordingDetails,snippet,statistics,status,suggestions,topicDetails";
        private const string _searchPart = "id,snippet";

        public YouTubeLibrary() { }

        public YouTubeLibrary(string user, string clientSecretsFile)
        {
            this._user = user;
            this._clientSecretsFile = clientSecretsFile;
            this._fileStore = new FileDataStore(this.GetType().ToString());
        }

        internal async Task ClearCredential()
        {
            try{
                await _fileStore.ClearAsync();
                // Clear static fields to force re-authentication
                _service = null;
                _credential = null;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error clearing credentials: {ex.Message}");
            }
        }

        private async Task<YouTubeService> GetService()
        {
            if (_service == null)
            {
                using (var stream = new FileStream(_clientSecretsFile, FileMode.Open, FileAccess.Read))
                {

                    if (_credential == null)
                    {
                        _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.FromStream(stream).Secrets,
                            // This OAuth 2.0 access scope allows for full read/write access to the
                            // authenticated user's account.
                            new[] { YouTubeService.Scope.Youtube },
                            _user,
                            CancellationToken.None,
                            _fileStore
                        );
                    }
                }

                _service = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = this.GetType().ToString()
                });
            }
            return _service;
        }

        private YouTubeService service
            => _service = _service ?? Task.Run<YouTubeService>(() => GetService()).Result;

        private IEnumerable<LiveStream> _streams;
        private async Task<IEnumerable<LiveStream>> GetLiveStreams()
        {
            var _liveStreamsRequest = (await GetService()).LiveStreams.List(_streamPart);
            _liveStreamsRequest.Mine = true;
            _liveStreamsRequest.MaxResults = 50;

            var _liveStreamResponse = await _liveStreamsRequest.ExecuteAsync();

            return _liveStreamResponse.Items.Cast<LiveStream>();

        }
        private IEnumerable<LiveStream> streams
            => _streams = _streams ?? Task.Run<IEnumerable<LiveStream>>(() => GetLiveStreams()).Result;

        public async Task<IEnumerable<LiveBroadcastInfo>> BuildBroadCast(
            Broadcast broadcast,
            int occurrences,
            string thumbnailDirectory,
            DateOnly? startsOn = null,
            bool testMode = false)
        {
            var _startDate = startsOn ?? DateOnly.FromDateTime(DateTime.Now);

            // Calculate the correct start date based on day of week
            DateOnly _nextBroadcastDay;
            var _startDayOfWeek = (int)_startDate.DayOfWeek;
            var _targetDayOfWeek = broadcast.dayOfWeek;

            if (_startDayOfWeek == _targetDayOfWeek)
            {
                // If the start date is already on the correct day of week, use it
                _nextBroadcastDay = _startDate;
            }
            else
            {
                // Otherwise, find the next occurrence of that day of the week
                var _daysToAdd = (7 + _targetDayOfWeek - _startDayOfWeek) % 7;
                if (_daysToAdd == 0) _daysToAdd = 7; // If it's the same day, go to next week
                _nextBroadcastDay = _startDate.AddDays(_daysToAdd);
            }

            var _startTime = DateTime.Parse($"{_nextBroadcastDay.ToShortDateString()} {broadcast.broadcastStart}");
            var _stream = streams.Single(s => s.Snippet.Title.ToLower() == broadcast.stream.ToLower());
            var _builtBroadcasts = new List<LiveBroadcastInfo>();

            for (int i = 0; i < (testMode ? 1 : occurrences); i++)
            {
                var _title = $"{(testMode ? "Test Mode: " : string.Empty)}{broadcast.name} Sacrament - {_startTime.ToShortDateString()}";
                var _lbInsertRequest = service.LiveBroadcasts.Insert(
                    new LiveBroadcast()
                    {
                        Snippet = new LiveBroadcastSnippet()
                        {
                            Title = _title,
                            ScheduledStartTime = _startTime.ToUniversalTime(),
                            ScheduledEndTime = _startTime.AddMinutes(broadcast.broadcastDurationInMinutes).ToUniversalTime(),
                        },
                        Status = new LiveBroadcastStatus()
                        {
                            PrivacyStatus = broadcast.privacy,
                            // SelfDeclaredMadeForKids = true, going to disable the chat by hand and not use this flag
                        },
                        ContentDetails = new LiveBroadcastContentDetails()
                        {
                            EnableAutoStart = broadcast.autoStart,
                            EnableAutoStop = broadcast.autoStop,
                            EnableDvr = false,
                            EnableEmbed = true, // This throws an error and should default to true
                            RecordFromStart = true,
                        },
                        Kind = "youtube#liveBroadcast"
                    }, _broadcastPart);

                var _broadcast = await _lbInsertRequest.ExecuteAsync();
                var _streamSnippet = SetStream(_broadcast.Id, _stream.Id);
                Task.WaitAll(new[] {
                    SetBroadcastThumbnail(_broadcast.Id, thumbnailDirectory, broadcast.thumbnail),
                        _streamSnippet
                    });
                _builtBroadcasts.Add(new LiveBroadcastInfo
                {
                    broadcast = broadcast.id,
                    youTubeId = _broadcast.Id,
                    title = _title,
                    start = _startTime,
                    autoStart = broadcast.autoStart,
                    autoStop = broadcast.autoStop,
                    privacy = broadcast.privacy
                });
                _startTime = _startTime.AddDays(7);
            }

            return _builtBroadcasts.ToArray();
        }

        public async Task UpdateBroadcast(string broadcastId, bool? autoStart, bool? autoStop, PrivacyEnum? privacy)
        {
            var _broadcastsRequest = service.LiveBroadcasts.List(_broadcastPart);
            _broadcastsRequest.Id = broadcastId;

            var _broadcast = (await _broadcastsRequest.ExecuteAsync())
                .Items.Single(b => b.Id == broadcastId);

            if (autoStart != null)
            {
                _broadcast.ContentDetails.EnableAutoStart = autoStart.Value;
            }
            if (autoStop != null)
            {
                _broadcast.ContentDetails.EnableAutoStop = autoStop.Value;
            }
            if (privacy != null)
            {
                _broadcast.Status.PrivacyStatus = privacy.ToString().ToLower();
            }

            var _updateRequest = service.LiveBroadcasts.Update(_broadcast, _broadcastPart);
            await _updateRequest.ExecuteAsync();
        }

        private async Task SetBroadcastThumbnail(string videoId, string thumbnailDirectory, string thumbnail)
        {
            using (var _client = new HttpClient())
            {
                // Debug: Show original values
                Console.WriteLine($"OS: {OSDetection.GetOSInfo()}");
                Console.WriteLine($"Thumbnail directory: {thumbnailDirectory}");
                Console.WriteLine($"Original thumbnail: {thumbnail}");

                // Use OS detection to properly normalize the thumbnail path
                var normalizedThumbnail = OSDetection.NormalizePath(thumbnail);
                Console.WriteLine($"Normalized thumbnail: {normalizedThumbnail}");

                var fullThumbnailPath = Path.Combine(thumbnailDirectory, normalizedThumbnail);
                Console.WriteLine($"Full thumbnail path: {fullThumbnailPath}");

                // Check if file exists before trying to open it
                if (!File.Exists(fullThumbnailPath))
                {
                    Console.WriteLine($"❌ File not found: {fullThumbnailPath}");
                    throw new FileNotFoundException($"Thumbnail file not found: {fullThumbnailPath}");
                }

                var _thumbnail = new FileStream(fullThumbnailPath, FileMode.Open, FileAccess.Read);
                var _type = Path.GetExtension(thumbnail) switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };
                var _thumbnailRequest = service.Thumbnails.Set(videoId, _thumbnail, _type);
                await _thumbnailRequest.UploadAsync();
            }
        }
        private async Task<LiveBroadcastSnippet> SetStream(string videoId, string streamId)
        {
            var _brInsertRequest = service.LiveBroadcasts.Bind(videoId, _broadcastPart);
            _brInsertRequest.StreamId = streamId;
            var _broadcast = await _brInsertRequest.ExecuteAsync();
            return _broadcast.Snippet;
        }

        public async Task<IEnumerable<LiveBroadcast>> ListBroadcasts(string parts = _broadcastPart)
        {
            var _listRequest = service.LiveBroadcasts.List(_broadcastPart);
            _listRequest.Mine = true;
            var _broadcasts = await _listRequest.ExecuteAsync();

            return _broadcasts.Items;
        }

        public async Task<IEnumerable<LinkDetails>> ListBroadcastUrls(bool upcoming = false)
           => (await ListBroadcasts("id,snippet"))
                .Where(b => !upcoming ||
                    (upcoming && b.Snippet.ScheduledStartTime > DateTime.Now))
                .Select(b => new LinkDetails
                {
                    id = b.Id,
                    title = b.Snippet.Title,
                });

        public async Task<IEnumerable<LinkDetails>> ListUpcomingBroadcastUrls()
            => await ListBroadcastUrls(true);
    }
}