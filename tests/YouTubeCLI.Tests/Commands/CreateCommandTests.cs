using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class CreateCommandTests
    {
        [Fact]
        public void CreateArgs_ShouldIncludeRequiredArguments()
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

        [Fact]
        public void CreateArgs_WithId_ShouldIncludeIdArgument()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                Id = "test-id-1,test-id-2"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain("test-id-1,test-id-2");
        }

        [Fact]
        public void CreateArgs_WithStartsOn_ShouldIncludeStartsOnArgument()
        {
            // Arrange
            var command = new CreateCommand
            {
                YouTubeUser = "test-user",
                ClientSecretsFile = "secrets.json",
                BroadcastFile = "broadcasts.json",
                StartsOn = new DateOnly(2024, 1, 15)
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("starts-on");
            args.Should().Contain("01/15/24");
        }

        [Fact]
        public void CreateArgs_WithoutOptionalArguments_ShouldNotIncludeThem()
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
    }
}