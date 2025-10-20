using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouTubeCLI.Commands
{
    [Command(Description = "End YouTube Broadcasts")]
    public class EndCommand : CommandsBase
    {
        [Required]
        [Option("-i|--id <path>", Description = "End broadcasts by id or comma separated list of is")]
        public string Id { get; set; }

        private YouTubeCLI Parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = Parent?.CreateArgs() ?? base.CreateArgs();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                args.Add("id");
                args.Add(Id);
            }

            return args;
        }
    }
}