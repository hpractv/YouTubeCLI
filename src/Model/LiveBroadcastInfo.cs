using System;

namespace YouTubeCLI.Models
{
    public class LiveBroadcastInfo
    {
        public string youTubeId { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public bool autoStart { get; set; }
        public bool autoStop { get; set; }
        public string privacy { get; set; }
    }
}


