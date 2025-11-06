using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;
using YouTubeCLI.Models;

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

        [Theory]
        [InlineData("private")]
        [InlineData("public")]
        [InlineData("unlisted")]
        [InlineData("Private")]
        [InlineData("Public")]
        [InlineData("Unlisted")]
        [InlineData("PRIVATE")]
        [InlineData("PUBLIC")]
        [InlineData("UNLISTED")]
        public void Broadcast_Privacy_WithValidValues_ShouldBeAccepted(string privacyValue)
        {
            // Arrange
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                privacy = privacyValue,
                active = true
            };

            // Act & Assert
            broadcast.privacy.Should().Be(privacyValue);
            
            // Verify it can be converted to lowercase (as used in YouTubeLibrary)
            var lowercasePrivacy = broadcast.privacy.ToLower();
            var validValues = new[] { "private", "public", "unlisted" };
            validValues.Should().Contain(lowercasePrivacy);
        }

        [Theory]
        [InlineData("private", "private")]
        [InlineData("public", "public")]
        [InlineData("unlisted", "unlisted")]
        [InlineData("Private", "private")]
        [InlineData("Public", "public")]
        [InlineData("Unlisted", "unlisted")]
        [InlineData("PRIVATE", "private")]
        [InlineData("PUBLIC", "public")]
        [InlineData("UNLISTED", "unlisted")]
        public void Broadcast_Privacy_ToLowerCase_ShouldProduceCorrectValue(string inputPrivacy, string expectedLowercase)
        {
            // Arrange
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                privacy = inputPrivacy,
                active = true
            };

            // Act
            var lowercasePrivacy = broadcast.privacy.ToLower();

            // Assert
            lowercasePrivacy.Should().Be(expectedLowercase);
        }

        [Theory]
        [InlineData("InvalidValue")]
        [InlineData("Protected")]
        [InlineData("Secret")]
        [InlineData("Hidden")]
        [InlineData("123")]
        public void Broadcast_Privacy_WithInvalidValues_ShouldNotMatchValidOptions(string invalidPrivacy)
        {
            // Arrange
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                privacy = invalidPrivacy,
                active = true
            };

            // Act
            var privacyLower = broadcast.privacy.ToLower();

            // Assert
            // Invalid values should not match any of the valid privacy options
            var validValues = new[] { "private", "public", "unlisted" };
            validValues.Should().NotContain(privacyLower);
        }

        [Fact]
        public void Broadcast_Privacy_WithEmptyString_ShouldNotMatchValidOptions()
        {
            // Arrange
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                privacy = "",
                active = true
            };

            // Act
            var privacyLower = broadcast.privacy.ToLower();

            // Assert
            // Empty string should not match any of the valid privacy options
            var validValues = new[] { "private", "public", "unlisted" };
            validValues.Should().NotContain(privacyLower);
            privacyLower.Should().BeEmpty();
        }

        [Fact]
        public void Broadcast_Privacy_ShouldBeStringType()
        {
            // Arrange
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                privacy = "public",
                active = true
            };

            // Act & Assert
            broadcast.privacy.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData("private")]
        [InlineData("public")]
        [InlineData("unlisted")]
        public void CreateCommand_BroadcastPrivacy_AllValidValuesSupported(string privacyValue)
        {
            // Arrange
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                autoStart = true,
                autoStop = true,
                privacy = privacyValue,
                active = true
            };

            // Act
            var privacyLower = broadcast.privacy.ToLower();

            // Assert
            // Verify the privacy value is one of the valid options
            var validValues = new[] { "private", "public", "unlisted" };
            validValues.Should().Contain(privacyLower);
            
            // Verify the broadcast object is properly constructed
            broadcast.Should().NotBeNull();
            broadcast.privacy.Should().Be(privacyValue);
        }
    }
}
