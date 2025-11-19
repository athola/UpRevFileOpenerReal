using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.IntegrationTests.Workflows;

/// <summary>
/// Integration tests for password protection workflows following BDD principles
/// Tests the complete password protection lifecycle
/// </summary>
public class PasswordProtectionWorkflowTests
{
    #region Password Protection Setup Workflow

    [Fact]
    public void Workflow_SetupPasswordProtection_ShouldCompleteSuccessfully()
    {
        // Arrange
        const string fileName = "sensitive-document.rtf";
        const string password = "MySecurePassword123";

        // Act - Simulate setting up password protection
        var fileId = IDVerification.GetId();
        var fileNameWithId = fileId + fileName;
        var passwordWithId = fileId + password;

        var fileNames = new List<string> { fileNameWithId };
        var passwords = new List<string> { passwordWithId };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Assert - Verify protection is active
        var isProtected = PasswordVerification.IsPasswordProtected(fileName);
        var storedId = IDVerification.GetIdFromFileNames(fileName);

        isProtected.Should().BeTrue();
        storedId.Should().Be(fileId);
    }

    [Fact]
    public void Workflow_ProtectMultipleFiles_ShouldTrackEachIndependently()
    {
        // Arrange
        var files = new Dictionary<string, string>
        {
            { "document1.rtf", "password1" },
            { "document2.rtf", "password2" },
            { "document3.rtf", "password3" }
        };

        // Act - Protect all files
        var fileNames = new List<string>();
        var passwords = new List<string>();

        foreach (var file in files)
        {
            var id = IDVerification.GetId();
            fileNames.Add(id + file.Key);
            passwords.Add(id + file.Value);
        }

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Assert - Verify all are protected
        foreach (var file in files)
        {
            var isProtected = PasswordVerification.IsPasswordProtected(file.Key);
            isProtected.Should().BeTrue($"{file.Key} should be password protected");
        }
    }

    #endregion

    #region Password Verification Workflow

