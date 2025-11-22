using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class ListCommandTests
    {
        private static void SetClearCredential(CommandsBase command, bool value)
        {
            var clearCredentialProperty = typeof(CommandsBase).GetProperty("ClearCredential",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clearCredentialProperty.Should().NotBeNull();
            clearCredentialProperty!.SetValue(command, value);
        }

        private static bool GetClearCredential(CommandsBase command)
        {
            var clearCredentialProperty = typeof(CommandsBase).GetProperty("ClearCredential",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clearCredentialProperty.Should().NotBeNull();
            return (bool)clearCredentialProperty!.GetValue(command)!;
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
