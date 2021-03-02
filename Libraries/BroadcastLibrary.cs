using System;
using System.IO;
using System.Text.Json;
using YouTubeCLI.Models;

namespace YouTubeCLI.Libraries {
    public class BroadcastLibrary
    {
        public static Broadcasts GetBroadcasts(string broadcastFile)
        {
            var _file = File.ReadAllText($@"{broadcastFile}");
            var _broadcasts = JsonSerializer.Deserialize<Broadcasts>(_file);
            return _broadcasts;
        }
    }
}