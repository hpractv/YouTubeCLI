using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;
using YouTubeCLI.Libraries;

namespace YouTubeCLI.Tests.Commands
{
    public class UpdateCommandTests
    {
        [Fact]
        public void CreateArgs_WithAllRequiredArguments_ShouldIncludeAllArguments()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("youtube-user");
            args.Should().Contain("test-user");
            args.Should().Contain("client-secrets");
            args.Should().Contain("secrets.json");
            args.Should().Contain("youtube-id");
            args.Should().Contain("test-youtube-id");
        }

        [Fact]
        public void CreateArgs_WithBroadcastFile_ShouldIncludeFileArgument()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.csv"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("file");
            args.Should().Contain("broadcasts.csv");
        }

        [Fact]
        public void CreateArgs_WithAutoStartTrue_ShouldIncludeAutoStartArgument()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id",
                AutoStart = true
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("auto-start");
            args.Should().Contain("True");
        }

        [Fact]
        public void CreateArgs_WithAutoStartFalse_ShouldIncludeAutoStartArgument()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id",
                AutoStart = false
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("auto-start");
            args.Should().Contain("False");
        }

        [Fact]
        public void CreateArgs_WithAutoStopTrue_ShouldIncludeAutoStopArgument()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id",
                AutoStop = true
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("auto-stop");
            args.Should().Contain("True");
        }

        [Fact]
        public void CreateArgs_WithAutoStopFalse_ShouldIncludeAutoStopArgument()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id",
                AutoStop = false
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("auto-stop");
            args.Should().Contain("False");
        }

        [Theory]
        [InlineData(PrivacyEnum.Private)]
        [InlineData(PrivacyEnum.Public)]
        [InlineData(PrivacyEnum.Unlisted)]
        public void CreateArgs_WithPrivacyValues_ShouldIncludePrivacyArgument(PrivacyEnum privacy)
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id",
                Privacy = privacy
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("privacy");
            args.Should().Contain(privacy.ToString());
        }

        [Fact]
        public void CreateArgs_WithAllOptionalArguments_ShouldIncludeAllArguments()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id",
                AutoStart = true,
                AutoStop = false,
                Privacy = PrivacyEnum.Public
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("auto-start");
            args.Should().Contain("True");
            args.Should().Contain("auto-stop");
            args.Should().Contain("False");
            args.Should().Contain("privacy");
            args.Should().Contain("Public");
        }

        [Fact]
        public void CreateArgs_WithNullOptionalArguments_ShouldNotIncludeThem()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-youtube-id"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("auto-start");
            args.Should().NotContain("auto-stop");
            args.Should().NotContain("privacy");
            args.Should().NotContain("file");
        }

        [Fact]
        public void CreateArgs_WithEmptyStrings_ShouldNotIncludeThem()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "",
                BroadcastFile = ""
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("file");
            args.Should().NotContain("youtube-id");
        }

        [Fact]
        public void CreateArgs_WithWhitespaceStrings_ShouldNotIncludeThem()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "\t",
                BroadcastFile = "   "
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("file");
            args.Should().NotContain("youtube-id");
        }
    }
}
