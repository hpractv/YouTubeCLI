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

namespace YouTubeCLI.Commands
{
    [Command(Description = "Create YouTube Broadcasts")]
    public class CreateCommand : CommandsBase, IBroadcastFile
    {
        [Required, FileExists, Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        public string BroadcastFile { get; set; }

        [Required, FileExists, Option("-c|--client-secrets <path>", Description = "Client secrets file for authentication and authorization.")]
        internal string ClientSecretsFile { get; set; }

        [Option("-o|--occurences <int>", Description = "Number of stream events to create. Defaults to 1.")]
        internal int Occurrences { get; set; } = 1;

        public Broadcasts broadcasts { get; set; }

        [Option(
            Template = "-i|--id <value>",
            Description = "Create broadcasts by id specified in a json configuration file 'id1[,id2,id3...]'")]
        public string Id { get; set; }

        private YouTubeCLI Parent { get; set; }

        public override List<string> CreateArgs()
        {
            var _args = Parent.CreateArgs();

            if (BroadcastFile != null)
            {
                _args.Add("file");
                _args.Add(BroadcastFile);
            }

            if (ClientSecretsFile != null)
            {
                _args.Add("client-secrets");
                _args.Add(ClientSecretsFile);
            }
            
            _args.Add("occurrences");
            _args.Add(Occurrences.ToString());
            
            if (Id != null)
            {
                _args.Add("id");
                _args.Add(Id);
            }

            return _args;
        }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;
            try
            {
                broadcasts = getBroadcasts(BroadcastFile, ClientSecretsFile);

                var _youTube = new YouTubeLibrary(
                    broadcasts.user,
                    Path.GetDirectoryName(BroadcastFile),
                    broadcasts.clientSecretsFile);

                Console.WriteLine("Creating Broadcasts");

                var _active = broadcasts.Items
                    .Where(b => b.active &&
                       (string.IsNullOrWhiteSpace(Id) ||
                         Id.Split(',').Contains(b.id)));

                Console.WriteLine($"Active Broadcast Count: {_active.Count()}");
                Console.WriteLine("==================================");

                if (TestMode)
                {
                    Console.WriteLine("Test Mode: Only the first broadcast will be created and set to private");
                    Console.WriteLine("==================================");
                }
                foreach (var _broadcast in _active.Take(TestMode ? 1 : _active.Count()))
                {
                    Console.WriteLine($"Creating {_broadcast.name}...");
                    var _builtBroadcast = Task.Run<IEnumerable<LiveBroadcastSnippet>>(
                        () => _youTube.BuildBroadCast(_broadcast, Occurrences, TestMode),
                        new CancellationToken());
                    _builtBroadcast.Wait();
                    if (_builtBroadcast.IsCompletedSuccessfully)
                    {
                        foreach (var b in _builtBroadcast.Result)
                        {
                            Console.WriteLine($"{b.Title} Created.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Creation Error: {_broadcast.name}");
                    }
                    Console.WriteLine("==================================");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error creating broadcasts: {exc.Message}");
                _success = 1;
            }
            finally
            {
            }
            return _success;
        }
    }
}