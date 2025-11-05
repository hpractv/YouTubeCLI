using FluentAssertions;
using System;
using Xunit;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Models
{
    public class LiveBroadcastInfoTests
    {
        [Fact]
        public void LiveBroadcastInfo_Url_ShouldGenerateCorrectYouTubeUrl()
        {
            // Arrange
            var broadcastInfo = new LiveBroadcastInfo
            {
                youTubeId = "abc123xyz",
                title = "Test Broadcast"
            };

            // Act
            var url = broadcastInfo.url;

            // Assert
            url.Should().Be("https://youtu.be/abc123xyz");
        }

        [Fact]
        public void LiveBroadcastInfo_Link_ShouldGenerateCorrectHtmlAnchor()
        {
            // Arrange
            var broadcastInfo = new LiveBroadcastInfo
            {
                youTubeId = "test456",
                title = "My Awesome Stream"
            };

            // Act
            var link = broadcastInfo.link;

            // Assert
            link.Should().Be("<a href='https://youtu.be/test456'>My Awesome Stream</a>");
        }

        [Fact]
        public void LiveBroadcastInfo_Properties_ShouldBeSettable()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 15, 10, 30, 0);

            // Act
            var broadcastInfo = new LiveBroadcastInfo
            {
                broadcast = "broadcast-1",
                youTubeId = "yt123",
                title = "Test Title",
                start = startDate,
                autoStart = true,
                autoStop = false,
                privacy = "public"
            };

            // Assert
            broadcastInfo.broadcast.Should().Be("broadcast-1");
            broadcastInfo.youTubeId.Should().Be("yt123");
            broadcastInfo.title.Should().Be("Test Title");
            broadcastInfo.start.Should().Be(startDate);
            broadcastInfo.autoStart.Should().BeTrue();
            broadcastInfo.autoStop.Should().BeFalse();
            broadcastInfo.privacy.Should().Be("public");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void LiveBroadcastInfo_AutoStartAndAutoStop_ShouldAcceptBooleanValues(bool autoStart, bool autoStop)
        {
            // Arrange & Act
            var broadcastInfo = new LiveBroadcastInfo
            {
                autoStart = autoStart,
                autoStop = autoStop
            };

            // Assert
            broadcastInfo.autoStart.Should().Be(autoStart);
            broadcastInfo.autoStop.Should().Be(autoStop);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("private")]
        [InlineData("unlisted")]
        public void LiveBroadcastInfo_Privacy_ShouldAcceptDifferentValues(string privacy)
        {
            // Arrange & Act
            var broadcastInfo = new LiveBroadcastInfo
            {
                privacy = privacy
            };

            // Assert
            broadcastInfo.privacy.Should().Be(privacy);
        }

        [Fact]
        public void LiveBroadcastInfo_Start_ShouldStoreDateTimeCorrectly()
        {
            // Arrange
            var expectedDate = new DateTime(2025, 12, 31, 23, 59, 59);

            // Act
            var broadcastInfo = new LiveBroadcastInfo
            {
                start = expectedDate
            };

            // Assert
            broadcastInfo.start.Should().Be(expectedDate);
            broadcastInfo.start.Year.Should().Be(2025);
            broadcastInfo.start.Month.Should().Be(12);
            broadcastInfo.start.Day.Should().Be(31);
        }

        [Fact]
        public void LiveBroadcastInfo_Link_ShouldUseUrlProperty()
        {
            // Arrange
            var broadcastInfo = new LiveBroadcastInfo
            {
                youTubeId = "unique-id-789",
                title = "Broadcast Title"
            };

            // Act
            var link = broadcastInfo.link;
            var url = broadcastInfo.url;

            // Assert
            link.Should().Contain(url);
            link.Should().Contain($"href='{url}'");
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("very-long-youtube-video-id-12345")]
        public void LiveBroadcastInfo_Url_ShouldHandleDifferentIdFormats(string youTubeId)
        {
            // Arrange
            var broadcastInfo = new LiveBroadcastInfo
            {
                youTubeId = youTubeId
            };

            // Act
            var url = broadcastInfo.url;

            // Assert
            url.Should().Be($"https://youtu.be/{youTubeId}");
        }
    }
}
