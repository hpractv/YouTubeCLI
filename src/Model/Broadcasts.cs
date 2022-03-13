using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YouTubeCLI.Models
{
    public class Broadcasts
    {
        public string user { get; set; }

        public string clientSecretsFile { get; set; }

        [JsonPropertyName("broadcasts")]
        public IEnumerable<Broadcast> Items { get; set; }
    }
}