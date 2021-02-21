using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace YouTubeCLI.Commands
{

    [Command(Description = "Create YouTube Broadcasts")]
    class CreateCommand : CommandsBase
    {

        [Option("-a|--all <path>", Description = "Create broadcasts from a json configuration file")]
        [FileExists]
        public string BroadcastFile { get; set; }

        private YouTubeCLI Parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("all");

            if (BroadcastFile != null)
            {
                args.Add(BroadcastFile);
            }

            return args;
        }
    }
}