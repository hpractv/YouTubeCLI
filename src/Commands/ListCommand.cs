using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
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

        [Option("--filter <value>", "Filter broadcasts by status (all, upcoming, active, completed). Accepts comma-separated values (e.g., 'upcoming,active'). Default: all", CommandOptionType.SingleValue)]
        public string FilterString { get; set; } = "all";

        public BroadcastFilter[] Filter
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FilterString))
                    return new[] { BroadcastFilter.All };

                var filters = FilterString.Split(',')
                    .Select(f => BroadcastFilterExtensions.FromString(f.Trim()))
                    .Distinct()
                    .ToArray();

                return filters.Length > 0 ? filters : new[] { BroadcastFilter.All };
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

                var filterText = (Filter.Length == 1 && Filter[0] == BroadcastFilter.All) 
                    ? "" 
                    : $" {string.Join(", ", Filter)}";
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

            if (!string.IsNullOrWhiteSpace(FilterString) && !FilterString.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                _args.Add("filter");
                _args.Add(FilterString);
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