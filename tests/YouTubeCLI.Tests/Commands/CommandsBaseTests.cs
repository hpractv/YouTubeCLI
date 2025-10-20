using FluentAssertions;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class CommandsBaseTests
    {
        [Fact]
        public void CreateArgs_ShouldReturnEmptyListByDefault()
        {
            // Arrange
            var command = new TestCommandsBase();

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotBeNull();
            args.Should().BeEmpty();
        }
    }

    // Test implementation of CommandsBase for testing
    public class TestCommandsBase : CommandsBase
    {
        public new List<string> CreateArgs()
        {
            return base.CreateArgs();
        }
    }
}