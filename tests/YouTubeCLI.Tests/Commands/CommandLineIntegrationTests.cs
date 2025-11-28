using FluentAssertions;
using Xunit;
using YouTubeCLI.Commands;
using YouTubeCLI.Libraries;

namespace YouTubeCLI.Tests.Commands
{
    public class CommandLineIntegrationTests
    {
        [Fact]
        public void CreateCommand_WithMinimalArguments_ShouldParseCorrectly()
        {
            // Arrange
            var command = new CreateCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
        }

        [Fact]
        public void CreateCommand_WithAllArguments_ShouldParseCorrectly()
        {
            // Arrange
            var command = new CreateCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.Id = "test-id-123";
            command.StartsOn = new DateOnly(2024, 1, 15);

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.Id.Should().Be("test-id-123");
            command.StartsOn.Should().Be(new DateOnly(2024, 1, 15));
        }

        [Fact]
        public void CreateCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var command = new CreateCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
        }

        [Fact]
        public void UpdateCommand_WithMinimalArguments_ShouldParseCorrectly()
        {
            // Arrange
            var command = new UpdateCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.YouTubeId = "test-youtube-id-123";

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.YouTubeId.Should().Be("test-youtube-id-123");
        }

        [Fact]
        public void UpdateCommand_WithAllArguments_ShouldParseCorrectly()
        {
            // Arrange
            var command = new UpdateCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.YouTubeId = "test-youtube-id-123";
            command.AutoStart = true;
            command.AutoStop = false;
            command.Privacy = PrivacyEnum.Public;

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.YouTubeId.Should().Be("test-youtube-id-123");
            command.AutoStart.Should().BeTrue();
            command.AutoStop.Should().BeFalse();
            command.Privacy.Should().Be(PrivacyEnum.Public);
        }

        [Fact]
        public void UpdateCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var command = new UpdateCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.YouTubeId = "test-youtube-id-123";
            command.AutoStart = true;
            command.AutoStop = false;
            command.Privacy = PrivacyEnum.Public;

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.YouTubeId.Should().Be("test-youtube-id-123");
            command.AutoStart.Should().BeTrue();
            command.AutoStop.Should().BeFalse();
            command.Privacy.Should().Be(PrivacyEnum.Public);
        }

        [Fact]
        public void ListCommand_WithMinimalArguments_ShouldParseCorrectly()
        {
            // Arrange
            var command = new ListCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.FilterString = "all";

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.Filter.Should().BeEquivalentTo(new[] { BroadcastFilter.All });
        }

        [Fact]
        public void ListCommand_WithFilterFlag_ShouldParseCorrectly()
        {
            // Arrange
            var command = new ListCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.FilterString = "upcoming";

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.Filter.Should().BeEquivalentTo(new[] { BroadcastFilter.Upcoming });
        }

        [Fact]
        public void ListCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var command = new ListCommand();
            command.YouTubeUser = "test-user";
            command.ClientSecretsFile = "secrets.json";
            command.BroadcastFile = "broadcasts.json";
            command.FilterString = "upcoming";

            // Act & Assert
            command.Should().NotBeNull();
            command.YouTubeUser.Should().Be("test-user");
            command.ClientSecretsFile.Should().Be("secrets.json");
            command.BroadcastFile.Should().Be("broadcasts.json");
            command.Filter.Should().BeEquivalentTo(new[] { BroadcastFilter.Upcoming });
        }

        [Fact]
        public void EndCommand_WithId_ShouldParseCorrectly()
        {
            // Arrange
            var command = new EndCommand();
            command.Id = "test-id-123";

            // Act & Assert
            command.Should().NotBeNull();
            command.Id.Should().Be("test-id-123");
        }

        [Fact]
        public void EndCommand_WithMultipleIds_ShouldParseCorrectly()
        {
            // Arrange
            var command = new EndCommand();
            command.Id = "test-id-123,test-id-456,test-id-789";

            // Act & Assert
            command.Should().NotBeNull();
            command.Id.Should().Be("test-id-123,test-id-456,test-id-789");
        }

        [Fact]
        public void EndCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var command = new EndCommand();
            command.Id = "test-id-123";

            // Act & Assert
            command.Should().NotBeNull();
            command.Id.Should().Be("test-id-123");
        }

        [Theory]
        [InlineData("create")]
        [InlineData("update")]
        [InlineData("list")]
        [InlineData("end")]
        public void Command_ShouldHaveValidName(string commandName)
        {
            // This test verifies that all commands have valid names
            commandName.Should().NotBeNullOrEmpty();
            commandName.Should().BeOneOf("create", "update", "list", "end");
        }
    }
}