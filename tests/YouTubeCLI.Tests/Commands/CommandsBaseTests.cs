using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using YouTubeCLI.Commands;
using YouTubeCLI.Models;

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
            // ClearCredential is always included in base CreateArgs
            args.Should().Contain("clear-credential");
            args.Should().Contain("False");
        }

        [Fact]
        public void CreateArgs_WithTestModeTrue_ShouldIncludeTestMode()
        {
            // Arrange
            var command = new TestCommandsBase();
            // Use reflection to set internal property
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            testModeProperty.Should().NotBeNull();
            testModeProperty!.SetValue(command, true);

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
            testModeProperty.Should().NotBeNull();
            testModeProperty!.SetValue(command, false);

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().NotContain("testmode");
            args.Should().Contain("clear-credential");
        }

        [Fact]
        public void CreateArgs_WithClearCredentialTrue_ShouldIncludeClearCredential()
        {
            // Arrange
            var command = new TestCommandsBase();
            var clearCredentialProperty = typeof(CommandsBase).GetProperty("ClearCredential",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clearCredentialProperty.Should().NotBeNull();
            clearCredentialProperty!.SetValue(command, true);

            // Act
            var args = command.CreateArgs();

            // Assert
            args.Should().Contain("clear-credential");
            args.Should().Contain("True");
        }

        [Fact]
        public void ClearCredential_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var command = new TestCommandsBase();
            var clearCredentialProperty = typeof(CommandsBase).GetProperty("ClearCredential",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clearCredentialProperty.Should().NotBeNull();
            var value = (bool)clearCredentialProperty!.GetValue(command)!;

            // Assert
            value.Should().BeFalse();
        }

        [Fact]
        public void TestMode_ShouldBeSettable()
        {
            // Arrange
            var command = new TestCommandsBase();
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            testModeProperty.Should().NotBeNull();

            // Act
            testModeProperty!.SetValue(command, true);
            var value = (bool)testModeProperty.GetValue(command)!;

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
            testModeProperty.Should().NotBeNull();
            var value = (bool)testModeProperty!.GetValue(command)!;

            // Assert
            value.Should().BeFalse();
        }

        [Fact]
        public void GetBroadcasts_WithValidJsonFile_ShouldReturnBroadcasts()
        {
            // Arrange
            var testJson = """
            {
                "broadcasts": [
                    {
                        "id": "test-id-1",
                        "name": "Test Broadcast 1",
                        "active": true,
                        "dayOfWeek": 1,
                        "broadcastStart": "10:00 AM",
                        "broadcastDurationInMinutes": 60,
                        "stream": "test-stream",
                        "privacy": "private",
                        "autoStart": true,
                        "autoStop": true
                    },
                    {
                        "id": "test-id-2",
                        "name": "Test Broadcast 2",
                        "active": false,
                        "dayOfWeek": 2,
                        "broadcastStart": "11:00 AM",
                        "broadcastDurationInMinutes": 90,
                        "stream": "test-stream-2",
                        "privacy": "public",
                        "autoStart": false,
                        "autoStop": false
                    }
                ]
            }
            """;

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testJson);

            try
            {
                var command = new TestCommandsBase();

                // Act
                var result = command.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(2);
                result.Items.First().id.Should().Be("test-id-1");
                result.Items.Last().id.Should().Be("test-id-2");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithInvalidJsonFile_ShouldThrowException()
        {
            // Arrange
            var invalidJson = "{ invalid json }";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, invalidJson);
            var command = new TestCommandsBase();

            try
            {
                // Act & Assert
                var action = () => command.GetBroadcasts(tempFile);
                action.Should().Throw<Exception>();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithNonExistentFile_ShouldThrowException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(Path.GetTempPath(), $"non-existent-{Guid.NewGuid()}.json");
            var command = new TestCommandsBase();

            // Act & Assert
            var action = () => command.GetBroadcasts(nonExistentFile);
            action.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void GetBroadcasts_WithEmptyJsonFile_ShouldReturnEmptyBroadcasts()
        {
            // Arrange
            var emptyJson = "{}";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, emptyJson);
            var command = new TestCommandsBase();

            try
            {
                // Act
                var result = command.GetBroadcasts(tempFile);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().BeEmpty();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcasts_WithMalformedJson_ShouldThrowException()
        {
            // Arrange
            var malformedJson = "{\"broadcasts\": [ invalid }";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, malformedJson);
            var command = new TestCommandsBase();

            try
            {
                // Act & Assert
                var action = () => command.GetBroadcasts(tempFile);
                action.Should().Throw<Exception>();
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcast_WithValidId_ShouldReturnBroadcast()
        {
            // Arrange
            var testJson = """
            {
                "broadcasts": [
                    {
                        "id": "test-id-1",
                        "name": "Test Broadcast 1",
                        "active": true,
                        "dayOfWeek": 1,
                        "broadcastStart": "10:00 AM",
                        "broadcastDurationInMinutes": 60,
                        "stream": "test-stream",
                        "privacy": "private",
                        "autoStart": true,
                        "autoStop": true
                    }
                ]
            }
            """;

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testJson);
            var command = new TestCommandsBase();

            try
            {
                // Act
                var result = command.GetBroadcast(tempFile, "test-id-1");

                // Assert
                result.Should().NotBeNull();
                result.id.Should().Be("test-id-1");
                result.name.Should().Be("Test Broadcast 1");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcast_WithNonExistentId_ShouldThrowException()
        {
            // Arrange
            var testJson = """
            {
                "broadcasts": [
                    {
                        "id": "test-id-1",
                        "name": "Test Broadcast 1",
                        "active": true,
                        "dayOfWeek": 1,
                        "broadcastStart": "10:00 AM",
                        "broadcastDurationInMinutes": 60,
                        "stream": "test-stream",
                        "privacy": "private",
                        "autoStart": true,
                        "autoStop": true
                    }
                ]
            }
            """;

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, testJson);
            var command = new TestCommandsBase();

            try
            {
                // Act & Assert
                var action = () => command.GetBroadcast(tempFile, "non-existent-id");
                action.Should().Throw<InvalidOperationException>()
                    .WithMessage("*Sequence contains no matching element*");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void GetBroadcast_WithEmptyBroadcastsList_ShouldThrowException()
        {
            // Arrange
            var emptyJson = "{\"broadcasts\": []}";
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, emptyJson);
            var command = new TestCommandsBase();

            try
            {
                // Act & Assert
                var action = () => command.GetBroadcast(tempFile, "any-id");
                action.Should().Throw<InvalidOperationException>()
                    .WithMessage("*Sequence contains no matching element*");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void OnExecute_ShouldReturnZeroAndPrintArgs()
        {
            // Arrange
            var command = new TestCommandsBase();
            var testModeProperty = typeof(CommandsBase).GetProperty("TestMode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            testModeProperty.Should().NotBeNull();
            testModeProperty!.SetValue(command, false);

            // Capture console output
            var originalOut = Console.Out;
            using var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            try
            {
                // Act
                var result = command.CallOnExecute(null);

                // Assert
                result.Should().Be(0);
                var output = stringWriter.ToString();
                output.Should().Contain("Result = ytc");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }

    // Test implementation of CommandsBase for testing
    public class TestCommandsBase : CommandsBase
    {
        public new List<string> CreateArgs()
        {
            return base.CreateArgs();
        }

        public Broadcasts GetBroadcasts(string broadcastFile)
        {
            // Use reflection to access internal method
            var method = typeof(CommandsBase).GetMethod("getBroadcasts",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            try
            {
                return (Broadcasts)method!.Invoke(this, new object[] { broadcastFile })!;
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
            {
                // Unwrap the exception
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw; // Will never execute
            }
        }

        public Broadcast GetBroadcast(string broadcastFile, string id)
        {
            // Use reflection to access internal method
            var method = typeof(CommandsBase).GetMethod("getBroadcast",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            try
            {
                return (Broadcast)method!.Invoke(this, new object[] { broadcastFile, id })!;
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
            {
                // Unwrap the exception
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw; // Will never execute
            }
        }

        public int CallOnExecute(McMaster.Extensions.CommandLineUtils.CommandLineApplication? app)
        {
            return base.OnExecute(app!);
        }
    }
}
