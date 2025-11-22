using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Commands
{
    public class ListCommandTests
    {
        private static System.Reflection.PropertyInfo GetClearCredentialProperty()
        {
            var property = typeof(CommandsBase).GetProperty("ClearCredential",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            property.Should().NotBeNull();
            return property!;
        }

        private static void SetClearCredential(CommandsBase command, bool value)
        {
            GetClearCredentialProperty().SetValue(command, value);
        }

        private static bool GetClearCredential(CommandsBase command)
        {
            return (bool)GetClearCredentialProperty().GetValue(command)!;
        }


        [Fact]
        public void CreateArgs_WithRequiredArguments_ShouldIncludeAllArguments()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("client-secrets");
            args.Should().Contain("secrets.json");
        }

        [Fact]
        public void CreateArgs_WithUpcomingTrue_ShouldIncludeUpcomingFlag()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Upcoming = true
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("upcoming");
        }

        [Fact]
        public void CreateArgs_WithUpcomingFalse_ShouldNotIncludeUpcomingFlag()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Upcoming = false
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("upcoming");
        }

        [Fact]
        public void CreateArgs_WithNullClientSecrets_ShouldNotIncludeClientSecrets()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = null,
                BroadcastFile = "broadcasts.json"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("client-secrets");
        }

        [Fact]
        public void CreateArgs_WithEmptyClientSecrets_ShouldNotIncludeClientSecrets()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "",
                BroadcastFile = "broadcasts.json"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("client-secrets");
        }

        [Fact]
        public void CreateArgs_WithAllFlags_ShouldIncludeAllRelevantArguments()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Upcoming = true
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("client-secrets");
            args.Should().Contain("secrets.json");
            args.Should().Contain("upcoming");
        }

        [Fact]
        public void ListCommand_Properties_ShouldBeSettable()
        {
            // Arrange & Act
            var command = new ListCommand
            {
                YouTubeUser = "user123",
                ClientSecretsFile = "my-secrets.json",
                BroadcastFile = "my-broadcasts.json",
                Upcoming = true
            };

            // Assert
            command.YouTubeUser.Should().Be("user123");
            command.ClientSecretsFile.Should().Be("my-secrets.json");
            command.BroadcastFile.Should().Be("my-broadcasts.json");
            command.Upcoming.Should().BeTrue();
        }

        [Fact]
        public void ListCommand_Upcoming_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var command = new ListCommand();

            // Assert
            command.Upcoming.Should().BeFalse();
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void CreateArgs_WithWhitespaceClientSecrets_ShouldNotIncludeClientSecrets(string whitespace)
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = whitespace,
                BroadcastFile = "broadcasts.json"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("client-secrets");
        }

        [Fact]
        public void ListCommand_OutputFormat_ShouldIncludePrivacyStatus_Public()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                title = "Test Broadcast",
                id = "abc123",
                privacyStatus = "public"
            };

            // Act
            var expectedOutput = $"{linkDetails.title} ({linkDetails.privacyStatus}): {linkDetails.broadcastUrl}";

            // Assert
            expectedOutput.Should().Be("Test Broadcast (public): https://youtu.be/abc123");
            expectedOutput.Should().Contain("(public)");
        }

        [Fact]
        public void ListCommand_OutputFormat_ShouldIncludePrivacyStatus_Private()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                title = "Private Stream",
                id = "xyz789",
                privacyStatus = "private"
            };

            // Act
            var expectedOutput = $"{linkDetails.title} ({linkDetails.privacyStatus}): {linkDetails.broadcastUrl}";

            // Assert
            expectedOutput.Should().Be("Private Stream (private): https://youtu.be/xyz789");
            expectedOutput.Should().Contain("(private)");
        }

        [Fact]
        public void ListCommand_OutputFormat_ShouldIncludePrivacyStatus_Unlisted()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                title = "Unlisted Video",
                id = "def456",
                privacyStatus = "unlisted"
            };

            // Act
            var expectedOutput = $"{linkDetails.title} ({linkDetails.privacyStatus}): {linkDetails.broadcastUrl}";

            // Assert
            expectedOutput.Should().Be("Unlisted Video (unlisted): https://youtu.be/def456");
            expectedOutput.Should().Contain("(unlisted)");
        }

        [Theory]
        [InlineData("public", "Test Public", "pub123")]
        [InlineData("private", "Test Private", "prv456")]
        [InlineData("unlisted", "Test Unlisted", "unl789")]
        public void ListCommand_OutputFormat_ShouldIncludePrivacyStatus_MultipleStates(
            string privacyStatus, string title, string id)
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                title = title,
                id = id,
                privacyStatus = privacyStatus
            };

            // Act
            var expectedOutput = $"{linkDetails.title} ({linkDetails.privacyStatus}): {linkDetails.broadcastUrl}";

            // Assert
            expectedOutput.Should().Contain($"({privacyStatus})");
            expectedOutput.Should().Contain($"https://youtu.be/{id}");
            expectedOutput.Should().Contain(title);
        }

        [Fact]
        public void ListCommand_OutputFormat_ShouldMatchExpectedPattern()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                title = "My Broadcast",
                id = "test123",
                privacyStatus = "public"
            };

            // Act
            var output = $"{linkDetails.title} ({linkDetails.privacyStatus}): {linkDetails.broadcastUrl}";

            // Assert - Verify the format matches: "Title (privacyStatus): URL"
            output.Should().MatchRegex(@"^[^(]+\([^)]+\): https://youtu\.be/[^\s]+$");
            output.Should().StartWith(linkDetails.title);
            output.Should().Contain($"({linkDetails.privacyStatus})");
            output.Should().EndWith(linkDetails.broadcastUrl);
        }

        [Fact]
        public void ListCommand_OutputFormat_WithMultipleBroadcasts_ShouldIncludePrivacyStatusForEach()
        {
            // Arrange
            var broadcasts = new List<LinkDetails>
            {
                new LinkDetails { title = "Stream 1", id = "id1", privacyStatus = "public" },
                new LinkDetails { title = "Stream 2", id = "id2", privacyStatus = "private" },
                new LinkDetails { title = "Stream 3", id = "id3", privacyStatus = "unlisted" }
            };

            // Act & Assert
            foreach (var broadcast in broadcasts)
            {
                var output = $"{broadcast.title} ({broadcast.privacyStatus}): {broadcast.broadcastUrl}";

                output.Should().Contain($"({broadcast.privacyStatus})");
                output.Should().Contain(broadcast.title);
                output.Should().Contain(broadcast.broadcastUrl);
            }
        }

        [Fact]
        public void CreateArgs_WithClearCredentialTrue_ShouldIncludeClearCredentialFlag()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json"
            };

            SetClearCredential(command, true);

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("clear-credential");
            args.Should().Contain("True");
        }

        [Fact]
        public void CreateArgs_WithClearCredentialFalse_ShouldIncludeClearCredentialAsFalse()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json"
            };

            SetClearCredential(command, false);

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("clear-credential");
            args.Should().Contain("False");
        }

        [Fact]
        public void ClearCredential_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var command = new ListCommand();
            var value = GetClearCredential(command);

            // Assert
            value.Should().BeFalse();
        }
    }
}
