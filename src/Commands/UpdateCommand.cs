using Google.Apis.YouTube.v3.Data;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

using analyticsLibrary.library;

namespace YouTubeCLI.Commands
{
    [Command(Description = "Update Broadcast")]
    public class UpdateCommand : CommandsBase, ICommandsUserCredentials
    {
        [Required, Option("-u|--youtube-user <string>", Description = "YouTube User Id")]
        public string YouTubeUser { get; set; }

        [Required, FileExists, Option("-c|--client-secrets <path>", Description = "Client secrets file for authentication and authorization")]
        public string ClientSecretsFile { get; set; }

        [FileExists, Option("-f|--file <path>", Description = "Update broadcasts csv file or YouTube id must be specified")]
        public string BroadcastFile { get; set; }

        [Option(
            Template = "-y|--youtube-id <value>",
            Description = "Update broadcast by YouTube id or broadcasts csv file must be specified")]
        public string YouTubeId { get; set; }

        [Option("-a|--auto-start <value>", "Set auto start to true or false",
            CommandOptionType.SingleOrNoValue)]
        public bool? AutoStart { get; set; }

        [Option("-o|--auto-stop <value>", "Set auto stop to true or false",
            CommandOptionType.SingleOrNoValue)]
        public bool? AutoStop { get; set; }

        [Option("-p|--privacy", "Set privacy to public, unlisted, or private",
            CommandOptionType.SingleOrNoValue)]
        public PrivacyEnum? Privacy { get; set; }

        [Option("-e|--chat-enabled <value>", "Set chat enabled to true or false",
            CommandOptionType.SingleOrNoValue)]
        public bool? ChatEnabled { get; set; }

        private YouTubeCLI Parent { get; set; }

        public override List<string> CreateArgs()
        {
            var _args = Parent?.CreateArgs() ?? base.CreateArgs();

            _args.Add("youtube-user");
            _args.Add(YouTubeUser);

            _args.Add("client-secrets");
            _args.Add(ClientSecretsFile);

            if (!string.IsNullOrWhiteSpace(BroadcastFile))
            {
                _args.Add("file");
                _args.Add(BroadcastFile);
            }

            if (!string.IsNullOrWhiteSpace(YouTubeId))
            {
                _args.Add("youtube-id");
                _args.Add(YouTubeId);
            }

            if (AutoStart != null)
            {
                _args.Add("auto-start");
                _args.Add(AutoStart.ToString());
            }

            if (AutoStop != null)
            {
                _args.Add("auto-stop");
                _args.Add(AutoStop.ToString());
            }

            if (Privacy != null)
            {
                _args.Add("privacy");
                _args.Add(Privacy.ToString());
            }

            if (ChatEnabled != null)
            {
                _args.Add("chat-enabled");
                _args.Add(ChatEnabled.ToString());
            }

            return _args;
        }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(BroadcastFile) && string.IsNullOrWhiteSpace(YouTubeId) || (!string.IsNullOrWhiteSpace(BroadcastFile) && !string.IsNullOrWhiteSpace(YouTubeId)))
                {
                    throw new ArgumentNullException("Either a broadcast csv file or YouTube Id may be specified");
                }

                var _youTube = new YouTubeLibrary(
                    YouTubeUser,
                    ClientSecretsFile);

                ClearCredentialsIfRequested(_youTube);

                if (!string.IsNullOrWhiteSpace(YouTubeId))
                {
                    _updateBroadcast(_youTube, YouTubeId, AutoStart, AutoStop, Privacy, ChatEnabled);
                }

                if (!string.IsNullOrWhiteSpace(BroadcastFile))
                {
                    var broadcastCsv = new csv(BroadcastFile);
                    foreach (var b in broadcastCsv.data)
                    {
                        var autoStart = bool.Parse(b[Constants.AutoStart_COLUMN]);
                        var autoStop = bool.Parse(b[Constants.AutoStop_COLUMN]);
                        var privacy = (PrivacyEnum)Enum.Parse(typeof(PrivacyEnum), b[Constants.Privacy_COLUMN], true);
                        var chatEnabled = ChatEnabled;

                        _updateBroadcast(_youTube, b[Constants.YouTubeId_COLUMN], autoStart, autoStop, privacy, chatEnabled);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error update broadcast(s): {exc.Message}");
                _success = 1;
            }
            finally
            {
            }
            return _success;

            void _updateBroadcast(YouTubeLibrary youTubelibrary, string youTubeId, bool? autoStart, bool? autoStop, PrivacyEnum? privacy, bool? chatEnabled)
            {
                Console.WriteLine($"Updating Broadcast: {youTubeId}");
                var _updateBroadcast = Task.Run(() => youTubelibrary.UpdateBroadcast(youTubeId, autoStart, autoStop, privacy, chatEnabled));
                _updateBroadcast.Wait();
                if (_updateBroadcast.IsCompletedSuccessfully)
                {
                    Console.WriteLine($"Broadcast updated successfully: {YouTubeId}");
                }
                else
                {
                    Console.WriteLine($"Error updating broadcast: {YouTubeId}");
                }
            }
        }
    }
}