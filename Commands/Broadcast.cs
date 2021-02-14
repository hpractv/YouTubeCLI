using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace YouTubeCLI.Commands
{

    [Command(Description = "YouTube Command Line Interface")]
    public class Broadcast : CommandsBase
    {
        public override List<string> CreateArgs()
        {
            var _args = new List<string>();
            return _args;
        }
    }

    [Command(Description = "Create broadcast(s)")]
    public class CreateCommand : CommandsBase
    {
        [Option("-c <id>", "create ids", CommandOptionType.MultipleValue)]
        public string[] Create { get; set; }

        [Option("-ca")]
        public bool CreateAll { get; set; }


        private Broadcast _parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = _parent.CreateArgs();

            args.Add("create");
            args.Add("createall");

            return args;
        }

    }

    [Command(Description = "End broadcast(s)")]
    public class EndCommand : CommandsBase
    {
        [Option("-e <id>", "create ids", CommandOptionType.MultipleValue)]
        public string[] End { get; set; }

        [Option("-ea")]
        public bool EndAAll { get; set; }


        private Broadcast _parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = _parent.CreateArgs();

            args.Add("end");
            args.Add("endall");

            return args;
        }

    }
}