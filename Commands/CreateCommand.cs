using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
                var _youTubelibrary = new YouTubeLibrary();
                var _broadcastsLibarary = new BroadcastLibrary(BroadcastFile);
                Console.WriteLine($"Count: {_broadcastsLibarary.broadcasts.Items.Count()}");

                var _create = _broadcastsLibarary.broadcasts.Items
                    .Where(b => b.active &&
                       (string.IsNullOrWhiteSpace(b.id) ||
                         Id.Split(',').Contains(b.id)));

                foreach (var _broadcast in _create)
                {
                    Console.WriteLine($"{_broadcast.name}");
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