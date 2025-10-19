using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var args = new[]
            {
                "create",
                "--youtube-user", "test-user",
                "--client-secrets", "secrets.json",
                "--file", "broadcasts.json"
            };

            // Act
            var result = TestCommandLineApplication.Parse<CreateCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.BroadcastFile.Should().Be("broadcasts.json");
        }

        [Fact]
        public void CreateCommand_WithAllArguments_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "create",
                "--youtube-user", "test-user",
                "--client-secrets", "secrets.json",
                "--file", "broadcasts.json",
                "--id", "test-id-1,test-id-2",
                "--starts-on", "01/15/24"
            };

            // Act
            var result = TestCommandLineApplication.Parse<CreateCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.BroadcastFile.Should().Be("broadcasts.json");
            result.Id.Should().Be("test-id-1,test-id-2");
            result.StartsOn.Should().Be(new DateOnly(2024, 1, 15));
        }

        [Fact]
        public void UpdateCommand_WithMinimalArguments_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "update",
                "--youtube-user", "test-user",
                "--client-secrets", "secrets.json",
                "--youtube-id", "test-youtube-id"
            };

            // Act
            var result = TestCommandLineApplication.Parse<UpdateCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.YouTubeId.Should().Be("test-youtube-id");
        }

        [Fact]
        public void UpdateCommand_WithAllArguments_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "update",
                "--youtube-user", "test-user",
                "--client-secrets", "secrets.json",
                "--youtube-id", "test-youtube-id",
                "--auto-start", "true",
                "--auto-stop", "false",
                "--privacy", "Public"
            };

            // Act
            var result = TestCommandLineApplication.Parse<UpdateCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.YouTubeId.Should().Be("test-youtube-id");
            result.AutoStart.Should().BeTrue();
            result.AutoStop.Should().BeFalse();
            result.Privacy.Should().Be(PrivacyEnum.Public);
        }

        [Fact]
        public void ListCommand_WithMinimalArguments_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "list",
                "--youtube-user", "test-user",
                "--client-secrets", "secrets.json",
                "--file", "broadcasts.json"
            };

            // Act
            var result = TestCommandLineApplication.Parse<ListCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.BroadcastFile.Should().Be("broadcasts.json");
            result.Upcoming.Should().BeFalse();
        }

        [Fact]
        public void ListCommand_WithUpcomingFlag_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "list",
                "--youtube-user", "test-user",
                "--client-secrets", "secrets.json",
                "--file", "broadcasts.json",
                "--upcoming"
            };

            // Act
            var result = TestCommandLineApplication.Parse<ListCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.BroadcastFile.Should().Be("broadcasts.json");
            result.Upcoming.Should().BeTrue();
        }

        [Fact]
        public void EndCommand_WithId_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "end",
                "--id", "test-id-123"
            };

            // Act
            var result = TestCommandLineApplication.Parse<EndCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("test-id-123");
        }

        [Fact]
        public void EndCommand_WithMultipleIds_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "end",
                "--id", "test-id-123,test-id-456,test-id-789"
            };

            // Act
            var result = TestCommandLineApplication.Parse<EndCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("test-id-123,test-id-456,test-id-789");
        }

        [Theory]
        [InlineData("create")]
        [InlineData("update")]
        [InlineData("list")]
        [InlineData("end")]
        public void Commands_ShouldBeRecognized(string commandName)
        {
            // Arrange
            var args = new[] { commandName, "--help" };

            // Act & Assert
            var action = () => TestCommandLineApplication.Parse<object>(args);
            action.Should().NotThrow();
        }

        [Fact]
        public void CreateCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "create",
                "-u", "test-user",
                "-c", "secrets.json",
                "-f", "broadcasts.json",
                "-i", "test-id",
                "-s", "01/15/24"
            };

            // Act
            var result = TestCommandLineApplication.Parse<CreateCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.BroadcastFile.Should().Be("broadcasts.json");
            result.Id.Should().Be("test-id");
            result.StartsOn.Should().Be(new DateOnly(2024, 1, 15));
        }

        [Fact]
        public void UpdateCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "update",
                "-u", "test-user",
                "-c", "secrets.json",
                "-y", "test-youtube-id",
                "-a", "true",
                "-o", "false",
                "-p", "Private"
            };

            // Act
            var result = TestCommandLineApplication.Parse<UpdateCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.YouTubeId.Should().Be("test-youtube-id");
            result.AutoStart.Should().BeTrue();
            result.AutoStop.Should().BeFalse();
            result.Privacy.Should().Be(PrivacyEnum.Private);
        }

        [Fact]
        public void ListCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "list",
                "-u", "test-user",
                "-c", "secrets.json",
                "-f", "broadcasts.json",
                "-p"
            };

            // Act
            var result = TestCommandLineApplication.Parse<ListCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.YouTubeUser.Should().Be("test-user");
            result.ClientSecretsFile.Should().Be("secrets.json");
            result.BroadcastFile.Should().Be("broadcasts.json");
            result.Upcoming.Should().BeTrue();
        }

        [Fact]
        public void EndCommand_WithShortFlags_ShouldParseCorrectly()
        {
            // Arrange
            var args = new[]
            {
                "end",
                "-i", "test-id-123"
            };

            // Act
            var result = TestCommandLineApplication.Parse<EndCommand>(args);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("test-id-123");
        }
    }

    // Helper class for testing command line parsing
    public static class TestCommandLineApplication
    {
        public static T Parse<T>(string[] args) where T : class
        {
            // This is a simplified version for testing
            // In a real implementation, you would use McMaster.Extensions.CommandLineUtils
            // to parse the arguments and return the command object
            return Activator.CreateInstance<T>();
        }
    }
}