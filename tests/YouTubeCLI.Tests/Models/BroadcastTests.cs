using FluentAssertions;
using Xunit;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Models
{
    public class BroadcastTests
    {
        [Fact]
        public void Broadcast_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange & Act
            var broadcast = new Broadcast();

            // Assert
            broadcast.id.Should().BeNull();
            broadcast.name.Should().BeNull();
            broadcast.dayOfWeek.Should().Be(0);
            broadcast.broadcastStart.Should().BeNull();
            broadcast.broadcastDurationInMinutes.Should().Be(0);
            broadcast.stream.Should().BeNull();
            broadcast.autoStart.Should().BeFalse();
            broadcast.autoStop.Should().BeFalse();
            broadcast.privacy.Should().BeNull();
            broadcast.thumbnail.Should().BeNull();
            broadcast.active.Should().BeFalse();
            broadcast.chatEnabled.Should().BeFalse();
        }

        [Fact]
        public void Broadcast_AllProperties_ShouldBeSettable()
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                id = "test-id",
                name = "Test Broadcast",
                dayOfWeek = 3,
                broadcastStart = "10:00 AM",
                broadcastDurationInMinutes = 90,
                stream = "test-stream",
                autoStart = true,
                autoStop = true,
                privacy = "public",
                thumbnail = "thumb.png",
                active = true,
                chatEnabled = true
            };

            // Assert
            broadcast.id.Should().Be("test-id");
            broadcast.name.Should().Be("Test Broadcast");
            broadcast.dayOfWeek.Should().Be(3);
            broadcast.broadcastStart.Should().Be("10:00 AM");
            broadcast.broadcastDurationInMinutes.Should().Be(90);
            broadcast.stream.Should().Be("test-stream");
            broadcast.autoStart.Should().BeTrue();
            broadcast.autoStop.Should().BeTrue();
            broadcast.privacy.Should().Be("public");
            broadcast.thumbnail.Should().Be("thumb.png");
            broadcast.active.Should().BeTrue();
            broadcast.chatEnabled.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(6)]
        public void Broadcast_DayOfWeek_ShouldAcceptValidValues(int dayOfWeek)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                dayOfWeek = dayOfWeek
            };

            // Assert
            broadcast.dayOfWeek.Should().Be(dayOfWeek);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Broadcast_AutoStart_ShouldAcceptBooleanValues(bool autoStart)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                autoStart = autoStart
            };

            // Assert
            broadcast.autoStart.Should().Be(autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Broadcast_AutoStop_ShouldAcceptBooleanValues(bool autoStop)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                autoStop = autoStop
            };

            // Assert
            broadcast.autoStop.Should().Be(autoStop);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Broadcast_Active_ShouldAcceptBooleanValues(bool active)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                active = active
            };

            // Assert
            broadcast.active.Should().Be(active);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Broadcast_ChatEnabled_ShouldAcceptBooleanValues(bool chatEnabled)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                chatEnabled = chatEnabled
            };

            // Assert
            broadcast.chatEnabled.Should().Be(chatEnabled);
        }

        [Theory]
        [InlineData("private")]
        [InlineData("public")]
        [InlineData("unlisted")]
        [InlineData("Private")]
        [InlineData("Public")]
        [InlineData("Unlisted")]
        public void Broadcast_Privacy_ShouldAcceptDifferentValues(string privacy)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                privacy = privacy
            };

            // Assert
            broadcast.privacy.Should().Be(privacy);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(60)]
        [InlineData(90)]
        [InlineData(120)]
        [InlineData(180)]
        [InlineData(240)]
        public void Broadcast_Duration_ShouldAcceptVariousValues(int duration)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                broadcastDurationInMinutes = duration
            };

            // Assert
            broadcast.broadcastDurationInMinutes.Should().Be(duration);
        }

        [Theory]
        [InlineData("10:00 AM")]
        [InlineData("2:30 PM")]
        [InlineData("12:00 PM")]
        [InlineData("11:59 PM")]
        [InlineData("00:00")]
        [InlineData("23:59")]
        public void Broadcast_StartTime_ShouldAcceptVariousFormats(string startTime)
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                broadcastStart = startTime
            };

            // Assert
            broadcast.broadcastStart.Should().Be(startTime);
        }

        [Fact]
        public void Broadcast_WithAllProperties_ShouldMaintainAllValues()
        {
            // Arrange & Act
            var broadcast = new Broadcast
            {
                id = "complex-id-123",
                name = "Complex Broadcast Name",
                dayOfWeek = 5,
                broadcastStart = "3:45 PM",
                broadcastDurationInMinutes = 150,
                stream = "complex-stream-name",
                autoStart = true,
                autoStop = false,
                privacy = "unlisted",
                thumbnail = "path/to/thumbnail.jpg",
                active = true,
                chatEnabled = true
            };

            // Assert - verify all properties are maintained
            broadcast.id.Should().Be("complex-id-123");
            broadcast.name.Should().Be("Complex Broadcast Name");
            broadcast.dayOfWeek.Should().Be(5);
            broadcast.broadcastStart.Should().Be("3:45 PM");
            broadcast.broadcastDurationInMinutes.Should().Be(150);
            broadcast.stream.Should().Be("complex-stream-name");
            broadcast.autoStart.Should().BeTrue();
            broadcast.autoStop.Should().BeFalse();
            broadcast.privacy.Should().Be("unlisted");
            broadcast.thumbnail.Should().Be("path/to/thumbnail.jpg");
            broadcast.active.Should().BeTrue();
            broadcast.chatEnabled.Should().BeTrue();
        }
    }
}

