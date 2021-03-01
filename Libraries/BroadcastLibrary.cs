using System;
using System.IO;
using System.Text.Json;
using YouTubeCLI.Models;

namespace YouTubeCLI.Libraries {
    public class BroadcastLibrary : LibraryBase
    {
        private string _broadcastFile;
        public BroadcastLibrary(string broadcastFile) {
            this._broadcastFile = broadcastFile;
        }

        private Broadcasts _broadcasts;
        public Broadcasts broadcasts {
            get
            {
                if (_broadcasts == null)
                {
                    var _file = File.ReadAllText($@"{_directory}\{_broadcastFile}");
                    _broadcasts = JsonSerializer.Deserialize<Broadcasts>(_file);
                }
                return _broadcasts;
            }
        }
    }
}