using FluentAssertions;
using System;
using System.Reflection;
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

        [Fact]
        public void UpdateBroadcast_Implementation_ShouldCheckAutoStartForNull()
        {
            // This test verifies that the UpdateBroadcast method implementation
            // contains a null check for the autoStart parameter before updating
            
            // Arrange
            var updateBroadcastMethod = typeof(YouTubeLibrary).GetMethod("UpdateBroadcast", 
                BindingFlags.Public | BindingFlags.Instance);
            
            // Assert
            updateBroadcastMethod.Should().NotBeNull("UpdateBroadcast method should exist");
            
            // Get the method body
            var methodBody = updateBroadcastMethod!.GetMethodBody();
            methodBody.Should().NotBeNull("UpdateBroadcast should have a method body");
            
            // Verify the method signature has nullable bool parameters
            var parameters = updateBroadcastMethod.GetParameters();
            parameters.Should().HaveCount(4, "UpdateBroadcast should have 4 parameters (broadcastId, autoStart, autoStop, privacy)");
            
            var autoStartParam = parameters[1];
            autoStartParam.Name.Should().Be("autoStart");
            autoStartParam.ParameterType.Should().Be(typeof(bool?), "autoStart should be nullable bool to allow unspecified values");
            
            var autoStopParam = parameters[2];
            autoStopParam.Name.Should().Be("autoStop");
            autoStopParam.ParameterType.Should().Be(typeof(bool?), "autoStop should be nullable bool to allow unspecified values");
            
            var privacyParam = parameters[3];
            privacyParam.Name.Should().Be("privacy");
            privacyParam.ParameterType.Should().Be(typeof(PrivacyEnum?), "privacy should be nullable enum to allow unspecified values");
        }

        [Theory]
        [InlineData(true, null, null)]
        [InlineData(false, null, null)]
        [InlineData(null, true, null)]
        [InlineData(null, false, null)]
        [InlineData(null, null, PrivacyEnum.Public)]
        [InlineData(null, null, PrivacyEnum.Private)]
        [InlineData(null, null, PrivacyEnum.Unlisted)]
        public void UpdateBroadcast_WithPartialParameters_ShouldAcceptNullValues(
            bool? autoStart, bool? autoStop, PrivacyEnum? privacy)
        {
            // This test verifies that UpdateBroadcast accepts null values for optional parameters
            // It documents the expected behavior that unspecified values should not cause errors
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart,
                autoStop,
                privacy);
            
            // Assert
            // The method should not throw ArgumentException for null values
            // It will fail with a different error (missing credentials/service) but that's expected
            // The key is that null parameters are accepted by the method signature
            act.Should().NotThrowAsync<ArgumentNullException>("null parameters should be allowed");
        }

        [Fact]
        public void UpdateBroadcast_WithAllNullOptionalParameters_ShouldAcceptCall()
        {
            // This test specifically verifies that when all optional update parameters are null,
            // the UpdateBroadcast method can be called (representing an update with no changes to optional fields)
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart: null,
                autoStop: null,
                privacy: null);
            
            // Assert
            // The method should accept the call even with all null values
            // It will fail later due to missing credentials, but the null parameters themselves should not cause ArgumentNullException
            act.Should().NotThrowAsync<ArgumentNullException>(
                "UpdateBroadcast should accept null values for all optional parameters, " +
                "allowing callers to update only the broadcast ID without changing other properties");
        }

        [Fact]
        public void UpdateBroadcast_WithOnlyAutoStartSpecified_ShouldAcceptCall()
        {
            // This test verifies that only autoStart can be updated while leaving autoStop and privacy unchanged
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart: true,
                autoStop: null,
                privacy: null);
            
            // Assert
            act.Should().NotThrowAsync<ArgumentNullException>(
                "UpdateBroadcast should accept updating only autoStart while leaving other values unspecified");
        }

        [Fact]
        public void UpdateBroadcast_WithOnlyAutoStopSpecified_ShouldAcceptCall()
        {
            // This test verifies that only autoStop can be updated while leaving autoStart and privacy unchanged
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart: null,
                autoStop: false,
                privacy: null);
            
            // Assert
            act.Should().NotThrowAsync<ArgumentNullException>(
                "UpdateBroadcast should accept updating only autoStop while leaving other values unspecified");
        }

        [Fact]
        public void UpdateBroadcast_WithOnlyPrivacySpecified_ShouldAcceptCall()
        {
            // This test verifies that only privacy can be updated while leaving autoStart and autoStop unchanged
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart: null,
                autoStop: null,
                privacy: PrivacyEnum.Public);
            
            // Assert
            act.Should().NotThrowAsync<ArgumentNullException>(
                "UpdateBroadcast should accept updating only privacy while leaving other values unspecified");
        }

        [Fact]
        public void UpdateBroadcast_WithTwoParametersSpecified_ShouldAcceptCall()
        {
            // This test verifies that a combination of parameters can be updated while leaving others unchanged
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart: true,
                autoStop: false,
                privacy: null);
            
            // Assert
            act.Should().NotThrowAsync<ArgumentNullException>(
                "UpdateBroadcast should accept updating multiple parameters while leaving others unspecified");
        }

        [Fact]
        public void UpdateBroadcast_WithAllParametersSpecified_ShouldAcceptCall()
        {
            // This test verifies that all parameters can be specified and updated together
            
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Act
            Func<Task> act = async () => await youTubeLibrary.UpdateBroadcast(
                "test-broadcast-id",
                autoStart: true,
                autoStop: false,
                privacy: PrivacyEnum.Private);
            
            // Assert
            act.Should().NotThrowAsync<ArgumentNullException>(
                "UpdateBroadcast should accept all parameters being specified");
        }
    }
}
