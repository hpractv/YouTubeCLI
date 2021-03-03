using Google.Apis.YouTube.v3.Data;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YouTubeCLI.Libraries;

namespace YouTubeCLI.Commands
{

    [Command(Description = "Create YouTube Broadcasts")]
    public class CreateCommand : CommandsBase
    {

        [Required, FileExists, Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        public string BroadcastFile { get; set; }

        [Option(
            Template = "-i|--id <value>",
            Description = "Create broadcasts by id specified in a json configuration file 'id1[,id2,id3...]'")]
        public string Id { get; set; }

        private YouTubeCLI Parent { get; set; }


        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();

            if (BroadcastFile != null)
            {
                args.Add("file");
                args.Add(BroadcastFile);
            }

            if (Id != null)
            {
                args.Add("id");
                args.Add(Id);
            }

            return args;
        }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;
            try
            {

                var _broadcasts = BroadcastLibrary.GetBroadcasts(BroadcastFile);
                var _youTube = new YouTubeLibrary(_broadcasts.user);

                Console.WriteLine("Creating Broadcasts");

                var _active = _broadcasts.Items
                    .Where(b => b.active &&
                       (string.IsNullOrWhiteSpace(Id) ||
                         Id.Split(',').Contains(b.id)));

                Console.WriteLine($"Active Broadcast Count: {_active.Count()}");
                Console.WriteLine("==================================");

                if (TestMode)
                {
                    Console.WriteLine("Test Mode: Only the first broadcast will be created and set to private");
                    Console.WriteLine("==================================");
                }
                foreach (var _broadcast in _active.Take(TestMode ? 1 : _active.Count()))
                {
                    Console.WriteLine($"Creating {_broadcast.name}...");
                    var _buildBroadcast = Task.Run<LiveBroadcastSnippet>(
                        () => _youTube.BuildBroadCast(_broadcast, TestMode),
                        new CancellationToken());
                    _buildBroadcast.Wait();
                    if (_buildBroadcast.IsCompletedSuccessfully)
                    {
                        Console.WriteLine($"{_buildBroadcast.Result.Title} Created.");
                    }
                    else
                    {
                        Console.WriteLine($"Creation Error: {_broadcast.name}");
                    }
                    Console.WriteLine("==================================");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error creating broadcasts: {exc.Message}");
                _success = 1;
            }
            finally
            {
            }
            return _success;
        }
    }
}