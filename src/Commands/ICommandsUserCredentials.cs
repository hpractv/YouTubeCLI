namespace YouTubeCLI.Commands;

public interface ICommandsUserCredentials {
    public string YouTubeUser { get; set; }
    public string ClientSecretsFile { get; set; }
}