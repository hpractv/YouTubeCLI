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

        [Required]
        [Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        [FileExists]
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
                // return base.OnExecute(app);
                Console.WriteLine("Creating Broadcasts");
                var _broadcasts = BroadcastLibrary.GetBroadcasts(BroadcastFile);
                var _youTube = new YouTubeLibrary(_broadcasts.user);
                Console.WriteLine($"Count: {_broadcasts.Items.Count()}");

                var _create = _broadcasts.Items
                    .Where(b => b.active &&
                       (string.IsNullOrWhiteSpace(b.id) ||
                         Id.Split(',').Contains(b.id)));

                if (TestMode)
                {
                    Console.WriteLine("Test Mode: Only the first broadcast will be created and set to private");
                }
                foreach (var _broadcast in _create.Take(TestMode ? 1 : _create.Count()))
                {
                    Console.WriteLine($"{_broadcast.name}");
                    var _buildBroadcast = Task.Run(
                        () => _youTube.BuildBroadCast(_broadcast, TestMode),
                        new CancellationToken());
                    _buildBroadcast.Wait();
                    Console.WriteLine($"{_broadcast.name} Created");
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