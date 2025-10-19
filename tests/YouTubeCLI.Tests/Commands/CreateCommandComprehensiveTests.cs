using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class CreateCommandComprehensiveTests
    {
        [Fact]
        public void CreateArgs_WithAllRequiredArguments_ShouldIncludeAllRequiredArgs()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("file");
            args.Should().Contain("broadcasts.json");
            args.Should().Contain("client-secrets");
            args.Should().Contain("secrets.json");
        }

        [Theory]
        [InlineData("single-id")]
        [InlineData("id1,id2")]
        [InlineData("id1,id2,id3")]
        [InlineData("1234567890")]
        [InlineData("abc-def-ghi")]
        public void CreateArgs_WithVariousIdFormats_ShouldIncludeIdArgument(string id)
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = id
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain(id);
        }

        [Theory]
        [InlineData("01/01/24")]
        [InlineData("12/31/23")]
        [InlineData("06/15/24")]
        public void CreateArgs_WithVariousDateFormats_ShouldIncludeStartsOnArgument(string dateString)
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                StartsOn = DateOnly.Parse(dateString)
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("starts-on");
            args.Should().Contain(dateString);
        }

        [Fact]
        public void CreateArgs_WithAllOptionalArguments_ShouldIncludeAllArguments()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = "test-id-1,test-id-2",
                StartsOn = new DateOnly(2024, 1, 15)
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain("test-id-1,test-id-2");
            args.Should().Contain("starts-on");
            args.Should().Contain("01/15/24");
        }

        [Fact]
        public void CreateArgs_WithNoOptionalArguments_ShouldNotIncludeOptionalArgs()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
            args.Should().NotContain("starts-on");
        }

        [Fact]
        public void CreateArgs_WithNullOptionalArguments_ShouldNotIncludeThem()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = null,
                StartsOn = null
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
            args.Should().NotContain("starts-on");
        }

        [Fact]
        public void CreateArgs_WithEmptyOptionalArguments_ShouldNotIncludeThem()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = ""
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void CreateArgs_WithWhitespaceId_ShouldNotIncludeIdArgument(string id)
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = id
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
        }

        [Fact]
        public void CreateArgs_WithFutureDate_ShouldHandleCorrectly()
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
        public void CreateArgs_WithPastDate_ShouldHandleCorrectly()
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
    }
}