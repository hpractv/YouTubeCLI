using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        [Option(
            "-l|--clear-credential",
            "Clear authorization credentials.",
            CommandOptionType.NoValue)]
        internal bool ClearCredential { get; set; }

        internal Broadcast getBroadcast(string broadcastFile, string id)
            => getBroadcasts(broadcastFile).Items.Single(b => b.id == id);

        internal Broadcasts getBroadcasts(string broadcastFile)
            => BroadcastLibrary.GetBroadcasts(broadcastFile);

        public virtual List<string> CreateArgs()
        {
            var _args = new List<string>();
            if (TestMode)
            {
                _args.Add("testmode");
            }
            _args.Add("clear-credential");
            _args.Add(ClearCredential.ToString());
            return _args;
        }

        protected void ClearCredentialsIfRequested(YouTubeLibrary youTube)
        {
            if (ClearCredential)
            {
                Console.WriteLine("Clearing Credentials...");
                youTube.ClearCredential().Wait();
                Console.WriteLine("==================================");
            }
        }

        protected virtual int OnExecute(CommandLineApplication app)
        {
            var args = CreateArgs();
            Console.WriteLine("Result = ytc " + ArgumentEscaper.EscapeAndConcatenate(args));
            return 0;
        }
    }
}