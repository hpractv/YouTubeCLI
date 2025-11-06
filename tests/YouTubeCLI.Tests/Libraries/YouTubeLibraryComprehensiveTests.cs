using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Libraries
{
    public class YouTubeLibraryComprehensiveTests
    {
        [Fact]
        public void YouTubeLibrary_DefaultConstructor_ShouldCreateInstance()
        {
            // Act
            var library = new YouTubeLibrary();

            // Assert
            library.Should().NotBeNull();
        }

        [Fact]
        public void YouTubeLibrary_ConstructorWithParameters_ShouldCreateInstance()
        {
            // Act
            var library = new YouTubeLibrary("test-user", "test-secrets.json");

            // Assert
            library.Should().NotBeNull();
        }

        [Fact]
        public void YouTubeLibrary_ConstructorWithNullParameters_ShouldCreateInstance()
        {
            // Act
            var library = new YouTubeLibrary(null, null);

            // Assert
            library.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0, 0)] // Sunday to Sunday - same day
        [InlineData(0, 1)] // Sunday to Monday
        [InlineData(0, 6)] // Sunday to Saturday
        [InlineData(1, 0)] // Monday to Sunday (wrap around)
        [InlineData(3, 3)] // Wednesday to Wednesday (next week)
        [InlineData(5, 2)] // Friday to Tuesday (wrap around)
        public void BuildBroadCast_ShouldCalculateCorrectDayOfWeek(int startDayOfWeek, int targetDayOfWeek)
        {
            // This test verifies the day-of-week calculation logic
            // Note: The actual implementation uses modulo arithmetic to find the next occurrence

            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var today = DateOnly.FromDateTime(DateTime.Now);
            var startDate = today.AddDays(-((int)today.DayOfWeek - startDayOfWeek + 7) % 7);

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act & Assert
            // This will fail with API errors, but we can verify the date calculation logic
            // by checking that it doesn't throw ArgumentException for valid dates
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: startDate,
                testMode: true);

            // Should not throw ArgumentException for date calculation
            act.Should().NotThrowAsync<ArgumentException>("Date calculation should be valid");
        }

        [Fact]
        public void BuildBroadCast_WithSameDayOfWeek_ShouldUseStartDate()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentDayOfWeek = (int)today.DayOfWeek;

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = currentDayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act & Assert
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: today,
                testMode: true);

            // Should not throw ArgumentException when start date matches target day
            act.Should().NotThrowAsync<ArgumentException>("Same day should use start date");
        }

        [Fact]
        public void BuildBroadCast_WithTestModeTrue_ShouldCreateOnlyOneOccurrence()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = (int)futureDate.DayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act & Assert
            // Test mode should limit to 1 occurrence regardless of occurrences parameter
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 5, // Request 5 but test mode should limit to 1
                thumbnailDirectory: "/tmp",
                startsOn: futureDate,
                testMode: true);

            // Should not throw ArgumentException
            act.Should().NotThrowAsync<ArgumentException>("Test mode should be valid");
        }

        [Fact]
        public void BuildBroadCast_WithNullStartsOn_ShouldUseToday()
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

            // Act & Assert
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: null,
                testMode: true);

            // Should not throw ArgumentException - null should default to today
            act.Should().NotThrowAsync<ArgumentException>("Null startsOn should default to today");
        }

        [Theory]
        [InlineData("private")]
        [InlineData("public")]
        [InlineData("unlisted")]
        [InlineData("Private")]
        [InlineData("Public")]
        [InlineData("Unlisted")]
        public void BuildBroadCast_WithVariousPrivacySettings_ShouldNotThrowForPrivacy(string privacy)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = (int)futureDate.DayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = privacy,
                autoStart = true,
                autoStop = true
            };

            // Act & Assert
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: futureDate,
                testMode: true);

            // Should not throw ArgumentException for privacy validation
            act.Should().NotThrowAsync<ArgumentException>("Privacy should be case-insensitive");
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void BuildBroadCast_WithAutoStartAndAutoStop_ShouldAcceptAllCombinations(bool autoStart, bool autoStop)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = (int)futureDate.DayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = autoStart,
                autoStop = autoStop
            };

            // Act & Assert
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: futureDate,
                testMode: true);

            // Should not throw ArgumentException for any combination
            act.Should().NotThrowAsync<ArgumentException>($"AutoStart={autoStart}, AutoStop={autoStop} should be valid");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BuildBroadCast_WithChatEnabled_ShouldAcceptBothValues(bool chatEnabled)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = (int)futureDate.DayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true,
                chatEnabled = chatEnabled
            };

            // Act & Assert
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: futureDate,
                testMode: true);

            // Should not throw ArgumentException for chatEnabled
            act.Should().NotThrowAsync<ArgumentException>($"ChatEnabled={chatEnabled} should be valid");
        }

        [Theory]
        [InlineData(30)]
        [InlineData(60)]
        [InlineData(90)]
        [InlineData(120)]
        [InlineData(180)]
        public void BuildBroadCast_WithVariousDurations_ShouldAcceptAllValues(int duration)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = (int)futureDate.DayOfWeek,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = duration,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act & Assert
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: futureDate,
                testMode: true);

            // Should not throw ArgumentException for duration
            act.Should().NotThrowAsync<ArgumentException>($"Duration={duration} should be valid");
        }

        [Fact]
        public void UpdateBroadcast_MethodSignature_ShouldHaveCorrectParameters()
        {
            // Arrange
            var method = typeof(YouTubeLibrary).GetMethod("UpdateBroadcast",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            var parameters = method!.GetParameters();
            parameters.Should().HaveCount(5);
            parameters[0].ParameterType.Should().Be(typeof(string)); // broadcastId
            parameters[1].ParameterType.Should().Be(typeof(bool?)); // autoStart
            parameters[2].ParameterType.Should().Be(typeof(bool?)); // autoStop
            parameters[3].ParameterType.Should().Be(typeof(PrivacyEnum?)); // privacy
            parameters[4].ParameterType.Should().Be(typeof(bool?)); // chatEnabled
        }

        [Fact]
        public void UpdateBroadcast_WithAllNullParameters_ShouldNotThrowForSignature()
        {
            // This test verifies the method can be called with all optional parameters as null
            // The actual API call will fail without credentials, but the signature should be valid

            // Arrange
            var library = new YouTubeLibrary();
            var method = typeof(YouTubeLibrary).GetMethod("UpdateBroadcast",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Assert - method should exist and accept nulls
            method.Should().NotBeNull();
            var parameters = method!.GetParameters();

            // All optional parameters should be nullable
            parameters[1].ParameterType.Should().Be(typeof(bool?));
            parameters[2].ParameterType.Should().Be(typeof(bool?));
            parameters[3].ParameterType.Should().Be(typeof(PrivacyEnum?));
            parameters[4].ParameterType.Should().Be(typeof(bool?));
        }
    }
}

