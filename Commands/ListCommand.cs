using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Commands
{

    [Command(Description = "List YouTube Broadcasts")]
    public class ListCommand : CommandsBase, IBroadcastFile
    {
        [Required, FileExists, Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        public string BroadcastFile { get; set; }

        public Broadcasts broadcasts { get; set; }

        [Option("-p|--upcoming", "List only upcoming broadcasts", CommandOptionType.NoValue)]
        public bool Upcoming { get; set; }

        private YouTubeCLI Parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;
            try
            {
                broadcasts = getBroadcasts(BroadcastFile);

                var _youTubeLibrary = new YouTubeLibrary(
                    broadcasts.user,
                    broadcasts.credentials);

                var _broadcasts = Task.Run<IEnumerable<LinkDetails>>(() => _youTubeLibrary.ListBroadcastUrls(Upcoming));
                _broadcasts.Wait();

                Console.WriteLine($"Getting{(Upcoming ? " Upcoming" : "")} Broadcasts.");
                foreach (var _broadcast in _broadcasts.Result)
                {
                    Console.WriteLine($"{_broadcast.title}: {_broadcast.broadcastUrl}");
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
            var args = Parent.CreateArgs();
            if (Upcoming)
            {
                args.Add("upcoming");
            }
            return args;
        }
    }
}