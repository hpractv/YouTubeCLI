using McMaster.Extensions.CommandLineUtils;
using System.Reflection;
using YouTubeCLI.Commands;

namespace YouTubeCLI
{
    [Command("fake-git")]
    [VersionOptionFromMember("-v,--version", MemberName = nameof(GetVersion))]
    [Subcommand(
        typeof(CreateCommand),
        typeof(EndCommand))]
    class Program
    {
        static void Main(string[] args)
            => CommandLineApplication.Execute<Broadcast>(args);

        private static string GetVersion()
         => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
