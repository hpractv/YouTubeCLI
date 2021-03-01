using System;

namespace YouTubeCLI.Models
{
    public class Broadcast
    {
        public string id { get; set; }
        public string name { get; set; }
        public int dayOfWeek { get; set; }
        public string broadcastStart { get; set; }
        public string stream { get; set; }
        public bool autoStart { get; set; }
        public string thumbnail { get; set; }
        public bool active { get; set; }
    }
}