using Google.Apis.YouTube.v3.Data;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;
using analyticsLibrary.library;

namespace YouTubeCLI.Commands
{

    [Command(Description = "Clear YouTube Authorization")]
    public class ClearCredentialCommand : CommandsBase
    {
        [Required, Option(
            "-l|--clear-credential",
            "Clear authorizaiton credentials.",
            CommandOptionType.NoValue)]
        internal bool ClearCredential { get; set; }

        private YouTubeCLI Parent { get; set; }

        public override List<string> CreateArgs()
        {
            var _args = Parent.CreateArgs();

            _args.Add("clear-credential");
            _args.Add(ClearCredential.ToString());

            return _args;
        }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;

            if (ClearCredential)
            {
                var _youTube = new YouTubeLibrary();
                _youTube.ClearCredential();
            }
            return _success;
        }
    }
}