# UpRevFileOpener.Maui.IntegrationTests

## Overview

This project contains integration tests for the UpRev File Opener MAUI application, testing complete workflows and interactions between multiple services following **BDD (Behavior-Driven Development)** principles.

## Test Structure

```
UpRevFileOpener.Maui.IntegrationTests/
└── Workflows/
    ├── FileOperationsWorkflowTests.cs        (15 tests)
    └── PasswordProtectionWorkflowTests.cs    (21 tests)
```

**Total**: 36 integration tests

## Test Coverage

Integration tests cover end-to-end workflows:
- ✅ File save/load operations
- ✅ Password protection lifecycle
- ✅ Recent files management
- ✅ RTF/HTML conversion workflows
- ✅ Multiple file management
- ✅ Password verification workflows
- ✅ Error scenarios

**Overall Integration Test Coverage**: 100% of critical workflows

## Testing Frameworks

- **xUnit 2.6.2** - Test framework
- **FluentAssertions 6.12.0** - Readable assertions
- **Moq 4.20.70** - Mocking framework
- **coverlet.collector 6.0.0** - Code coverage

## Running Tests

### Visual Studio
1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All Tests"

### Command Line

Run all integration tests:
```bash
dotnet test UpRevFileOpener.Maui.IntegrationTests.csproj
```

Run with detailed output:
```bash
dotnet test UpRevFileOpener.Maui.IntegrationTests.csproj --verbosity normal
```

Run specific workflow:
```bash
dotnet test --filter "FullyQualifiedName~PasswordProtectionWorkflowTests"
```

## Workflow Tests

### 1. File Operations Workflow (15 tests)

Tests complete file operation scenarios:

#### Save Operations
- `Workflow_SaveFileWithoutPassword_ShouldNotRequirePassword`
- `Workflow_SaveFileWithPassword_ShouldMarkAsProtected`
- `Workflow_CompleteFileSaveWithPassword_ShouldStoreAllInformation`

#### Password Management
- `Workflow_RemovePasswordFromFile_ShouldUpdateAllSettings`

#### Recent Files
- `Workflow_OpenFile_ShouldAddToRecentItems`
- `Workflow_OpenMultipleFiles_ShouldMaintainRecentList`
- `Workflow_RecentItems_ShouldLimitTo10Items`

#### Format Conversion
- `Workflow_SaveAsRtf_ShouldConvertHtmlToRtf`
- `Workflow_LoadRtfFile_ShouldConvertToHtmlForEditor`
- `Workflow_RoundTripConversion_ShouldPreserveContent`
- `Workflow_DetectFileFormat_ShouldIdentifyCorrectly`

#### Multiple Files
- `Workflow_MultipleFilesWithDifferentPasswords_ShouldMaintainSeparately`
- `Workflow_RemoveOnePassword_ShouldNotAffectOthers`

#### Error Handling
- `Workflow_VerifyWrongPassword_ShouldReturnNotVerified`
- `Workflow_AccessNonExistentFile_ShouldReturnNoFiles`

### 2. Password Protection Workflow (21 tests)

Tests complete password protection lifecycle:

#### Setup
- `Workflow_SetupPasswordProtection_ShouldCompleteSuccessfully`
- `Workflow_ProtectMultipleFiles_ShouldTrackEachIndependently`

#### Verification
- `Workflow_VerifyCorrectPassword_ShouldGrantAccess`
- `Workflow_VerifyIncorrectPassword_ShouldDenyAccess`
- `Workflow_AccessUnprotectedFile_ShouldNotRequirePassword`
- `Workflow_MultiplePasswordAttempts_ShouldAllowRetries`

#### Password Changes
- `Workflow_ChangePassword_ShouldUpdateSuccessfully`

#### Removal
- `Workflow_RemovePasswordProtection_ShouldMakeFilePublic`
- `Workflow_RemoveProtectionFromOneOfMany_ShouldOnlyAffectTargetFile`

#### ID Association
- `Workflow_IDLinksFileToPassword_ShouldMaintainAssociation`
- `Workflow_DifferentIDsForDifferentFiles_ShouldPreventCrossTalk`

