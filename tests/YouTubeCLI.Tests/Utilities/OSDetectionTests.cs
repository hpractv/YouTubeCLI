using FluentAssertions;
using Xunit;
using YouTubeCLI.Utilities;

namespace YouTubeCLI.Tests.Utilities
{
    public class OSDetectionTests
    {
        [Fact]
        public void GetOSInfo_ShouldReturnValidOSInfo()
        {
            // Act
            var osInfo = OSDetection.GetOSInfo();

            // Assert
            osInfo.Should().NotBeNullOrEmpty();
            osInfo.Should().ContainAny("Windows", "macOS", "Linux", "Unix");
        }

        [Theory]
        [InlineData("path/to/file.png", "path/to/file.png")]
        [InlineData("path\\to\\file.png", "path/to/file.png")]
        [InlineData("path/to\\file.png", "path/to/file.png")]
        public void NormalizePath_ShouldNormalizePathCorrectly(string input, string expected)
        {
            // Act
            var result = OSDetection.NormalizePath(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void NormalizePath_WithNullInput_ShouldReturnNull()
        {
            // Act
            var result = OSDetection.NormalizePath(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NormalizePath_WithEmptyInput_ShouldReturnEmpty()
        {
            // Act
            var result = OSDetection.NormalizePath("");

            // Assert
            result.Should().Be("");
        }
    }
}
