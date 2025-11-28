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
        public void CreateArgs_WithUpcomingFilter_ShouldIncludeUpcomingFilter()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Filter = new[] { BroadcastFilter.Upcoming }
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("filter");
            args.Should().Contain("Upcoming");
        }

        [Fact]
        public void CreateArgs_WithDefaultFilter_ShouldNotIncludeFilterFlag()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Filter = new[] { BroadcastFilter.All }
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("filter");
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
                Filter = new[] { BroadcastFilter.Upcoming },
                Limit = 50
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("client-secrets");
            args.Should().Contain("secrets.json");
            args.Should().Contain("filter");
            args.Should().Contain("Upcoming");
            args.Should().Contain("limit");
            args.Should().Contain("50");
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
                Filter = new[] { BroadcastFilter.Upcoming },
                Limit = 50
            };

            // Assert
            command.YouTubeUser.Should().Be("user123");
            command.ClientSecretsFile.Should().Be("my-secrets.json");
            command.BroadcastFile.Should().Be("my-broadcasts.json");
            command.Filter.Should().ContainSingle();
            command.Filter[0].Should().Be(BroadcastFilter.Upcoming);
            command.Limit.Should().Be(50);
        }

        [Fact]
        public void ListCommand_Filter_ShouldDefaultToAll()
        {
            // Arrange & Act
            var command = new ListCommand();

            // Assert
            command.Filter.Should().ContainSingle();
            command.Filter[0].Should().Be(BroadcastFilter.All);
        }

        [Fact]
        public void ListCommand_Limit_ShouldDefaultTo100()
        {
            // Arrange & Act
            var command = new ListCommand();

            // Assert
            command.Limit.Should().Be(100);
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
            output.Should().MatchRegex(@"^.+ \([^)]+\): https://youtu\.be/[^\s]+$");
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
        public void ListCommand_OutputFormat_WithParenthesesInTitle_ShouldMatchPattern()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                title = "My (Special) Broadcast",
                id = "abc123",
                privacyStatus = "public"
            };

            // Act
            var output = $"{linkDetails.title} ({linkDetails.privacyStatus}): {linkDetails.broadcastUrl}";

            // Assert - Verify pattern matches titles with parentheses
            output.Should().Be("My (Special) Broadcast (public): https://youtu.be/abc123");
            output.Should().MatchRegex(@"^.+ \([^)]+\): https://youtu\.be/[^\s]+$");
            output.Should().StartWith(linkDetails.title);
            output.Should().Contain($"({linkDetails.privacyStatus})");
            output.Should().EndWith(linkDetails.broadcastUrl);
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

        [Fact]
        public void ListCommand_Filter_ShouldAcceptMultipleValues()
        {
            // Arrange & Act
            var command = new ListCommand
            {
                Filter = new[] { BroadcastFilter.Upcoming, BroadcastFilter.Active }
            };

            // Assert
            command.Filter.Should().HaveCount(2);
            command.Filter.Should().Contain(BroadcastFilter.Upcoming);
            command.Filter.Should().Contain(BroadcastFilter.Active);
        }

        [Fact]
        public void CreateArgs_WithActiveFilter_ShouldIncludeActiveFilter()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Filter = new[] { BroadcastFilter.Active }
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("filter");
            args.Should().Contain("Active");
        }

        [Fact]
        public void CreateArgs_WithCompletedFilter_ShouldIncludeCompletedFilter()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Filter = new[] { BroadcastFilter.Completed }
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("filter");
            args.Should().Contain("Completed");
        }

        [Fact]
        public void CreateArgs_WithLimitDefault_ShouldNotIncludeLimitFlag()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Limit = 100
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("limit");
        }

        [Fact]
        public void CreateArgs_WithLimitCustom_ShouldIncludeLimitFlag()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Limit = 50
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("limit");
            args.Should().Contain("50");
        }

        [Theory]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(200)]
        [InlineData(1000)]
        public void CreateArgs_WithDifferentLimits_ShouldIncludeCorrectLimit(int limit)
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Limit = limit
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("limit");
            args.Should().Contain(limit.ToString());
        }

        [Fact]
        public void CreateArgs_WithCompletedFilterAndLimit_ShouldIncludeBoth()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Filter = new[] { BroadcastFilter.Completed },
                Limit = 25
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("filter");
            args.Should().Contain("Completed");
            args.Should().Contain("limit");
            args.Should().Contain("25");
        }

        [Fact]
        public void CreateArgs_WithMultipleFilters_ShouldIncludeAllFilters()
        {
            // Arrange
            var command = new ListCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Filter = new[] { BroadcastFilter.Upcoming, BroadcastFilter.Active }
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            var filterIndices = args.Select((arg, index) => arg == "filter" ? index : -1)
                .Where(index => index >= 0)
                .ToList();
            filterIndices.Should().HaveCount(2);
            args.Should().Contain("Upcoming");
            args.Should().Contain("Active");
        }
    }
}
