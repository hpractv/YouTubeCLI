using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouTubeCLI.Commands
{

    [Command(Description = "List YouTube Broadcasts")]
    public class ListCommand : CommandsBase
    {

        [Required]
        [Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        [FileExists]
        public string BroadcastFile { get; set; }

         [Option("-i|--id <path>", Description = "Create broadcasts by id from a json configuration file")]
        public string Id { get; set; }

        private YouTubeCLI Parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();

            if (BroadcastFile != null)
            {
                args.Add("file");
                args.Add(BroadcastFile);
            }

            if(Id != null){
                args.Add("id");
                args.Add(Id);
            }

            return args;
        }
    }
}