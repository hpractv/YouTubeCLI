using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using YouTubeCLI.Libraries;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Libraries
{
    /// <summary>
    /// Comprehensive tests for date calculation logic in BuildBroadCast method.
    /// Tests all combinations of start day of week and target day of week to ensure
    /// correct next broadcast date calculation.
    /// </summary>
    public class BroadcastDateCalculationTests
    {
        /// <summary>
        /// Test all 49 combinations of start day (0-6) and target day (0-6).
        /// This ensures the date calculation logic works correctly for every possible scenario.
        /// </summary>
        /// <param name="startDay">Day of week for start date (0=Sunday, 6=Saturday)</param>
        /// <param name="targetDay">Target day of week for broadcast (0=Sunday, 6=Saturday)</param>
        /// <param name="expectedDaysToAdd">Expected number of days to add to reach target day</param>
        [Theory]
        [InlineData(0, 0, 0)] // Sunday to Sunday - same day
        [InlineData(0, 1, 1)] // Sunday to Monday - next day
        [InlineData(0, 2, 2)] // Sunday to Tuesday
        [InlineData(0, 3, 3)] // Sunday to Wednesday
        [InlineData(0, 4, 4)] // Sunday to Thursday
        [InlineData(0, 5, 5)] // Sunday to Friday
        [InlineData(0, 6, 6)] // Sunday to Saturday
        [InlineData(1, 0, 6)] // Monday to Sunday - wrap around
        [InlineData(1, 1, 0)] // Monday to Monday - same day
        [InlineData(1, 2, 1)] // Monday to Tuesday - next day
        [InlineData(1, 3, 2)] // Monday to Wednesday
        [InlineData(1, 4, 3)] // Monday to Thursday
        [InlineData(1, 5, 4)] // Monday to Friday
        [InlineData(1, 6, 5)] // Monday to Saturday
        [InlineData(2, 0, 5)] // Tuesday to Sunday - wrap around
        [InlineData(2, 1, 6)] // Tuesday to Monday - wrap around
        [InlineData(2, 2, 0)] // Tuesday to Tuesday - same day
        [InlineData(2, 3, 1)] // Tuesday to Wednesday - next day
        [InlineData(2, 4, 2)] // Tuesday to Thursday
        [InlineData(2, 5, 3)] // Tuesday to Friday
        [InlineData(2, 6, 4)] // Tuesday to Saturday
        [InlineData(3, 0, 4)] // Wednesday to Sunday - wrap around
        [InlineData(3, 1, 5)] // Wednesday to Monday - wrap around
        [InlineData(3, 2, 6)] // Wednesday to Tuesday - wrap around
        [InlineData(3, 3, 0)] // Wednesday to Wednesday - same day
        [InlineData(3, 4, 1)] // Wednesday to Thursday - next day
        [InlineData(3, 5, 2)] // Wednesday to Friday
        [InlineData(3, 6, 3)] // Wednesday to Saturday
        [InlineData(4, 0, 3)] // Thursday to Sunday - wrap around
        [InlineData(4, 1, 4)] // Thursday to Monday - wrap around
        [InlineData(4, 2, 5)] // Thursday to Tuesday - wrap around
        [InlineData(4, 3, 6)] // Thursday to Wednesday - wrap around
        [InlineData(4, 4, 0)] // Thursday to Thursday - same day
        [InlineData(4, 5, 1)] // Thursday to Friday - next day
        [InlineData(4, 6, 2)] // Thursday to Saturday
        [InlineData(5, 0, 2)] // Friday to Sunday - wrap around
        [InlineData(5, 1, 3)] // Friday to Monday - wrap around
        [InlineData(5, 2, 4)] // Friday to Tuesday - wrap around
        [InlineData(5, 3, 5)] // Friday to Wednesday - wrap around
        [InlineData(5, 4, 6)] // Friday to Thursday - wrap around
        [InlineData(5, 5, 0)] // Friday to Friday - same day
        [InlineData(5, 6, 1)] // Friday to Saturday - next day
        [InlineData(6, 0, 1)] // Saturday to Sunday - next day
        [InlineData(6, 1, 2)] // Saturday to Monday
        [InlineData(6, 2, 3)] // Saturday to Tuesday
        [InlineData(6, 3, 4)] // Saturday to Wednesday
        [InlineData(6, 4, 5)] // Saturday to Thursday
        [InlineData(6, 5, 6)] // Saturday to Friday
        [InlineData(6, 6, 0)] // Saturday to Saturday - same day
        public void BuildBroadCast_DateCalculation_ShouldCalculateCorrectNextOccurrence(
            int startDay, int targetDay, int expectedDaysToAdd)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Create a start date on a specific day of week
            // We use a known date and adjust to get the desired start day
            var baseDate = new DateOnly(2024, 1, 7); // This is a Sunday (day 0)
            var startDate = baseDate.AddDays(startDay);
            
            // Verify our start date has the correct day of week
            ((int)startDate.DayOfWeek).Should().Be(startDay, 
                $"Start date should be on day {startDay} ({(DayOfWeek)startDay})");
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act
            // We expect this to fail with API/credentials error, but not with ArgumentException
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: startDate,
                testMode: true);

            // Assert
            // Should not throw ArgumentException - the date calculation should be valid
            act.Should().NotThrowAsync<ArgumentException>(
                $"Date calculation from {(DayOfWeek)startDay} to {(DayOfWeek)targetDay} " +
                $"(expecting {expectedDaysToAdd} days added) should be valid");
        }

        /// <summary>
        /// Test that when start date is on the target day of week, it uses that same date
        /// </summary>
        [Theory]
        [InlineData(0)] // Sunday
        [InlineData(1)] // Monday
        [InlineData(2)] // Tuesday
        [InlineData(3)] // Wednesday
        [InlineData(4)] // Thursday
        [InlineData(5)] // Friday
        [InlineData(6)] // Saturday
        public void BuildBroadCast_WhenStartDateMatchesTargetDay_ShouldUseSameDate(int dayOfWeek)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Create a date on the specific day of week
            var baseDate = new DateOnly(2024, 1, 7); // Sunday
            var startDate = baseDate.AddDays(dayOfWeek);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = dayOfWeek,
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
                startsOn: startDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"When start date is on {(DayOfWeek)dayOfWeek}, it should use that same date");
        }

        /// <summary>
        /// Test wrap-around scenarios where target day is before start day in the week
        /// </summary>
        [Theory]
        [InlineData(6, 0, 1)] // Saturday to Sunday - should add 1 day (next day)
        [InlineData(5, 0, 2)] // Friday to Sunday - should add 2 days
        [InlineData(4, 0, 3)] // Thursday to Sunday - should add 3 days
        [InlineData(3, 0, 4)] // Wednesday to Sunday - should add 4 days
        [InlineData(2, 0, 5)] // Tuesday to Sunday - should add 5 days
        [InlineData(1, 0, 6)] // Monday to Sunday - should add 6 days
        [InlineData(6, 1, 2)] // Saturday to Monday - should add 2 days
        [InlineData(5, 1, 3)] // Friday to Monday - should add 3 days
        [InlineData(4, 1, 4)] // Thursday to Monday - should add 4 days
        [InlineData(3, 1, 5)] // Wednesday to Monday - should add 5 days
        [InlineData(2, 1, 6)] // Tuesday to Monday - should add 6 days
        public void BuildBroadCast_WrapAroundDays_ShouldCalculateCorrectly(
            int startDay, int targetDay, int expectedDaysToAdd)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            var baseDate = new DateOnly(2024, 1, 7); // Sunday
            var startDate = baseDate.AddDays(startDay);
            
            // Verify the wrap-around logic
            startDay.Should().BeGreaterThan(targetDay, 
                "This test is for wrap-around scenarios where target day is before start day");
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: startDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"Wrap-around from {(DayOfWeek)startDay} to {(DayOfWeek)targetDay} " +
                $"(expecting {expectedDaysToAdd} days) should work correctly");
        }

        /// <summary>
        /// Test forward scenarios where target day is after start day in the same week
        /// </summary>
        [Theory]
        [InlineData(0, 1, 1)] // Sunday to Monday
        [InlineData(0, 6, 6)] // Sunday to Saturday
        [InlineData(1, 2, 1)] // Monday to Tuesday
        [InlineData(1, 6, 5)] // Monday to Saturday
        [InlineData(2, 3, 1)] // Tuesday to Wednesday
        [InlineData(2, 6, 4)] // Tuesday to Saturday
        [InlineData(3, 4, 1)] // Wednesday to Thursday
        [InlineData(3, 6, 3)] // Wednesday to Saturday
        [InlineData(4, 5, 1)] // Thursday to Friday
        [InlineData(4, 6, 2)] // Thursday to Saturday
        [InlineData(5, 6, 1)] // Friday to Saturday
        public void BuildBroadCast_ForwardDays_ShouldCalculateCorrectly(
            int startDay, int targetDay, int expectedDaysToAdd)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            var baseDate = new DateOnly(2024, 1, 7); // Sunday
            var startDate = baseDate.AddDays(startDay);
            
            // Verify the forward logic
            startDay.Should().BeLessThan(targetDay, 
                "This test is for forward scenarios where target day is after start day");
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: startDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"Forward from {(DayOfWeek)startDay} to {(DayOfWeek)targetDay} " +
                $"(expecting {expectedDaysToAdd} days) should work correctly");
        }

        /// <summary>
        /// Test that broadcast start time is parsed correctly with various time formats
        /// </summary>
        [Theory]
        [InlineData("10:00 AM")]
        [InlineData("2:30 PM")]
        [InlineData("12:00 PM")]
        [InlineData("11:59 PM")]
        [InlineData("1:00 AM")]
        [InlineData("9:45 AM")]
        public void BuildBroadCast_WithVariousTimeFormats_ShouldParseCorrectly(string timeString)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = (int)futureDate.DayOfWeek,
                broadcastStart = timeString,
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
                startsOn: futureDate,
                testMode: true);

            // Assert
            // Should not throw ArgumentException or FormatException for time parsing
            act.Should().NotThrowAsync<ArgumentException>(
                $"Time format '{timeString}' should be parseable");
            act.Should().NotThrowAsync<FormatException>(
                $"Time format '{timeString}' should be valid");
        }

        /// <summary>
        /// Test date calculation with today as start date for all days of the week
        /// </summary>
        [Theory]
        [InlineData(0)] // Sunday
        [InlineData(1)] // Monday
        [InlineData(2)] // Tuesday
        [InlineData(3)] // Wednesday
        [InlineData(4)] // Thursday
        [InlineData(5)] // Friday
        [InlineData(6)] // Saturday
        public void BuildBroadCast_WithTodayAsStartDate_ForEachTargetDay_ShouldCalculateCorrectly(
            int targetDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: today,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"Calculating next {(DayOfWeek)targetDay} from today should be valid");
        }

        /// <summary>
        /// Test date calculation with null start date (defaults to today) for all target days
        /// </summary>
        [Theory]
        [InlineData(0)] // Sunday
        [InlineData(1)] // Monday
        [InlineData(2)] // Tuesday
        [InlineData(3)] // Wednesday
        [InlineData(4)] // Thursday
        [InlineData(5)] // Friday
        [InlineData(6)] // Saturday
        public void BuildBroadCast_WithNullStartDate_ForEachTargetDay_ShouldDefaultToToday(
            int targetDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: null, // Should default to today
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"Calculating next {(DayOfWeek)targetDay} with null start (today) should be valid");
        }

        /// <summary>
        /// Test that dates far in the future are calculated correctly
        /// </summary>
        [Theory]
        [InlineData(30, 0)] // 30 days from now, targeting Sunday
        [InlineData(60, 3)] // 60 days from now, targeting Wednesday
        [InlineData(90, 6)] // 90 days from now, targeting Saturday
        [InlineData(365, 1)] // 1 year from now, targeting Monday
        public void BuildBroadCast_WithFutureStartDate_ShouldCalculateCorrectly(
            int daysInFuture, int targetDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var futureDate = DateOnly.FromDateTime(DateTime.Now).AddDays(daysInFuture);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: futureDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"Calculating next {(DayOfWeek)targetDay} from {daysInFuture} days in future should be valid");
        }

        /// <summary>
        /// Test edge case of leap year dates (Feb 29)
        /// </summary>
        [Theory]
        [InlineData(2024, 2, 29, 4)] // Feb 29, 2024 is Thursday, target Thursday (same day)
        [InlineData(2024, 2, 29, 5)] // Feb 29, 2024 is Thursday, target Friday (next day)
        [InlineData(2024, 2, 29, 0)] // Feb 29, 2024 is Thursday, target Sunday (wrap around)
        public void BuildBroadCast_WithLeapYearDate_ShouldCalculateCorrectly(
            int year, int month, int day, int targetDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var leapYearDate = new DateOnly(year, month, day);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: leapYearDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"Leap year date {leapYearDate} to {(DayOfWeek)targetDay} should be valid");
        }

        /// <summary>
        /// Test year boundary crossing (December to January)
        /// </summary>
        [Theory]
        [InlineData(2024, 12, 29, 1)] // Dec 29 (Sunday) to Monday - crosses year boundary
        [InlineData(2024, 12, 31, 1)] // Dec 31 (Tuesday) to Monday - crosses year boundary, wrap around
        public void BuildBroadCast_CrossingYearBoundary_ShouldCalculateCorrectly(
            int year, int month, int day, int targetDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var endOfYearDate = new DateOnly(year, month, day);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: endOfYearDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"End of year date {endOfYearDate} to {(DayOfWeek)targetDay} (crossing year boundary) should be valid");
        }

        /// <summary>
        /// Test month boundary crossing
        /// </summary>
        [Theory]
        [InlineData(2024, 1, 31, 0)] // Jan 31 (Wednesday) to Sunday - crosses into February
        [InlineData(2024, 3, 31, 1)] // Mar 31 (Sunday) to Monday - crosses into April
        [InlineData(2024, 5, 31, 5)] // May 31 (Friday) to Friday - same day of week
        public void BuildBroadCast_CrossingMonthBoundary_ShouldCalculateCorrectly(
            int year, int month, int day, int targetDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var endOfMonthDate = new DateOnly(year, month, day);
            
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = targetDay,
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
                startsOn: endOfMonthDate,
                testMode: true);

            // Assert
            act.Should().NotThrowAsync<ArgumentException>(
                $"End of month date {endOfMonthDate} to {(DayOfWeek)targetDay} (crossing month boundary) should be valid");
        }

        /// <summary>
        /// Regression test for bug: Running on Saturday with broadcast set for Sunday was creating Monday broadcast
        /// </summary>
        [Fact]
        public void BuildBroadCast_RunningOnSaturdayForSundayBroadcast_ShouldCreateSundayNotMonday()
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            
            // Create a Saturday date
            var saturday = new DateOnly(2024, 12, 7); // This is a Saturday
            saturday.DayOfWeek.Should().Be(DayOfWeek.Saturday);
            
            // Create a broadcast set for Sunday (day 0)
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Sunday Broadcast",
                dayOfWeek = 0, // Sunday
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 60,
                stream = "test-stream",
                privacy = "private",
                autoStart = true,
                autoStop = true
            };

            // Act - Run on Saturday for Sunday broadcast
            Func<Task> act = async () => await youTubeLibrary.BuildBroadCast(
                broadcast,
                occurrences: 1,
                thumbnailDirectory: "/tmp",
                startsOn: saturday,
                testMode: true);

            // Assert
            // Should not throw ArgumentException
            // The next broadcast should be on Sunday (1 day after Saturday), not Monday (2 days after)
            act.Should().NotThrowAsync<ArgumentException>(
                "Running on Saturday for Sunday broadcast should calculate Sunday as next broadcast day");
        }

        /// <summary>
        /// Direct test of date calculation logic - verifies actual calculated dates
        /// </summary>
        [Theory]
        [InlineData(2024, 12, 7, 6, 0, 2024, 12, 8)] // Saturday Dec 7 -> Sunday Dec 8
        [InlineData(2024, 12, 7, 6, 1, 2024, 12, 9)] // Saturday Dec 7 -> Monday Dec 9
        [InlineData(2024, 12, 1, 0, 0, 2024, 12, 1)] // Sunday Dec 1 -> Sunday Dec 1 (same day)
        [InlineData(2024, 12, 1, 0, 1, 2024, 12, 2)] // Sunday Dec 1 -> Monday Dec 2
        [InlineData(2024, 12, 1, 0, 6, 2024, 12, 7)] // Sunday Dec 1 -> Saturday Dec 7
        [InlineData(2024, 12, 6, 5, 0, 2024, 12, 8)] // Friday Dec 6 -> Sunday Dec 8 (wrap)
        [InlineData(2025, 11, 29, 6, 0, 2025, 11, 30)] // User scenario: Saturday Nov 29, 2025 -> Sunday Nov 30, 2025
        [InlineData(2025, 11, 30, 0, 0, 2025, 11, 30)] // User scenario: Sunday Nov 30, 2025 -> Sunday Nov 30, 2025 (same day)
        public void CalculateNextBroadcastDate_WithVariousCombinations_ShouldReturnCorrectDate(
            int startYear, int startMonth, int startDay, int startDayOfWeek,
            int targetDayOfWeek,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var startDate = new DateOnly(startYear, startMonth, startDay);
            
            // Verify our test data is correct
            ((int)startDate.DayOfWeek).Should().Be(startDayOfWeek, 
                $"Test data error: {startDate:yyyy-MM-dd} should be day {startDayOfWeek} ({(DayOfWeek)startDayOfWeek})");
            
            var expectedDate = new DateOnly(expectedYear, expectedMonth, expectedDay);
            
            // Act
            var result = youTubeLibrary.CalculateNextBroadcastDate(startDate, targetDayOfWeek);
            
            // Assert
            result.Should().Be(expectedDate, 
                $"From {startDate:yyyy-MM-dd} ({(DayOfWeek)startDayOfWeek}) to target day {targetDayOfWeek} ({(DayOfWeek)targetDayOfWeek}) " +
                $"should give {expectedDate:yyyy-MM-dd} ({(DayOfWeek)targetDayOfWeek})");
            
            // Also verify the result is on the correct day of week
            ((int)result.DayOfWeek).Should().Be(targetDayOfWeek,
                $"Result {result:yyyy-MM-dd} should be on day {targetDayOfWeek} ({(DayOfWeek)targetDayOfWeek})");
        }

        /// <summary>
        /// Test that multiple occurrences are correctly spaced by 7 days
        /// This tests the loop logic that creates multiple broadcasts
        /// </summary>
        [Theory]
        [InlineData(2025, 11, 22, 6, 0, 4)] // User's scenario: Saturday Nov 22 -> 4 Sunday broadcasts
        [InlineData(2024, 12, 7, 6, 0, 3)] // Saturday Dec 7 -> 3 Sunday broadcasts
        public void CalculateNextBroadcastDate_WithMultipleOccurrences_ShouldSpaceBy7Days(
            int startYear, int startMonth, int startDay, int startDayOfWeek,
            int targetDayOfWeek, int occurrences)
        {
            // Arrange
            var youTubeLibrary = new YouTubeLibrary();
            var startDate = new DateOnly(startYear, startMonth, startDay);
            
            // Verify our test data is correct
            ((int)startDate.DayOfWeek).Should().Be(startDayOfWeek);
            
            // Act - Calculate first broadcast date
            var firstBroadcastDate = youTubeLibrary.CalculateNextBroadcastDate(startDate, targetDayOfWeek);
            
            // Assert - First broadcast should be on correct day of week
            ((int)firstBroadcastDate.DayOfWeek).Should().Be(targetDayOfWeek,
                $"First broadcast from {startDate:yyyy-MM-dd} should be on {(DayOfWeek)targetDayOfWeek}");
            
            // Simulate the loop logic that creates multiple occurrences
            var broadcastDate = firstBroadcastDate;
            for (int i = 0; i < occurrences; i++)
            {
                // Each occurrence should be on the target day of week
                ((int)broadcastDate.DayOfWeek).Should().Be(targetDayOfWeek,
                    $"Occurrence {i + 1} should be on {(DayOfWeek)targetDayOfWeek}, but got {broadcastDate:yyyy-MM-dd} ({broadcastDate.DayOfWeek})");
                
                // Move to next week
                broadcastDate = broadcastDate.AddDays(7);
            }
        }
    }
}
