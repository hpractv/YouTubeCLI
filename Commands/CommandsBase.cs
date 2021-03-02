using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace YouTubeCLI.Commands
{
    [HelpOption("-h|--help")]
    public abstract class CommandsBase
    {
        [Option(
           Template = "-t|--test-mode <value>",
           Description = "Create the broadcasts in testing mode.")]
        internal bool TestMode { get; set; }

        public abstract List<string> CreateArgs();

        protected virtual int OnExecute(CommandLineApplication app)
        {
            var args = CreateArgs();

            Console.WriteLine("Result = ytc " + ArgumentEscaper.EscapeAndConcatenate(args));
            return 0;
        }
    }
}