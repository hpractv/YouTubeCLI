using McMaster.Extensions.CommandLineUtils;
using YouTubeCLI.Commands;
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
        typeof(EndCommand),
        typeof(ClearCredentialCommand))]
    public class YouTubeCLI : CommandsBase
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<YouTubeCLI>(args);

        private static string GetVersion()
            => typeof(YouTubeCLI).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
