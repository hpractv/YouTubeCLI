using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YouTubeCLI.Models
{
    public class Broadcasts
    {
        [JsonPropertyName("broadcasts")]
        public IEnumerable<Broadcast> Items { get; set; }
    }
}