using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Commands
{
    [HelpOption("-h|--help")]
    public abstract class CommandsBase
    {
        [Option(
           "-t|--test-mode",
           "Create the first active broadcast with private visibility.",
           CommandOptionType.NoValue)]
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