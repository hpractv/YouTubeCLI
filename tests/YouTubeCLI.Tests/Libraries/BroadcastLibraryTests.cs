using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using Xunit;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Libraries
{
    public class BroadcastLibraryTests
    {
        [Fact]
        public void GetBroadcasts_WithValidJsonFile_ShouldReturnBroadcasts()
        {
            // Arrange
            var testJson = """
            {
                "broadcasts": [
                    {
                        "id": "test-broadcast-1",
                        "name": "Test Broadcast",
                        "active": true,
                        "dayOfWeek": 1,
                        "broadcastStart": "10:00 AM",
                        "broadcastDurationInMinutes": 60,
                        "privacy": "private",
                        "autoStart": true,
                        "autoStop": true,
                        "stream": "test-stream",
                        "thumbnail": "test-thumbnail.png"
                    }
                ]
            }
            """;

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testJson);

            try
            {
                // Act
                var result = BroadcastLibrary.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(1);
                result.Items.First().id.Should().Be("test-broadcast-1");
                result.Items.First().name.Should().Be("Test Broadcast");
                result.Items.First().active.Should().BeTrue();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithInvalidJsonFile_ShouldThrowException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, invalidJson);

            try
            {
                // Act & Assert
                var action = () => BroadcastLibrary.GetBroadcasts(tempFile);
                action.Should().Throw<Exception>();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithNonExistentFile_ShouldThrowException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(Path.GetTempPath(), "non-existent-file.json");

            // Act & Assert
            var action = () => BroadcastLibrary.GetBroadcasts(nonExistentFile);
            action.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void GetBroadcasts_WithEmptyJsonFile_ShouldReturnEmptyBroadcasts()
        {
            // Arrange
            var emptyJson = "{}";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, emptyJson);

            try
            {
                // Act
                var result = BroadcastLibrary.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().BeEmpty();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithEmptyBroadcastsArray_ShouldReturnEmptyBroadcasts()
        {
            // Arrange
            var emptyArrayJson = "{\"broadcasts\": []}";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, emptyArrayJson);

            try
            {
                // Act
                var result = BroadcastLibrary.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().BeEmpty();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithMalformedJson_ShouldThrowException()
        {
            // Arrange
            var malformedJson = "{\"broadcasts\": [ invalid }";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, malformedJson);

            try
            {
                // Act & Assert
                var action = () => BroadcastLibrary.GetBroadcasts(tempFile);
                action.Should().Throw<Exception>();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithMultipleBroadcasts_ShouldReturnAll()
        {
            // Arrange
            var testJson = """
            {
                "broadcasts": [
                    {
                        "id": "test-id-1",
                        "name": "Test Broadcast 1",
                        "active": true,
                        "dayOfWeek": 1,
                        "broadcastStart": "10:00 AM",
                        "broadcastDurationInMinutes": 60,
                        "stream": "test-stream",
                        "privacy": "private",
                        "autoStart": true,
                        "autoStop": true,
                        "chatEnabled": true
                    },
                    {
                        "id": "test-id-2",
                        "name": "Test Broadcast 2",
                        "active": false,
                        "dayOfWeek": 2,
                        "broadcastStart": "11:00 AM",
                        "broadcastDurationInMinutes": 90,
                        "stream": "test-stream-2",
                        "privacy": "public",
                        "autoStart": false,
                        "autoStop": false,
                        "chatEnabled": false
                    },
                    {
                        "id": "test-id-3",
                        "name": "Test Broadcast 3",
                        "active": true,
                        "dayOfWeek": 3,
                        "broadcastStart": "12:00 PM",
                        "broadcastDurationInMinutes": 120,
                        "stream": "test-stream-3",
                        "privacy": "unlisted",
                        "autoStart": true,
                        "autoStop": true,
                        "chatEnabled": true
                    }
                ]
            }
            """;

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testJson);

            try
            {
                // Act
                var result = BroadcastLibrary.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(3);
                result.Items.First().id.Should().Be("test-id-1");
                result.Items.Last().id.Should().Be("test-id-3");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithMissingOptionalFields_ShouldStillParse()
        {
            // Arrange
            var testJson = """
            {
                "broadcasts": [
                    {
                        "id": "test-id",
                        "name": "Test Broadcast",
                        "active": true,
                        "dayOfWeek": 1,
                        "broadcastStart": "10:00 AM",
                        "broadcastDurationInMinutes": 60,
                        "stream": "test-stream",
                        "privacy": "private"
                    }
                ]
            }
            """;

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testJson);

            try
            {
                // Act
                var result = BroadcastLibrary.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(1);
                var broadcast = result.Items.First();
                broadcast.id.Should().Be("test-id");
                broadcast.name.Should().Be("Test Broadcast");
                // Optional fields should have default values
                broadcast.autoStart.Should().BeFalse();
                broadcast.autoStop.Should().BeFalse();
                broadcast.chatEnabled.Should().BeFalse();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