    [Fact]
    public void Workflow_VerifyCorrectPassword_ShouldGrantAccess()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string password = "SecretPass123";
        const string fileId = "12345678";

        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + password };

        // Act - User tries to open file with correct password
        var isProtected = PasswordVerification.IsPasswordProtected(fileName);
        var verificationResult = PasswordVerification.VerifyPassword(fileId + password);

        // Assert
        isProtected.Should().BeTrue();
        verificationResult.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_VerifyIncorrectPassword_ShouldDenyAccess()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string correctPassword = "CorrectPass";
        const string incorrectPassword = "WrongPass";
        const string fileId = "12345678";

        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + correctPassword };

        // Act - User tries to open file with wrong password
        var verificationResult = PasswordVerification.VerifyPassword(fileId + incorrectPassword);

        // Assert
        verificationResult.Should().Be("Not Verified");
    }

    [Fact]
    public void Workflow_AccessUnprotectedFile_ShouldNotRequirePassword()
    {
        // Arrange
        const string fileName = "public-document.rtf";
        SettingsService.FileNames = new List<string>();
        SettingsService.Passwords = new List<string>();

        // Act
        var isProtected = PasswordVerification.IsPasswordProtected(fileName);
        var verificationResult = PasswordVerification.VerifyPassword("anypassword");

        // Assert
        isProtected.Should().BeFalse();
        verificationResult.Should().Be("Not protected");
    }

    [Fact]
    public void Workflow_MultiplePasswordAttempts_ShouldAllowRetries()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string correctPassword = "CorrectPass";
        const string fileId = "12345678";

        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + correctPassword };

        // Act - Multiple password attempts
        var attempt1 = PasswordVerification.VerifyPassword(fileId + "wrong1");
        var attempt2 = PasswordVerification.VerifyPassword(fileId + "wrong2");
        var attempt3 = PasswordVerification.VerifyPassword(fileId + correctPassword);

        // Assert
        attempt1.Should().Be("Not Verified");
        attempt2.Should().Be("Not Verified");
        attempt3.Should().Be("Verified");
    }

    #endregion

    #region Password Change Workflow

    [Fact]
    public void Workflow_ChangePassword_ShouldUpdateSuccessfully()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string oldPassword = "OldPassword";
        const string newPassword = "NewPassword";
        const string fileId = "12345678";

        // Setup with old password
        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + oldPassword };

        // Act - Change password (remove old, add new)
        PasswordVerification.UpdatePassword(fileName);

        var newId = IDVerification.GetId();
        var newFileNames = new List<string> { newId + fileName };
        var newPasswords = new List<string> { newId + newPassword };

        SettingsService.FileNames = newFileNames;
        SettingsService.Passwords = newPasswords;

        // Assert
        var oldPasswordWorks = PasswordVerification.VerifyPassword(fileId + oldPassword);
        var newPasswordWorks = PasswordVerification.VerifyPassword(newId + newPassword);

        oldPasswordWorks.Should().Be("Not Verified");
        newPasswordWorks.Should().Be("Verified");
    }

    #endregion

    #region Password Removal Workflow

    [Fact]
    public void Workflow_RemovePasswordProtection_ShouldMakeFilePublic()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string password = "Password123";
        const string fileId = "12345678";

        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + password };

        // Verify file is protected
        var wasProtected = PasswordVerification.IsPasswordProtected(fileName);
        wasProtected.Should().BeTrue();

        // Act - Remove password protection
        PasswordVerification.UpdatePassword(fileName);

        // Assert
        var isStillProtected = PasswordVerification.IsPasswordProtected(fileName);
        var storedFileNames = SettingsService.FileNames;
        var storedPasswords = SettingsService.Passwords;

        isStillProtected.Should().BeFalse();
        storedFileNames.Should().BeEmpty();
        storedPasswords.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_RemoveProtectionFromOneOfMany_ShouldOnlyAffectTargetFile()
    {
        // Arrange
        const string file1 = "document1.rtf";
        const string file2 = "document2.rtf";
        const string file3 = "document3.rtf";

        var fileNames = new List<string>
        {
            "11111111" + file1,
            "22222222" + file2,
            "33333333" + file3
        };
        var passwords = new List<string>
        {
            "11111111password1",
            "22222222password2",
            "33333333password3"
        };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act - Remove protection from file2 only
        PasswordVerification.UpdatePassword(file2);

        // Assert
        var file1Protected = PasswordVerification.IsPasswordProtected(file1);
        var file2Protected = PasswordVerification.IsPasswordProtected(file2);
        var file3Protected = PasswordVerification.IsPasswordProtected(file3);

        file1Protected.Should().BeTrue();
        file2Protected.Should().BeFalse();
        file3Protected.Should().BeTrue();

        var storedFileNames = SettingsService.FileNames;
        var storedPasswords = SettingsService.Passwords;

        storedFileNames.Should().HaveCount(2);
        storedPasswords.Should().HaveCount(2);
    }

    #endregion

    #region ID-Based Association Workflow

    [Fact]
    public void Workflow_IDLinksFileToPassword_ShouldMaintainAssociation()
    {
        // Arrange
        const string fileName = "secure.rtf";
        const string password = "Pass123";
        const string fileId = "99887766";

        // Act - Create association through ID
        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + password };

        // Assert - Verify association works both ways
        var retrievedId = IDVerification.GetIdFromFileNames(fileName);
        var isProtected = PasswordVerification.IsPasswordProtected(fileName);
        var passwordVerified = PasswordVerification.VerifyPassword(fileId + password);

        retrievedId.Should().Be(fileId);
        isProtected.Should().BeTrue();
        passwordVerified.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_DifferentIDsForDifferentFiles_ShouldPreventCrossTalk()
    {
        // Arrange
        const string file1 = "document1.rtf";
        const string file2 = "document2.rtf";
        const string id1 = "11111111";
        const string id2 = "22222222";
        const string password1 = "password1";
        const string password2 = "password2";

        SettingsService.FileNames = new List<string>
        {
            id1 + file1,
            id2 + file2
        };
        SettingsService.Passwords = new List<string>
        {
            id1 + password1,
            id2 + password2
        };

        // Act - Try to verify file1 with file2's password
        var wrongCombination = PasswordVerification.VerifyPassword(id2 + password1);
        var correctCombination1 = PasswordVerification.VerifyPassword(id1 + password1);
        var correctCombination2 = PasswordVerification.VerifyPassword(id2 + password2);

        // Assert
        wrongCombination.Should().Be("Not Verified");
        correctCombination1.Should().Be("Verified");
        correctCombination2.Should().Be("Verified");
    }

    #endregion

    #region Edge Cases and Error Scenarios

    [Fact]
    public void Workflow_EmptyPasswordList_ShouldIndicateNotProtected()
    {
        // Arrange
        SettingsService.FileNames = new List<string>();
        SettingsService.Passwords = new List<string>();

        // Act
        var result = PasswordVerification.VerifyPassword("anypassword");

        // Assert
        result.Should().Be("Not protected");
    }

    [Fact]
    public void Workflow_MismatchedFileAndPasswordCounts_ShouldHandleGracefully()
    {
        // Arrange - More files than passwords
        SettingsService.FileNames = new List<string>
        {
            "11111111file1.rtf",
            "22222222file2.rtf"
        };
        SettingsService.Passwords = new List<string>
        {
            "11111111password1"
        };

        // Act
        var file1Protected = PasswordVerification.IsPasswordProtected("file1.rtf");
        var file2Protected = PasswordVerification.IsPasswordProtected("file2.rtf");
        var password1Verified = PasswordVerification.VerifyPassword("11111111password1");

        // Assert - System should still work for valid combinations
        file1Protected.Should().BeTrue();
        file2Protected.Should().BeTrue(); // File is in list, so marked as protected
        password1Verified.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_SamePasswordForMultipleFiles_ShouldWorkIndependently()
    {
        // Arrange - Different files with same password text (but different IDs)
        const string password = "SamePassword";
        const string file1 = "doc1.rtf";
        const string file2 = "doc2.rtf";
        const string id1 = "11111111";
        const string id2 = "22222222";

        SettingsService.FileNames = new List<string>
        {
            id1 + file1,
            id2 + file2
        };
        SettingsService.Passwords = new List<string>
        {
            id1 + password,
            id2 + password
        };

        // Act
        var file1Verified = PasswordVerification.VerifyPassword(id1 + password);
        var file2Verified = PasswordVerification.VerifyPassword(id2 + password);

        // Assert
        file1Verified.Should().Be("Verified");
        file2Verified.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_CaseSensitivePassword_ShouldDistinguishCase()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string password = "Password123";
        const string fileId = "12345678";

        SettingsService.FileNames = new List<string> { fileId + fileName };
        SettingsService.Passwords = new List<string> { fileId + password };

        // Act
        var correctCase = PasswordVerification.VerifyPassword(fileId + "Password123");
        var wrongCase = PasswordVerification.VerifyPassword(fileId + "password123");

        // Assert
        correctCase.Should().Be("Verified");
        wrongCase.Should().Be("Not Verified");
    }

    #endregion

    #region Complete Lifecycle Test

    [Fact]
    public void Workflow_CompletePasswordLifecycle_ShouldWorkEndToEnd()
    {
        // Arrange
        const string fileName = "lifecycle-test.rtf";
        const string initialPassword = "InitialPass";
        const string newPassword = "NewPass";

        // Act & Assert - Step 1: Create new protected file
        var fileId1 = IDVerification.GetId();
        SettingsService.FileNames = new List<string> { fileId1 + fileName };
        SettingsService.Passwords = new List<string> { fileId1 + initialPassword };

        var isProtected1 = PasswordVerification.IsPasswordProtected(fileName);
        var initialVerified = PasswordVerification.VerifyPassword(fileId1 + initialPassword);

        isProtected1.Should().BeTrue();
        initialVerified.Should().Be("Verified");

        // Step 2: Change password
        PasswordVerification.UpdatePassword(fileName);
        var fileId2 = IDVerification.GetId();
        SettingsService.FileNames = new List<string> { fileId2 + fileName };
        SettingsService.Passwords = new List<string> { fileId2 + newPassword };

        var newVerified = PasswordVerification.VerifyPassword(fileId2 + newPassword);
        var oldNotVerified = PasswordVerification.VerifyPassword(fileId1 + initialPassword);

        newVerified.Should().Be("Verified");
        oldNotVerified.Should().Be("Not Verified");

        // Step 3: Remove password protection
        PasswordVerification.UpdatePassword(fileName);

        var isProtected2 = PasswordVerification.IsPasswordProtected(fileName);
        var verificationAfterRemoval = PasswordVerification.VerifyPassword("anypassword");

        isProtected2.Should().BeFalse();
        verificationAfterRemoval.Should().Be("Not protected");
    }

    #endregion

    #region Cleanup

    public PasswordProtectionWorkflowTests()
    {
        // Clear all settings before each test
        SettingsService.RecentItems = new List<string>();
        SettingsService.FileNames = new List<string>();
        SettingsService.Passwords = new List<string>();
        SettingsService.ProductKeyEntered = false;
    }

    #endregion
}
