using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class EdgeCaseTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void CreateCommand_WithWhitespaceArguments_ShouldHandleGracefully(string whitespace)
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = whitespace,
                StartsOn = null
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
            args.Should().NotContain("starts-on");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void UpdateCommand_WithWhitespaceArguments_ShouldHandleGracefully(string whitespace)
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = whitespace,
                BroadcastFile = whitespace
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("youtube-id");
            args.Should().NotContain("file");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void EndCommand_WithWhitespaceArguments_ShouldHandleGracefully(string whitespace)
        {
            // Arrange
            var command = new EndCommand
            {
                Id = whitespace
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
        }

        [Fact]
        public void CreateCommand_WithVeryLongId_ShouldHandleCorrectly()
        {
            // Arrange
            var longId = new string('a', 1000);
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = longId
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain(longId);
        }

        [Fact]
        public void CreateCommand_WithManyCommaSeparatedIds_ShouldHandleCorrectly()
        {
            // Arrange
            var manyIds = string.Join(",", Enumerable.Range(1, 100).Select(i => $"id-{i}"));
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = manyIds
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain(manyIds);
        }

        [Fact]
        public void CreateCommand_WithFutureDate_ShouldHandleCorrectly()
        {
            // Arrange
            var futureDate = new DateOnly(2030, 12, 31);
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                StartsOn = futureDate
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("starts-on");
            args.Should().Contain("12/31/30");
        }

        [Fact]
        public void CreateCommand_WithPastDate_ShouldHandleCorrectly()
        {
            // Arrange
            var pastDate = new DateOnly(2020, 1, 1);
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                StartsOn = pastDate
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("starts-on");
            args.Should().Contain("01/01/20");
        }

        [Fact]
        public void UpdateCommand_WithNullOptionalValues_ShouldNotIncludeThem()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-id",
                AutoStart = null,
                AutoStop = null,
                Privacy = null
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("auto-start");
            args.Should().NotContain("auto-stop");
            args.Should().NotContain("privacy");
        }

        [Fact]
        public void UpdateCommand_WithBothFileAndId_ShouldIncludeBoth()
        {
            // Arrange
            var command = new UpdateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                YouTubeId = "test-id",
                BroadcastFile = "broadcasts.csv"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("youtube-id");
            args.Should().Contain("test-id");
            args.Should().Contain("file");
            args.Should().Contain("broadcasts.csv");
        }

        [Fact]
        public void ListCommand_WithUpcomingFalse_ShouldNotIncludeUpcomingFlag()
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
        public void EndCommand_WithSpecialCharactersInId_ShouldHandleCorrectly()
        {
            // Arrange
            var specialId = "test-id-with-special-chars-!@#$%^&*()_+-=[]{}|;':\",./<>?";
            var command = new EndCommand
            {
                Id = specialId
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain(specialId);
        }

        [Fact]
        public void EndCommand_WithUnicodeCharactersInId_ShouldHandleCorrectly()
        {
            // Arrange
            var unicodeId = "test-id-with-unicode-🚀-🎉-测试";
            var command = new EndCommand
            {
                Id = unicodeId
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain(unicodeId);
        }
    }
}