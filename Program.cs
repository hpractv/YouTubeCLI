using McMaster.Extensions.CommandLineUtils;
using YouTubeCLI.Commands;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace YouTubeCLI
{

    // [VersionOptionFromMember("-v,--version", MemberName = nameof(GetVersion))]
    // class Program
    // {
    //     static void Main(string[] args)
    //         => CommandLineApplication.Execute<Broadcast>(args);

    //     private static string GetVersion()
    //         => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    // }


    /// <summary>
    /// In this example, each sub command type inherits from <see cref="CommandsBase"/>,
    /// which provides shared functionality between all the commands.
    /// This example also shows you how the subcommands can be linked to their parent types.
    /// </summary>
    [Command("ytc")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(
        typeof(CreateCommand),
        typeof(CommitCommand))]
    public class YouTubeCLI : CommandsBase
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<YouTubeCLI>(args);

        // [Option("-C <path>")]
        // [FileExists]
        // public string ConfigFile { get; set; }

        // [Option("-c <name>=<value>")]
        // public string[] ConfigSetting { get; set; }

        // [Option("--exec-path[:<path>]")]
        // public (bool hasValue, string value) ExecPath { get; set; }

        // [Option("--bare")]
        // public bool Bare { get; }

        // [Option("--git-dir=<path>")]
        // [DirectoryExists]
        // public string GitDir { get; set; }

        // protected override int OnExecute(CommandLineApplication app)
        // {
        //     // this shows help even if the --help option isn't specified
        //     app.ShowHelp();
        //     return 1;
        // }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            // if (GitDir != null)
            // {
            //     args.Add("--git-dir=" + GitDir);
            // }

            return args;
        }

        private static string GetVersion()
            => typeof(YouTubeCLI).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }



    [Command(Description = "Record changes to the repository")]
    class CommitCommand : CommandsBase
    {
        [Option("-m")]
        public string Message { get; set; }

        // This will automatically be set before OnExecute is invoked.
        private YouTubeCLI Parent { get; set; }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("commit");

            if (Message != null)
            {
                args.Add("-m");
                args.Add(Message);
            }

            return args;
        }
    }


}
