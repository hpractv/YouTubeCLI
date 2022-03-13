using YouTubeCLI.Models;

namespace YouTubeCLI.Commands
{
    public interface IBroadcastFile
    {
        string BroadcastFile { get; set; }
        Broadcasts broadcasts { get; set; }
    }
}