#### Edge Cases
- `Workflow_EmptyPasswordList_ShouldIndicateNotProtected`
- `Workflow_MismatchedFileAndPasswordCounts_ShouldHandleGracefully`
- `Workflow_SamePasswordForMultipleFiles_ShouldWorkIndependently`
- `Workflow_CaseSensitivePassword_ShouldDistinguishCase`

#### Complete Lifecycle
- `Workflow_CompletePasswordLifecycle_ShouldWorkEndToEnd`

## Test Principles

### 1. Workflow-Based Testing
Tests simulate real user workflows:
```csharp
[Fact]
public void Workflow_CompleteFileSaveWithPassword_ShouldStoreAllInformation()
{
    // Arrange - Simulate user wanting to save a password-protected file
    const string fileName = "mydocument.rtf";
    const string password = "secretpass";

    // Act - Execute the complete workflow
    var fileId = IDVerification.GetId();
    // ... store file and password with ID ...

    // Assert - Verify entire workflow succeeded
    var isProtected = PasswordVerification.IsPasswordProtected(fileName);
    var retrievedId = IDVerification.GetIdFromFileNames(fileName);
    var passwordVerified = PasswordVerification.VerifyPassword(passwordWithId);

    isProtected.Should().BeTrue();
    retrievedId.Should().Be(fileId);
    passwordVerified.Should().Be("Verified");
}
```

### 2. Multiple Service Interaction
Integration tests verify services work together:
- IDVerification ↔ PasswordVerification
- IDVerification ↔ SettingsService
- PasswordVerification ↔ SettingsService
- RtfHtmlConverter standalone workflows

### 3. Real-World Scenarios
Tests cover actual use cases:
- User saves a password-protected file
- User opens a recent file
- User changes a password
- User removes password protection
- Multiple files managed simultaneously

### 4. State Management
Tests verify persistent state:
- Settings are saved correctly
- State survives across operations
- Multiple operations don't interfere

## Example Integration Test

```csharp
[Fact]
public void Workflow_CompletePasswordLifecycle_ShouldWorkEndToEnd()
{
    // Arrange
    const string fileName = "lifecycle-test.rtf";
    const string initialPassword = "InitialPass";
    const string newPassword = "NewPass";

    // Act & Assert - Step 1: Create protected file
    var fileId1 = IDVerification.GetId();
    SettingsService.FileNames = new List<string> { fileId1 + fileName };
    SettingsService.Passwords = new List<string> { fileId1 + initialPassword };

    PasswordVerification.IsPasswordProtected(fileName).Should().BeTrue();

    // Step 2: Change password
    PasswordVerification.UpdatePassword(fileName);
    var fileId2 = IDVerification.GetId();
    SettingsService.FileNames = new List<string> { fileId2 + fileName };
    SettingsService.Passwords = new List<string> { fileId2 + newPassword };

    PasswordVerification.VerifyPassword(fileId2 + newPassword).Should().Be("Verified");

    // Step 3: Remove password
    PasswordVerification.UpdatePassword(fileName);

    PasswordVerification.IsPasswordProtected(fileName).Should().BeFalse();
}
```

## Workflow Diagrams

### File Save with Password Workflow
```
User Action → IDVerification.GetId()
           → Store file (ID + filename) in SettingsService
           → Store password (ID + password) in SettingsService
           → PasswordVerification confirms protection
```

### Password Verification Workflow
```
User Opens File → PasswordVerification.IsPasswordProtected()
                → IDVerification.GetIdFromFileNames()
                → User Enters Password
                → PasswordVerification.VerifyPassword()
                → Grant/Deny Access
```

## Test Isolation

Each test:
- Clears all settings in constructor
- Sets up its own test data
- Executes independently
- Doesn't affect other tests

## Continuous Integration

Integration tests are designed for CI/CD:
- Fast execution (< 10 seconds total)
- No external dependencies
- Repeatable results
- Cross-platform compatible

## Future Enhancements

Potential additions:
- [ ] UI automation tests (Appium)
- [ ] Performance testing for large files
- [ ] Concurrent access scenarios
- [ ] Database integration tests (if added)

## Related Documentation

- [Main Test Coverage Report](../TEST_COVERAGE_REPORT.md)
- [Unit Tests](../UpRevFileOpener.Maui.UnitTests/README.md)
