namespace YouTubeCLI.Models
{
    public class LinkDetails
    {
        public string title { get; set; }
        public string id { get; set; }
        public string broadcastUrl => $"https://youtu.be/{id}";
        public string embeddedCode => $@"<iframe width=""425"" height=""344"" src=""https://www.youtube.com/embed/{id}?autoplay=1&livemonitor=1"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"" allowfullscreen></iframe>";
    }
}


