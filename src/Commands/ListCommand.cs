using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Commands
{

    [Command(Description = "List YouTube Broadcasts")]
    public class ListCommand : CommandsBase, ICommandsUserCredentials, IBroadcastFile
    {
        [Required, Option("-u|--youtube-user <string>", Description = "YouTube User Id")]
        public string YouTubeUser { get; set; }

        [Required, FileExists, Option("-c|--client-secrets <path>", Description = "Client secrets file for authentication and authorization")]
        public string ClientSecretsFile { get; set; }

        [Required, FileExists, Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        public string BroadcastFile { get; set; }

        public Broadcasts broadcasts { get; set; }

        [Option("--upcoming", "Filter to show only upcoming broadcasts", CommandOptionType.NoValue)]
        public bool Upcoming { get; set; }

        [Option("--active", "Filter to show only active broadcasts", CommandOptionType.NoValue)]
        public bool Active { get; set; }

        [Option("--completed", "Filter to show only completed broadcasts", CommandOptionType.NoValue)]
        public bool Completed { get; set; }

        public BroadcastFilter Filter
        {
            get
            {
                if (Upcoming) return BroadcastFilter.Upcoming;
                if (Active) return BroadcastFilter.Active;
                if (Completed) return BroadcastFilter.Completed;
                return BroadcastFilter.All;
            }
        }

        [Option("-n|--limit <int>", "Limit the number of broadcasts returned. Default: 100", CommandOptionType.SingleValue)]
        public int Limit { get; set; } = 100;

        private YouTubeCLI Parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;
            try
            {
                broadcasts = getBroadcasts(BroadcastFile);

                var _youTubeLibrary = new YouTubeLibrary(YouTubeUser, ClientSecretsFile);
                ClearCredentialsIfRequested(_youTubeLibrary);
                var _broadcasts = Task.Run<IEnumerable<LinkDetails>>(() => _youTubeLibrary.ListBroadcastUrls(Filter, Limit));
                _broadcasts.Wait();

                var filterText = Filter == BroadcastFilter.All ? "" : $" {Filter}";
                Console.WriteLine($"Getting{filterText} Broadcasts{(Limit != 100 ? $" (limit: {Limit})" : "")}.");
                foreach (var _broadcast in _broadcasts.Result)
                {
                    Console.WriteLine($"{_broadcast.title} ({_broadcast.privacyStatus}): {_broadcast.broadcastUrl}");
                }
            }
            catch (Exception _exc)
            {
                Console.WriteLine($"Error Listing Broadcasts: {_exc.Message}");
                _success = 1;
            }

            return _success;
        }

        public override List<string> CreateArgs()
        {
            var _args = Parent?.CreateArgs() ?? base.CreateArgs();

            if (!string.IsNullOrWhiteSpace(ClientSecretsFile))
            {
                _args.Add("client-secrets");
                _args.Add(ClientSecretsFile);
            }

            if (Upcoming)
            {
                _args.Add("upcoming");
            }
            else if (Active)
            {
                _args.Add("active");
            }
            else if (Completed)
            {
                _args.Add("completed");
            }

            if (Limit != 100)
            {
                _args.Add("limit");
                _args.Add(Limit.ToString());
            }
            return _args;
        }
    }
}