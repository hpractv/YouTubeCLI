using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace YouTubeCLI.Commands
{

    [Command(Description = "Record changes to the repository")]
    class Broadcast : CommandsBase
    {
        [Option("-c <id>", "create ids", CommandOptionType.MultipleValue)]
        public string[] Create { get; set; }

        [Option("-ca")]
        public bool CreateAll { get; set; }

        public override List<string> CreateArgs()
        {
            var _args = new List<string>();
            if (Create != null && Create.Length == 0)
            {
                _args.Add("-c requires an id");
            }

            return _args;
        }
    }
}