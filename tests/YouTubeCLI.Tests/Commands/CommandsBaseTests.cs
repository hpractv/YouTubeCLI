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

        [Fact]
        public void CreateArgs_WithTestModeTrue_ShouldIncludeTestMode()
        {
            // Arrange
            var command = new TestCommandsBase();
            // Use reflection to set internal property
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            testModeProperty.SetValue(command, true);

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("testmode");
        }

        [Fact]
        public void CreateArgs_WithTestModeFalse_ShouldNotIncludeTestMode()
        {
            // Arrange
            var command = new TestCommandsBase();
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            testModeProperty.SetValue(command, false);

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("testmode");
        }

        [Fact]
        public void TestMode_ShouldBeSettable()
        {
            // Arrange
            var command = new TestCommandsBase();
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            testModeProperty.SetValue(command, true);
            var value = (bool)testModeProperty.GetValue(command);

            // Assert
            value.Should().BeTrue();
        }

        [Fact]
        public void TestMode_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var command = new TestCommandsBase();
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = (bool)testModeProperty.GetValue(command);

            // Assert
            value.Should().BeFalse();
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
