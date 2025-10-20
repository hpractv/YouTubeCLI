using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using YouTubeCLI.Commands;

namespace YouTubeCLI.Tests.Commands
{
    public class EndCommandTests
    {
        [Fact]
        public void CreateArgs_WithSingleId_ShouldIncludeIdArgument()
        {
            // Arrange
            var command = new EndCommand
            {
                Id = "test-id-123"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain("test-id-123");
        }

        [Fact]
        public void CreateArgs_WithMultipleIds_ShouldIncludeCommaSeparatedIds()
        {
            // Arrange
            var command = new EndCommand
            {
                Id = "test-id-123,test-id-456,test-id-789"
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain("test-id-123,test-id-456,test-id-789");
        }

        [Fact]
        public void CreateArgs_WithNullId_ShouldNotIncludeIdArgument()
        {
            // Arrange
            var command = new EndCommand
            {
                Id = null
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
        }

        [Fact]
        public void CreateArgs_WithEmptyId_ShouldNotIncludeIdArgument()
        {
            // Arrange
            var command = new EndCommand
            {
                Id = ""
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
        }

        [Fact]
        public void CreateArgs_WithWhitespaceId_ShouldNotIncludeIdArgument()
        {
            // Arrange
            var command = new EndCommand
            {
                Id = "   "
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("id");
        }

        [Theory]
        [InlineData("single-id")]
        [InlineData("id1,id2")]
        [InlineData("id1,id2,id3,id4")]
        [InlineData("1234567890")]
        [InlineData("abc-def-ghi")]
        public void CreateArgs_WithVariousIdFormats_ShouldIncludeIdArgument(string id)
        {
            // Arrange
            var command = new EndCommand
            {
                Id = id
            };

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("id");
            args.Should().Contain(id);
        }
    }
}
