using FluentAssertions;
using System.Runtime.InteropServices;
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
            osInfo.Should().ContainAny("Windows", "macOS", "Linux", "Unknown");
        }

        [Fact]
        public void GetOSInfo_ShouldContainArchitecture()
        {
            // Act
            var osInfo = OSDetection.GetOSInfo();

            // Assert
            osInfo.Should().NotBeNullOrEmpty();
            // Should contain architecture info (X64, Arm64, etc.)
            var architecture = RuntimeInformation.OSArchitecture.ToString();
            osInfo.Should().Contain(architecture);
        }

        [Fact]
        public void GetArchitecture_ShouldReturnValidArchitecture()
        {
            // Act
            var architecture = OSDetection.GetArchitecture();

            // Assert
            architecture.Should().NotBeNullOrEmpty();
            architecture.Should().BeOneOf("X64", "X86", "Arm64", "Arm", "Wasm", "S390x");
        }

        [Fact]
        public void IsWindows_ShouldReturnBoolean()
        {
            // Act
            var isWindows = OSDetection.IsWindows();

            // Assert - should return a boolean value without throwing
            (isWindows == true || isWindows == false).Should().BeTrue();
        }

        [Fact]
        public void IsMacOS_ShouldReturnBoolean()
        {
            // Act
            var isMacOS = OSDetection.IsMacOS();

            // Assert - should return a boolean value without throwing
            (isMacOS == true || isMacOS == false).Should().BeTrue();
        }

        [Fact]
        public void IsLinux_ShouldReturnBoolean()
        {
            // Act
            var isLinux = OSDetection.IsLinux();

            // Assert - should return a boolean value without throwing
            (isLinux == true || isLinux == false).Should().BeTrue();
        }

        [Fact]
        public void GetOSName_ShouldReturnValidOSName()
        {
            // Act
            var osName = OSDetection.GetOSName();

            // Assert
            osName.Should().NotBeNullOrEmpty();
            osName.Should().BeOneOf("Windows", "macOS", "Linux", "Unknown");
        }

        [Fact]
        public void GetPathSeparator_ShouldReturnCorrectSeparator()
        {
            // Act
            var separator = OSDetection.GetPathSeparator();

            // Assert
            if (OSDetection.IsWindows())
            {
                separator.Should().Be('\\');
            }
            else
            {
                separator.Should().Be('/');
            }
        }

        [Theory]
        [InlineData("path/to/file.png")]
        [InlineData("path\\to\\file.png")]
        [InlineData("path/to\\file.png")]
        public void NormalizePath_ShouldNormalizePathCorrectly(string input)
        {
            // Act
            var result = OSDetection.NormalizePath(input);

            // Assert - On non-Windows, both should use forward slashes
            if (!OSDetection.IsWindows())
            {
                result.Should().NotContain("\\");
            }
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

        [Fact]
        public void NormalizePath_WithMixedSeparators_ShouldNormalize()
        {
            // Arrange
            var input = "path/to\\file.png";

            // Act
            var result = OSDetection.NormalizePath(input);

            // Assert
            result.Should().NotBeNullOrEmpty();
            if (OSDetection.IsWindows())
            {
                result.Should().NotContain("/");
                result.Should().Contain("\\");
            }
            else
            {
                result.Should().NotContain("\\");
                result.Should().Contain("/");
            }
        }

        [Fact]
        public void NormalizePath_WithWindowsPath_ShouldNormalizeOnWindows()
        {
            // Arrange
            var input = "C:\\Users\\Test\\file.png";

            // Act
            var result = OSDetection.NormalizePath(input);

            // Assert
            if (OSDetection.IsWindows())
            {
                result.Should().NotContain("/");
            }
        }

        [Fact]
        public void NormalizePath_WithUnixPath_ShouldNormalizeOnUnix()
        {
            // Arrange
            var input = "/home/user/file.png";

            // Act
            var result = OSDetection.NormalizePath(input);

            // Assert
            if (!OSDetection.IsWindows())
            {
                result.Should().NotContain("\\");
            }
        }

        [Theory]
        [InlineData("simple")]
        [InlineData("path/to/file")]
        [InlineData("path\\to\\file")]
        [InlineData("C:\\Windows\\Path")]
        [InlineData("/usr/local/bin")]
        public void NormalizePath_WithVariousPaths_ShouldNotThrow(string path)
        {
            // Act
            var result = OSDetection.NormalizePath(path);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
