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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using YouTubeCLI.Models;
using YouTubeCLI.Utilities;
using YouTubeCLI.Commands;

[assembly: InternalsVisibleTo("YouTubeCLI.Tests")]

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
            try {
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

        /// <summary>
        /// Calculates the next occurrence of a target day of the week from a given start date.
        /// Supports both 0-based (0=Sunday...6=Saturday) and 1-based (1=Sunday...7=Saturday) day-of-week values.
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="targetDayOfWeek">The target day of the week. Accepts 0-6 (0-based) or 1-7 (1-based)</param>
        /// <returns>The next occurrence of the target day of the week</returns>
        internal DateOnly CalculateNextBroadcastDate(DateOnly startDate, int targetDayOfWeek)
        {
            // Validate input range first
            if (targetDayOfWeek < 0 || targetDayOfWeek > 7)
            {
                throw new ArgumentException($"Invalid dayOfWeek value: {targetDayOfWeek}. Expected 0-6 (0-based) or 1-7 (1-based).", nameof(targetDayOfWeek));
            }

            // Convert from 1-based (1=Sunday, 2=Monday, ..., 7=Saturday) to 0-based (0=Sunday, 1=Monday, ..., 6=Saturday)
            // If targetDayOfWeek is already 0-based (0-6), value 0 passes through as Sunday
            // If it's 1-based (1-7), we subtract 1 to convert to 0-based
            var normalizedTargetDay = targetDayOfWeek >= 1 && targetDayOfWeek <= 7 ? targetDayOfWeek - 1 : targetDayOfWeek;

            var startDayOfWeek = (int)startDate.DayOfWeek;

            if (startDayOfWeek == normalizedTargetDay)
            {
                // If the start date is already on the correct day of week, use it
                return startDate;
            }
            
            // Otherwise, find the next occurrence of that day of the week
            var daysToAdd = (7 + normalizedTargetDay - startDayOfWeek) % 7;
            return startDate.AddDays(daysToAdd);
        }

        public async Task<IEnumerable<LiveBroadcastInfo>> BuildBroadCast(
            Broadcast broadcast,
            int occurrences,
            string thumbnailDirectory,
            DateOnly? startsOn = null,
            bool testMode = false)
        {
            var _startDate = startsOn ?? DateOnly.FromDateTime(DateTime.Now);

            // Validate that start date is not before today
            if (startsOn.HasValue && startsOn.Value < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException($"Start date '{startsOn.Value:MM/dd/yyyy}' cannot be before today's date '{DateOnly.FromDateTime(DateTime.Now):MM/dd/yyyy}'.", nameof(startsOn));
            }

            // Calculate the correct start date based on day of week
            var _nextBroadcastDay = CalculateNextBroadcastDate(_startDate, broadcast.dayOfWeek);

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
                            PrivacyStatus = broadcast.privacy.ToLower(),
                            // Using SelfDeclaredMadeForKids to control chat: when true, chat is disabled
                            // When false (default), chat is enabled
                            SelfDeclaredMadeForKids = !broadcast.chatEnabled,
                        },
                        ContentDetails = new LiveBroadcastContentDetails()
                        {
                            EnableAutoStart = broadcast.autoStart,
                            EnableAutoStop = broadcast.autoStop,
                            EnableDvr = false,
                            EnableEmbed = true,
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

        public async Task UpdateBroadcast(string broadcastId, bool? autoStart, bool? autoStop, PrivacyEnum? privacy, bool? chatEnabled = null)
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
            if (chatEnabled != null)
            {
                // Using SelfDeclaredMadeForKids to control chat: when true, chat is disabled
                // When false, chat is enabled
                if (_broadcast.Status == null)
                {
                    _broadcast.Status = new LiveBroadcastStatus();
                }
                _broadcast.Status.SelfDeclaredMadeForKids = !chatEnabled.Value;
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

        public async Task<IEnumerable<LiveBroadcast>> ListBroadcasts(string parts = _broadcastPart, string broadcastStatus = "all")
        {
            var _listRequest = service.LiveBroadcasts.List(_broadcastPart);
            
            // Note: According to YouTube API docs, Mine and BroadcastStatus parameters are incompatible
            // When BroadcastStatus is not "all", we use BroadcastStatus parameter
            // When BroadcastStatus is "all", we use Mine parameter instead
            
            switch (broadcastStatus.ToLowerInvariant())
            {
                case "all":
                    // Use Mine instead of BroadcastStatus for "all"
                    _listRequest.Mine = true;
                    break;
                case "upcoming":
                    _listRequest.BroadcastStatus = Google.Apis.YouTube.v3.LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Upcoming;
                    break;
                case "active":
                    _listRequest.BroadcastStatus = Google.Apis.YouTube.v3.LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;
                    break;
                case "completed":
                    _listRequest.BroadcastStatus = Google.Apis.YouTube.v3.LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Completed;
                    break;
            }

            var _broadcasts = await _listRequest.ExecuteAsync();

            return _broadcasts.Items;
        }

        public async Task<IEnumerable<LinkDetails>> ListBroadcastUrls(BroadcastFilter filter = BroadcastFilter.All, int limit = 100)
        {
            var broadcastStatus = filter.ToApiString();
            var broadcasts = await ListBroadcasts("id,snippet,status", broadcastStatus);

            var linkDetails = broadcasts
                .OrderByDescending(b => b.Snippet?.ScheduledStartTime ?? DateTime.MinValue)
                .ThenBy(b => b.Snippet?.ScheduledStartTime == null ? 1 : 0) // Put nulls last
                .Take(limit)
                .Select(b => new LinkDetails
                {
                    id = b.Id,
                    title = b.Snippet?.Title ?? "Unknown",
                    privacyStatus = b.Status?.PrivacyStatus ?? "unknown",
                });

            return linkDetails;
        }

        public async Task<IEnumerable<LinkDetails>> ListBroadcastUrls(IEnumerable<BroadcastFilter> filters, int limit = 100)
        {
            if (filters == null || !filters.Any())
            {
                filters = new[] { BroadcastFilter.All };
            }

            // If All is specified, just use that
            if (filters.Contains(BroadcastFilter.All))
            {
                return await ListBroadcastUrls(BroadcastFilter.All, limit);
            }

            // Fetch broadcasts for each filter and combine
            var allBroadcasts = new List<LinkDetails>();
            foreach (var filter in filters.Distinct())
            {
                var broadcasts = await ListBroadcastUrls(filter, limit);
                allBroadcasts.AddRange(broadcasts);
            }

            // Remove duplicates and apply limit
            var linkDetails = allBroadcasts
                .GroupBy(b => b.id)
                .Select(g => g.First())
                .OrderByDescending(b => b.title)
                .Take(limit);

            return linkDetails;
        }

        public async Task<IEnumerable<LinkDetails>> ListUpcomingBroadcastUrls()
            => await ListBroadcastUrls(BroadcastFilter.Upcoming);
    }
}