using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class CommandLineParsingTests
    {
        [Fact]
        public void CreateCommand_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var commandType = typeof(CreateCommand);
            var commandAttribute = commandType.GetCustomAttribute<McMaster.Extensions.CommandLineUtils.CommandAttribute>();

            // Assert
            commandAttribute.Should().NotBeNull();
            commandAttribute!.Description.Should().Be("Create YouTube Broadcasts");
        }

        [Fact]
        public void UpdateCommand_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var commandType = typeof(UpdateCommand);
            var commandAttribute = commandType.GetCustomAttribute<McMaster.Extensions.CommandLineUtils.CommandAttribute>();

            // Assert
            commandAttribute.Should().NotBeNull();
            commandAttribute!.Description.Should().Be("Update Broadcast");
        }

        [Fact]
        public void ListCommand_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var commandType = typeof(ListCommand);
            var commandAttribute = commandType.GetCustomAttribute<McMaster.Extensions.CommandLineUtils.CommandAttribute>();

            // Assert
            commandAttribute.Should().NotBeNull();
            commandAttribute!.Description.Should().Be("List YouTube Broadcasts");
        }

        [Fact]
        public void EndCommand_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act
            var commandType = typeof(EndCommand);
            var commandAttribute = commandType.GetCustomAttribute<McMaster.Extensions.CommandLineUtils.CommandAttribute>();

            // Assert
            commandAttribute.Should().NotBeNull();
            commandAttribute!.Description.Should().Be("End YouTube Broadcasts");
        }

        [Fact]
        public void CreateCommand_ShouldHaveRequiredOptions()
        {
            // Arrange
            var commandType = typeof(CreateCommand);
            var properties = commandType.GetProperties();

            // Act & Assert
            var youtubeUserProperty = commandType.GetProperty(nameof(CreateCommand.YouTubeUser));
            var clientSecretsProperty = commandType.GetProperty(nameof(CreateCommand.ClientSecretsFile));
            var broadcastFileProperty = commandType.GetProperty(nameof(CreateCommand.BroadcastFile));

            youtubeUserProperty.Should().NotBeNull();
            clientSecretsProperty.Should().NotBeNull();
            broadcastFileProperty.Should().NotBeNull();

            // Check for Required attributes
            youtubeUserProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
            clientSecretsProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
            broadcastFileProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
        }

        [Fact]
        public void UpdateCommand_ShouldHaveRequiredOptions()
        {
            // Arrange
            var commandType = typeof(UpdateCommand);
            var properties = commandType.GetProperties();

            // Act & Assert
            var youtubeUserProperty = commandType.GetProperty(nameof(UpdateCommand.YouTubeUser));
            var clientSecretsProperty = commandType.GetProperty(nameof(UpdateCommand.ClientSecretsFile));

            youtubeUserProperty.Should().NotBeNull();
            clientSecretsProperty.Should().NotBeNull();

            // Check for Required attributes
            youtubeUserProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
            clientSecretsProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
        }

        [Fact]
        public void ListCommand_ShouldHaveRequiredOptions()
        {
            // Arrange
            var commandType = typeof(ListCommand);
            var properties = commandType.GetProperties();

            // Act & Assert
            var youtubeUserProperty = commandType.GetProperty(nameof(ListCommand.YouTubeUser));
            var clientSecretsProperty = commandType.GetProperty(nameof(ListCommand.ClientSecretsFile));
            var broadcastFileProperty = commandType.GetProperty(nameof(ListCommand.BroadcastFile));

            youtubeUserProperty.Should().NotBeNull();
            clientSecretsProperty.Should().NotBeNull();
            broadcastFileProperty.Should().NotBeNull();

            // Check for Required attributes
            youtubeUserProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
            clientSecretsProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
            broadcastFileProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
        }

        [Fact]
        public void EndCommand_ShouldHaveRequiredOptions()
        {
            // Arrange
            var commandType = typeof(EndCommand);
            var properties = commandType.GetProperties();

            // Act & Assert
            var idProperty = commandType.GetProperty(nameof(EndCommand.Id));

            idProperty.Should().NotBeNull();

            // Check for Required attributes
            idProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().NotBeNull();
        }

        [Fact]
        public void CreateCommand_ShouldHaveOptionalOptions()
        {
            // Arrange
            var commandType = typeof(CreateCommand);

            // Act & Assert
            var idProperty = commandType.GetProperty(nameof(CreateCommand.Id));
            var startsOnProperty = commandType.GetProperty(nameof(CreateCommand.StartsOn));

            idProperty.Should().NotBeNull();
            startsOnProperty.Should().NotBeNull();

            // These should not have Required attributes
            idProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
            startsOnProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
        }

        [Fact]
        public void UpdateCommand_ShouldHaveOptionalOptions()
        {
            // Arrange
            var commandType = typeof(UpdateCommand);

            // Act & Assert
            var broadcastFileProperty = commandType.GetProperty(nameof(UpdateCommand.BroadcastFile));
            var youtubeIdProperty = commandType.GetProperty(nameof(UpdateCommand.YouTubeId));
            var autoStartProperty = commandType.GetProperty(nameof(UpdateCommand.AutoStart));
            var autoStopProperty = commandType.GetProperty(nameof(UpdateCommand.AutoStop));
            var privacyProperty = commandType.GetProperty(nameof(UpdateCommand.Privacy));

            broadcastFileProperty.Should().NotBeNull();
            youtubeIdProperty.Should().NotBeNull();
            autoStartProperty.Should().NotBeNull();
            autoStopProperty.Should().NotBeNull();
            privacyProperty.Should().NotBeNull();

            // These should not have Required attributes
            broadcastFileProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
            youtubeIdProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
            autoStartProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
            autoStopProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
            privacyProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
        }

        [Fact]
        public void ListCommand_ShouldHaveOptionalOptions()
        {
            // Arrange
            var commandType = typeof(ListCommand);

            // Act & Assert
            var filterProperty = commandType.GetProperty(nameof(ListCommand.FilterString));
            var limitProperty = commandType.GetProperty(nameof(ListCommand.Limit));

            filterProperty.Should().NotBeNull();
            limitProperty.Should().NotBeNull();

            // These should not have Required attribute
            filterProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
            limitProperty!.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>().Should().BeNull();
        }
    }
}
