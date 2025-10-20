using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Libraries
{
    public class YouTubeLibraryTests
    {
        [Fact]
        public async Task BuildBroadCast_WithPastStartDate_ShouldThrowArgumentException()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };
            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

            // Act
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast, 
                occurrences: 1, 
                thumbnailDirectory: "/tmp", 
                startsOn: pastDate, 
                testMode: true);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Start date '{pastDate:MM/dd/yyyy}' cannot be before today's date*")
                .Where(ex => ex.ParamName == "startsOn");
        }

        [Fact]
        public async Task BuildBroadCast_WithYesterdayStartDate_ShouldThrowArgumentException()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };
            var yesterday = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

            // Act
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast, 
                occurrences: 1, 
                thumbnailDirectory: "/tmp", 
                startsOn: yesterday, 
                testMode: true);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("startsOn");
        }

        [Fact]
        public async Task BuildBroadCast_WithDateOneWeekAgo_ShouldThrowArgumentException()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };
            var oneWeekAgo = DateOnly.FromDateTime(DateTime.Now).AddDays(-7);

            // Act
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast, 
                occurrences: 1, 
                thumbnailDirectory: "/tmp", 
                startsOn: oneWeekAgo, 
                testMode: true);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("startsOn");
        }

        [Fact]
        public void BuildBroadCast_WithNullStartDate_ShouldNotThrowException()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast, 
                occurrences: 1, 
                thumbnailDirectory: "/tmp", 
                startsOn: null, 
                testMode: true);

            // Assert - this will eventually fail with a different error (no credentials)
            // but it should NOT throw ArgumentException about past date
            act.Should().NotThrowAsync<ArgumentException>("startsOn should be optional and default to today");
        }

        [Fact]
        public void BuildBroadCast_WithTodayStartDate_ShouldNotThrowArgumentException()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Act
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast, 
                occurrences: 1, 
                thumbnailDirectory: "/tmp", 
                startsOn: today, 
                testMode: true);

            // Assert - this will eventually fail with a different error (no credentials)
            // but it should NOT throw ArgumentException about past date
            act.Should().NotThrowAsync<ArgumentException>("today's date should be valid");
        }

        [Fact]
        public void BuildBroadCast_WithFutureStartDate_ShouldNotThrowArgumentException()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 1,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            // Act
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast, 
                occurrences: 1, 
                thumbnailDirectory: "/tmp", 
                startsOn: futureDate, 
                testMode: true);

            // Assert - this will eventually fail with a different error (no credentials)
            // but it should NOT throw ArgumentException about past date
            act.Should().NotThrowAsync<ArgumentException>("future dates should be valid");
        }
    }
}
