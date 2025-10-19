using McMaster.Extensions.CommandLineUtils;
using YouTubeCLI.Commands;
using YouTubeCLI.Utilities;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace YouTubeCLI
{
    [Command("ytc")]
    [VersionOptionFromMember("-v|--version", MemberName = nameof(GetVersion))]
    [Subcommand(
        typeof(CreateCommand),
        typeof(UpdateCommand),
        typeof(ListCommand),
        typeof(EndCommand))]
    public class YouTubeCLI : CommandsBase
    {
        public static void Main(string[] args)
        {
            // Display OS information on startup
            Console.WriteLine($"Running on: {OSDetection.GetOSInfo()}");
            CommandLineApplication.Execute<YouTubeCLI>(args);
        }

        private static string GetVersion()
            => typeof(YouTubeCLI).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
