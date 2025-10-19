# YouTubeCLI Tests

This directory contains unit tests for the YouTubeCLI project.

## Test Structure

- `YouTubeCLI.Tests/` - Main test project
  - `Utilities/` - Tests for utility classes
  - `Libraries/` - Tests for library classes
  - `Commands/` - Tests for command classes

## Running Tests

### From Command Line

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "ClassName"
```

### From Visual Studio

1. Open the solution in Visual Studio
2. Go to Test → Test Explorer
3. Run all tests or specific tests

## Test Coverage

The tests cover:

- OS detection utilities
- Broadcast library functionality
- Command argument parsing
- Basic program structure validation

## Adding New Tests

When adding new functionality, please add corresponding unit tests to ensure code quality and prevent regressions.
