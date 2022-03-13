using System;
using System.IO;
using System.Text.Json;
using YouTubeCLI.Models;

namespace YouTubeCLI.Libraries {
    public class BroadcastLibrary
    {
        public static Broadcasts GetBroadcasts(string broadcastFile, string clientSecretsFile)
        {
            var _file = File.ReadAllText($@"{broadcastFile}");
            var _broadcasts = JsonSerializer.Deserialize<Broadcasts>(_file);
            _broadcasts.clientSecretsFile = clientSecretsFile;
            return _broadcasts;
        }
    }
}