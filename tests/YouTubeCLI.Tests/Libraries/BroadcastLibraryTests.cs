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
    }
}
