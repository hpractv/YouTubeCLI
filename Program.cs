using McMaster.Extensions.CommandLineUtils;
using System;
using YouTubeCLI.Commands;

namespace YouTubeCLI
{
    class Program
    {
        static void Main(string[] args)
            => CommandLineApplication.Execute<Broadcast>(args);
    }
}
