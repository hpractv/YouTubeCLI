using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace YouTubeCLI.Commands
{
    [HelpOption("--help")]
    public abstract class CommandsBase
    {
        public abstract List<string> CreateArgs();

        protected virtual int OnExecute(CommandLineApplication app)
        {
            var args = CreateArgs();

            Console.WriteLine("Result = ytc " + ArgumentEscaper.EscapeAndConcatenate(args));
            return 0;
        }
    }
}