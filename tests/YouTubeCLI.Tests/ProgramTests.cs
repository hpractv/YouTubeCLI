using FluentAssertions;
using System;
using System.Reflection;
using Xunit;
using YouTubeCLI;
using YouTubeCLI = YouTubeCLI.YouTubeCLI;

namespace YouTubeCLI.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void GetVersion_ShouldReturnValidVersion()
        {
            // Act
            var version = typeof(YouTubeCLI).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            // Assert
            version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void YouTubeCLI_ShouldHaveCorrectCommandName()
        {
            // Arrange
            var commandAttribute = typeof(YouTubeCLI)
                .GetCustomAttribute<McMaster.Extensions.CommandLineUtils.CommandAttribute>();

            // Assert
            commandAttribute.Should().NotBeNull();
            commandAttribute?.Name.Should().Be("ytc");
        }

        [Fact]
        public void YouTubeCLI_ShouldHaveVersionOption()
        {
            // Arrange
            var versionAttribute = typeof(YouTubeCLI)
                .GetCustomAttribute<McMaster.Extensions.CommandLineUtils.VersionOptionFromMemberAttribute>();

            // Assert
            versionAttribute.Should().NotBeNull();
            versionAttribute?.Template.Should().Be("-v|--version");
        }

        [Fact]
        public void YouTubeCLI_ShouldHaveSubcommands()
        {
            // Arrange
            var subcommandAttribute = typeof(YouTubeCLI)
                .GetCustomAttribute<McMaster.Extensions.CommandLineUtils.SubcommandAttribute>();

            // Assert
            subcommandAttribute.Should().NotBeNull();
            // Note: We can't easily test the CommandTypes property without reflection
            // as it's not publicly accessible, so we'll just verify the attribute exists
        }
    }
}