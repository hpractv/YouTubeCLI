using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        internal Broadcasts getBroadcasts(string broadcastFile)
            => BroadcastLibrary.GetBroadcasts(broadcastFile);

        public virtual List<string> CreateArgs()
        {
            var _args = new List<string>();
            if (TestMode)
            {
                _args.Add("testmode");
            }
            return _args;
        }

        protected virtual int OnExecute(CommandLineApplication app)
        {
            var args = CreateArgs();
            Console.WriteLine("Result = ytc " + ArgumentEscaper.EscapeAndConcatenate(args));
            return 0;
        }
    }
}