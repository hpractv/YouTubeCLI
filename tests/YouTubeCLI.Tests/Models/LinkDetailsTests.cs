using FluentAssertions;
using Xunit;
using YouTubeCLI.Models;

namespace YouTubeCLI.Tests.Models
{
    public class LinkDetailsTests
    {
        [Fact]
        public void LinkDetails_BroadcastUrl_ShouldGenerateCorrectYouTubeUrl()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                id = "dQw4w9WgXcQ",
                title = "Test Video"
            };

            // Act
            var url = linkDetails.broadcastUrl;

            // Assert
            url.Should().Be("https://youtu.be/dQw4w9WgXcQ");
        }

        [Fact]
        public void LinkDetails_EmbeddedCode_ShouldGenerateCorrectIframeHtml()
        {
            // Arrange
            var linkDetails = new LinkDetails
            {
                id = "test123",
                title = "Test Broadcast"
            };

            // Act
            var embeddedCode = linkDetails.embeddedCode;

            // Assert
            embeddedCode.Should().Contain("<iframe");
            embeddedCode.Should().Contain("https://www.youtube.com/embed/test123");
            embeddedCode.Should().Contain("autoplay=1");
            embeddedCode.Should().Contain("livemonitor=1");
            embeddedCode.Should().Contain("allowfullscreen");
        }

        [Fact]
        public void LinkDetails_Properties_ShouldBeSettable()
        {
            // Arrange & Act
            var linkDetails = new LinkDetails
            {
                id = "abc123",
                title = "My Test Title"
            };

            // Assert
            linkDetails.id.Should().Be("abc123");
            linkDetails.title.Should().Be("My Test Title");
        }

        [Theory]
        [InlineData("")]
        [InlineData("short")]
        [InlineData("a-very-long-video-id-with-special-chars_123")]
        public void LinkDetails_BroadcastUrl_ShouldHandleDifferentIdFormats(string videoId)
        {
            // Arrange
            var linkDetails = new LinkDetails { id = videoId };

            // Act
            var url = linkDetails.broadcastUrl;

            // Assert
            url.Should().Be($"https://youtu.be/{videoId}");
        }

        [Fact]
        public void LinkDetails_EmbeddedCode_ShouldIncludeIframeAttributes()
        {
            // Arrange
            var linkDetails = new LinkDetails { id = "video123" };

            // Act
            var embeddedCode = linkDetails.embeddedCode;

            // Assert
            embeddedCode.Should().Contain("width=\"425\"");
            embeddedCode.Should().Contain("height=\"344\"");
            embeddedCode.Should().Contain("frameborder=\"0\"");
            embeddedCode.Should().Contain("allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\"");
        }
    }
}
