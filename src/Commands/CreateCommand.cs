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
    public enum OutputOptionsEnum { SingleCsv = 1, MonthlyCsv = 2, DailyCsv = 3, HourlyCsv = 4 }

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
            if (OutputOptions.Contains(OutputOptionsEnum.SingleCsv)) output_options.Add("SingleCsv");
            if (OutputOptions.Contains(OutputOptionsEnum.MonthlyCsv)) output_options.Add("MonthlyCsv");
            if (OutputOptions.Contains(OutputOptionsEnum.DailyCsv)) output_options.Add("DailyCsv");
            if (OutputOptions.Contains(OutputOptionsEnum.HourlyCsv)) output_options.Add("HourlyCsv");
            if (output_options.Count() > 0)
            {
                _args.Add("output");
                _args.Add(string.Join(", ", output_options));
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
                    Console.WriteLine("Test Mode: Only the first broadcast will be created and set to private");
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

                    var _outputBroadcasts = new Action<string, Func<LiveBroadcastInfo, string>>((aggregateName, aggregate) => {
                        IEnumerable<IGrouping<string, LiveBroadcastInfo>> _aggregated = null;
                        if(aggregate == null){
                            _aggregated = _createdBroadcasts
                                .GroupBy(b => "1 == 1");
                        } else {
                            _aggregated = _createdBroadcasts
                                .GroupBy(b => aggregate(b));
                        }

                        foreach(var _g in _aggregated){
                            var _path = Path.Combine(thumbnailDirectory, $"{(aggregateName == "ALL" ? "ALL" : _g.Key)}_broadcasts_info.csv");
                            _g.OrderBy(i => i.start)
                                .Select(b => new object[] { b.youTubeId, b.title, b.start, b.autoStart, b.autoStop, b.privacy })
                                .writeCsv(Path.Combine(thumbnailDirectory, _path), _outputColumns);
                        }
                    });

                    if (OutputOptions.Contains(OutputOptionsEnum.SingleCsv))
                    {
                        _outputBroadcasts("ALL", null);
                    }

                    if (OutputOptions.Contains(OutputOptionsEnum.MonthlyCsv))
                    {
                        _outputBroadcasts("MONTH", b => b.start.ToString("yyyyMM"));
                    }

                    if (OutputOptions.Contains(OutputOptionsEnum.DailyCsv))
                    {
                         _outputBroadcasts("DAY", b => b.start.ToString("yyyyMMdd"));
                    }

                    if (OutputOptions.Contains(OutputOptionsEnum.HourlyCsv))
                    {
                        _outputBroadcasts("HOUR", b => b.start.ToString("yyyyMMddhh"));
                    }
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