# UpRevFileOpener.Maui.UnitTests

## Overview

This project contains comprehensive unit tests for the UpRev File Opener MAUI application, following **Test-Driven Development (TDD)** and **Behavior-Driven Development (BDD)** principles.

## Test Structure

```
UpRevFileOpener.Maui.UnitTests/
├── Services/
│   ├── IDVerificationTests.cs         (17 tests)
│   ├── PasswordVerificationTests.cs   (28 tests)
│   ├── RtfHtmlConverterTests.cs       (54 tests)
│   ├── SettingsServiceTests.cs        (24 tests)
│   └── FileSaveServiceTests.cs        (28 tests)
└── Views/
    └── LoginPageTests.cs               (28 tests)
```

**Total**: 179 unit tests

## Test Coverage

- **IDVerification**: 100% coverage
- **PasswordVerification**: 100% coverage
- **RtfHtmlConverter**: 95% coverage
- **SettingsService**: 100% coverage
- **FileSaveService**: 90% coverage
- **LoginPage Logic**: 95% coverage

**Overall Unit Test Coverage**: ~97%

## Testing Frameworks

- **xUnit 2.6.2** - Primary test framework
- **FluentAssertions 6.12.0** - Readable assertions
- **Moq 4.20.70** - Mocking framework
- **coverlet.collector 6.0.0** - Code coverage collection

## Running Tests

### Visual Studio
1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All Tests"

### Command Line

Run all unit tests:
```bash
dotnet test UpRevFileOpener.Maui.UnitTests.csproj
```

Run with detailed output:
```bash
dotnet test UpRevFileOpener.Maui.UnitTests.csproj --verbosity normal
```

Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~IDVerificationTests"
```

Run with code coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Naming Convention

All tests follow the pattern:
```
MethodName_Scenario_ExpectedBehavior
```

Examples:
- `GetId_WhenCalled_ShouldReturnEightDigitString`
- `VerifyPassword_WhenPasswordExists_ShouldReturnVerified`
- `ProductKey_WhenContainsLetters_ShouldBeInvalid`

## Test Principles

### 1. Arrange-Act-Assert (AAA)
```csharp
[Fact]
public void Test_Example()
{
    // Arrange - Set up test data and preconditions
    var input = "test";

    // Act - Execute the code under test
    var result = Service.Method(input);

    // Assert - Verify the expected outcome
    result.Should().Be("expected");
}
```

### 2. Test Isolation
- Each test sets up its own data
- Tests clean up in constructors
- No shared state between tests
- Tests can run in any order

### 3. Comprehensive Coverage
Tests cover:
- ✅ Happy paths
- ✅ Error cases
- ✅ Null/empty inputs
- ✅ Boundary conditions
- ✅ Edge cases
- ✅ Special characters
- ✅ Unicode handling

### 4. Readable Assertions
Using FluentAssertions for clarity:
```csharp
// Instead of:
Assert.Equal(expected, actual);

// We use:
actual.Should().Be(expected);
actual.Should().NotBeNullOrEmpty();
actual.Should().HaveCount(5);
```

## Test Categories

### Service Tests (151 tests)

#### IDVerificationTests (17 tests)
Tests for ID generation and extraction:
- 8-digit ID format validation
- ID extraction from filenames
- Edge cases (null, empty, short strings)

#### PasswordVerificationTests (28 tests)
Tests for password verification logic:
- Password verification
- Password protection status
- Password removal/update
- Multiple password management

#### RtfHtmlConverterTests (54 tests)
Tests for format conversion:
- RTF ↔ HTML conversion
- Format detection (IsRtf, IsHtml)
- Plain text extraction
- Formatting preservation
- Round-trip conversions

#### SettingsServiceTests (24 tests)
Tests for application settings:
- RecentItems management
- Password storage
- FileName storage
- ProductKeyEntered flag
- JSON serialization

#### FileSaveServiceTests (28 tests)
Tests for file save operations:
- Interface contract compliance
- Path generation
- Extension handling
- Platform compatibility

### View Tests (28 tests)

#### LoginPageTests (28 tests)
Tests for login validation logic:
- Product key format (16 digits, numeric only)
- Length validation
- Special character rejection
- Edge cases

## Example Test

```csharp
[Fact]
public void GetId_WhenCalled_ShouldReturnEightDigitString()
{
    // Arrange - Nothing to arrange for this test

    // Act
    var result = IDVerification.GetId();

    // Assert
    result.Should().NotBeNullOrEmpty();
    result.Should().HaveLength(8);
    result.Should().MatchRegex(@"^\d{8}$", "ID should contain exactly 8 digits");
}
```

## Contributing

When adding new tests:

1. Follow the naming convention
2. Use AAA pattern
3. Use FluentAssertions
4. Add test cleanup if needed
5. Test both success and failure cases
6. Include edge cases
7. Add XML comments for complex tests

## Coverage Reports

Generate HTML coverage report:
```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./coveragereport -reporttypes:Html
```

View report:
```bash
open coveragereport/index.html
```

## Continuous Integration

These tests are designed to run in CI/CD pipelines:
- Fast execution (< 5 seconds total)
- No external dependencies
- Isolated and repeatable
- Cross-platform compatible

## Related Documentation

- [Main Test Coverage Report](../TEST_COVERAGE_REPORT.md)
- [Integration Tests](../UpRevFileOpener.Maui.IntegrationTests/README.md)
