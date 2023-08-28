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
    public enum OutputOptionsEnum { Single = 1, Monthly = 2, Daily = 3, Hourly = 4, Broadcast = 5 }

    [Command(Description = "Create YouTube Broadcasts")]
    public class CreateCommand : CommandsBase, ICommandsUserCredentials, IBroadcastFile
    {
        [Required, Option("-u|--youtube-user <string>", Description = "YouTube User Id")]
        public string YouTubeUser { get; set; }

        [Required, FileExists, Option("-c|--client-secrets <path>", Description = "Client secrets file for authentication and authorization.")]
        public string ClientSecretsFile { get; set; }

        [Required, FileExists, Option("-f|--file <path>", Description = "Create broadcasts from a json configuration file")]
        public string BroadcastFile { get; set; }


        [Option("-o|--occurences <int>", Description = "Number of stream events to create. Defaults to 1.")]
        internal int Occurrences { get; set; } = 1;

        [Option("-e|--output <int>", Description = "Broadcast output file")]
        internal OutputOptionsEnum[] OutputOptions { get; set; }

         [Option("-p|--output-prefix <string>", Description = "Broadcast output file prefix")]
        internal string OutputPrefix { get; set; }

        public Broadcasts broadcasts { get; set; }

        [Option(
            Template = "-i|--id <value>",
            Description = "Create broadcasts by id specified in a json configuration file 'id1[,id2,id3...]'")]
        public string Id { get; set; }

        private YouTubeCLI Parent { get; set; }

        public override List<string> CreateArgs()
        {
            var _args = Parent.CreateArgs();

            _args.Add("file");
            _args.Add(BroadcastFile);
            _args.Add("client-secrets");
            _args.Add(ClientSecretsFile);
            _args.Add("occurrences");
            _args.Add(Occurrences.ToString());

            if (Id != null)
            {
                _args.Add("id");
                _args.Add(Id);
            }

            var output_options = new List<string>();
            if (OutputOptions.Contains(OutputOptionsEnum.Single)) output_options.Add("Single");
            if (OutputOptions.Contains(OutputOptionsEnum.Monthly)) output_options.Add("Monthly");
            if (OutputOptions.Contains(OutputOptionsEnum.Daily)) output_options.Add("Daily");
            if (OutputOptions.Contains(OutputOptionsEnum.Hourly)) output_options.Add("Hourly");
            if (OutputOptions.Contains(OutputOptionsEnum.Broadcast)) output_options.Add("Broadcast");
            if (output_options.Count() > 0)
            {
                _args.Add("output");
                _args.Add(string.Join(", ", output_options));
            }
            if(!string.IsNullOrWhiteSpace(OutputPrefix)){
                _args.Add("output-prefix");
                _args.Add(OutputPrefix);
            }

            return _args;
        }

        protected override int OnExecute(CommandLineApplication app)
        {
            var _success = 0;
            var _createdBroadcasts = new List<LiveBroadcastInfo>();
            try
            {
                broadcasts = getBroadcasts(BroadcastFile);
                var thumbnailDirectory = Path.GetDirectoryName(BroadcastFile);
                var _youTube = new YouTubeLibrary(YouTubeUser, ClientSecretsFile);
                Console.WriteLine("Creating Broadcasts");

                var _active = broadcasts.Items
                    .Where(b => b.active &&
                       (string.IsNullOrWhiteSpace(Id) ||
                         Id.Split(',').Contains(b.id)));

                Console.WriteLine($"Active Broadcast Count: {_active.Count()}");
                Console.WriteLine("==================================");

                if (TestMode)
                {
                    Console.WriteLine("Test Mode: Only the first broadcast will be created");
                    Console.WriteLine("==================================");
                }
                foreach (var _broadcast in _active.Take(TestMode ? 1 : _active.Count()))
                {
                    Console.WriteLine($"Creating {_broadcast.name}...");
                    var _builtBroadcast = Task.Run<IEnumerable<LiveBroadcastInfo>>(
                        () => _youTube.BuildBroadCast(_broadcast, Occurrences, thumbnailDirectory, TestMode),
                        new CancellationToken());
                    _builtBroadcast.Wait();
                    if (_builtBroadcast.IsCompletedSuccessfully)
                    {
                        _createdBroadcasts.AddRange(_builtBroadcast.Result);
                        foreach (var b in _builtBroadcast.Result)
                        {
                            Console.WriteLine($"{b.title} Created.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Creation Error: {_broadcast.name}");
                    }
                    Console.WriteLine("==================================");
                }

                if (_createdBroadcasts.Any())
                {
                    var _outputColumns = Constants.COLUMNS;
                    var _outputPrefix = !string.IsNullOrWhiteSpace(OutputPrefix) ? $"_{OutputPrefix}" : string.Empty;

                    var _outputBroadcasts = new Action<OutputOptionsEnum, Func<LiveBroadcastInfo, string>>((aggregate, aggregateFunction) => {
                        IEnumerable<IGrouping<string, LiveBroadcastInfo>> _aggregated = null;
                        if(aggregateFunction == null){
                            _aggregated = _createdBroadcasts
                                .GroupBy(b => "1 == 1");
                        } else {
                            _aggregated = _createdBroadcasts
                                .GroupBy(b => aggregate != OutputOptionsEnum.Broadcast ? aggregateFunction(b) : $"broadcastId-{aggregateFunction(b)}");
                        }

                        foreach(var _g in _aggregated){
                            var _path = Path.Combine(thumbnailDirectory,
                                $"{(aggregate == OutputOptionsEnum.Single ? "ALL" : _g.Key)}{_outputPrefix}_broadcasts_info.csv");
                            _g.OrderBy(i => i.start)
                                .Select(b => new object[] { b.youTubeId, b.title, b.start, b.autoStart, b.autoStop, b.privacy, b.url, b.link })
                                .writeCsv(Path.Combine(thumbnailDirectory, _path), _outputColumns);
                        }
                    });

                    if (OutputOptions.Contains(OutputOptionsEnum.Single))
                        _outputBroadcasts(OutputOptionsEnum.Single, null);

                    if (OutputOptions.Contains(OutputOptionsEnum.Monthly))
                        _outputBroadcasts(OutputOptionsEnum.Monthly, b => b.start.ToString("yyyyMM"));

                    if (OutputOptions.Contains(OutputOptionsEnum.Daily))
                        _outputBroadcasts(OutputOptionsEnum.Daily, b => b.start.ToString("yyyyMMdd"));

                    if (OutputOptions.Contains(OutputOptionsEnum.Hourly))
                        _outputBroadcasts(OutputOptionsEnum.Hourly, b => b.start.ToString("yyyyMMddhh"));

                    if (OutputOptions.Contains(OutputOptionsEnum.Broadcast))
                       _outputBroadcasts(OutputOptionsEnum.Broadcast, b => b.broadcast);
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