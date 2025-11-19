using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.UnitTests.Services;

/// <summary>
/// Unit tests for PasswordVerification service following BDD principles
/// </summary>
public class PasswordVerificationTests
{
    #region VerifyPassword Tests

    [Fact]
    public void VerifyPassword_WhenPasswordExists_ShouldReturnVerified()
    {
        // Arrange
        const string password = "12345678mypassword";
        var passwords = new List<string> { password, "87654321another" };
        SettingsService.Passwords = passwords;

        // Act
        var result = PasswordVerification.VerifyPassword(password);

        // Assert
        result.Should().Be("Verified");
    }

    [Fact]
    public void VerifyPassword_WhenPasswordDoesNotExist_ShouldReturnNotVerified()
    {
        // Arrange
        var passwords = new List<string> { "12345678password1", "87654321password2" };
        SettingsService.Passwords = passwords;

        // Act
        var result = PasswordVerification.VerifyPassword("99999999wrongpassword");

        // Assert
        result.Should().Be("Not Verified");
    }

    [Fact]
    public void VerifyPassword_WhenPasswordsListIsEmpty_ShouldReturnNotProtected()
    {
        // Arrange
        SettingsService.Passwords = new List<string>();

        // Act
        var result = PasswordVerification.VerifyPassword("anypassword");

        // Assert
        result.Should().Be("Not protected");
    }

    [Fact]
    public void VerifyPassword_WhenPasswordsListIsNull_ShouldReturnNotProtected()
    {
        // Arrange
        SettingsService.Passwords = null!;

        // Act
        var result = PasswordVerification.VerifyPassword("anypassword");

        // Assert
        result.Should().Be("Not protected");
    }

    [Fact]
    public void VerifyPassword_WhenMultiplePasswordsExist_ShouldFindCorrectMatch()
    {
        // Arrange
        const string correctPassword = "55555555correctpass";
        var passwords = new List<string>
        {
            "11111111password1",
            "22222222password2",
            correctPassword,
            "33333333password3"
        };
        SettingsService.Passwords = passwords;

        // Act
        var result = PasswordVerification.VerifyPassword(correctPassword);

        // Assert
        result.Should().Be("Verified");
    }

    [Fact]
    public void VerifyPassword_WhenPasswordIsEmptyString_ShouldReturnNotVerified()
    {
        // Arrange
        var passwords = new List<string> { "12345678password" };
        SettingsService.Passwords = passwords;

        // Act
        var result = PasswordVerification.VerifyPassword("");

        // Assert
        result.Should().Be("Not Verified");
    }

    [Theory]
    [InlineData("12345678pass123", "Verified")]
    [InlineData("87654321different", "Not Verified")]
    public void VerifyPassword_WithVariousPasswords_ShouldReturnExpectedResult(
        string passwordToVerify, string expectedResult)
    {
        // Arrange
        var passwords = new List<string> { "12345678pass123", "11111111other" };
        SettingsService.Passwords = passwords;

        // Act
        var result = PasswordVerification.VerifyPassword(passwordToVerify);

        // Assert
        result.Should().Be(expectedResult);
    }

    #endregion

    #region IsPasswordProtected Tests

    [Fact]
    public void IsPasswordProtected_WhenFileNameExists_ShouldReturnTrue()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string fileNameWithId = "12345678" + fileName;
        var fileNames = new List<string> { fileNameWithId };
        SettingsService.FileNames = fileNames;

        // Act
        var result = PasswordVerification.IsPasswordProtected(fileName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPasswordProtected_WhenFileNameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var fileNames = new List<string> { "12345678document.rtf" };
        SettingsService.FileNames = fileNames;

        // Act
        var result = PasswordVerification.IsPasswordProtected("nonexistent.txt");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsPasswordProtected_WhenFileNamesListIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        SettingsService.FileNames = new List<string>();

        // Act
        var result = PasswordVerification.IsPasswordProtected("anyfile.txt");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsPasswordProtected_WhenFileNamesListIsNull_ShouldReturnFalse()
    {
        // Arrange
        SettingsService.FileNames = null!;

        // Act
        var result = PasswordVerification.IsPasswordProtected("anyfile.txt");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsPasswordProtected_WhenMultipleFilesExist_ShouldIdentifyCorrectFile()
    {
        // Arrange
        const string targetFileName = "mydocument.rtf";
        var fileNames = new List<string>
        {
            "11111111otherfile.txt",
            "22222222" + targetFileName,
            "33333333anotherfile.doc"
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = PasswordVerification.IsPasswordProtected(targetFileName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPasswordProtected_WhenFileNameWithIdIsTooShort_ShouldReturnFalse()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "short",     // Too short (< 9 chars)
            "12345678"   // Exactly 8 chars
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = PasswordVerification.IsPasswordProtected("file.txt");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsPasswordProtected_WhenFileNameHasSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        const string fileName = "my-file_name (1).txt";
        var fileNames = new List<string> { "55555555" + fileName };
        SettingsService.FileNames = fileNames;

        // Act
        var result = PasswordVerification.IsPasswordProtected(fileName);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("document.rtf", "12345678document.rtf", true)]
    [InlineData("test.txt", "99999999different.txt", false)]
    [InlineData("file.doc", "11111111file.doc", true)]
    public void IsPasswordProtected_WithVariousInputs_ShouldReturnExpectedResult(
        string fileName, string storedFileName, bool expectedResult)
    {
        // Arrange
        var fileNames = new List<string> { storedFileName };
        SettingsService.FileNames = fileNames;

        // Act
        var result = PasswordVerification.IsPasswordProtected(fileName);

        // Assert
        result.Should().Be(expectedResult);
    }

    #endregion

    #region UpdatePassword Tests

    [Fact]
    public void UpdatePassword_WhenFileExists_ShouldRemoveFileNameAndPassword()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string fileId = "12345678";
        const string fileNameWithId = fileId + fileName;
        const string passwordWithId = fileId + "mypassword";

        var fileNames = new List<string> { fileNameWithId, "87654321other.txt" };
        var passwords = new List<string> { passwordWithId, "87654321otherpass" };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        PasswordVerification.UpdatePassword(fileName);

        // Assert
        var updatedFileNames = SettingsService.FileNames;
        var updatedPasswords = SettingsService.Passwords;

        updatedFileNames.Should().NotContain(fileNameWithId);
        updatedPasswords.Should().NotContain(passwordWithId);
        updatedFileNames.Should().Contain("87654321other.txt");
        updatedPasswords.Should().Contain("87654321otherpass");
    }

    [Fact]
    public void UpdatePassword_WhenFileDoesNotExist_ShouldNotModifyLists()
    {
        // Arrange
        var originalFileNames = new List<string> { "12345678file1.txt", "87654321file2.txt" };
        var originalPasswords = new List<string> { "12345678pass1", "87654321pass2" };

        SettingsService.FileNames = new List<string>(originalFileNames);
        SettingsService.Passwords = new List<string>(originalPasswords);

        // Act
        PasswordVerification.UpdatePassword("nonexistent.txt");

        // Assert
        var updatedFileNames = SettingsService.FileNames;
        var updatedPasswords = SettingsService.Passwords;

        updatedFileNames.Should().BeEquivalentTo(originalFileNames);
        updatedPasswords.Should().BeEquivalentTo(originalPasswords);
    }

    [Fact]
    public void UpdatePassword_WhenMultipleFilesExist_ShouldOnlyRemoveMatchingOne()
    {
        // Arrange
        const string targetFileName = "target.txt";
        const string targetId = "55555555";

        var fileNames = new List<string>
        {
            "11111111file1.txt",
            targetId + targetFileName,
            "33333333file3.txt"
        };
        var passwords = new List<string>
        {
            "11111111pass1",
            targetId + "targetpass",
            "33333333pass3"
        };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        PasswordVerification.UpdatePassword(targetFileName);

        // Assert
        var updatedFileNames = SettingsService.FileNames;
        var updatedPasswords = SettingsService.Passwords;

        updatedFileNames.Should().HaveCount(2);
        updatedFileNames.Should().Contain("11111111file1.txt");
        updatedFileNames.Should().Contain("33333333file3.txt");
        updatedFileNames.Should().NotContain(targetId + targetFileName);

        updatedPasswords.Should().HaveCount(2);
        updatedPasswords.Should().Contain("11111111pass1");
        updatedPasswords.Should().Contain("33333333pass3");
        updatedPasswords.Should().NotContain(targetId + "targetpass");
    }

    [Fact]
    public void UpdatePassword_WhenPasswordListDoesNotContainMatchingId_ShouldStillRemoveFileName()
    {
        // Arrange
        const string fileName = "document.txt";
        const string fileId = "12345678";
        var fileNames = new List<string> { fileId + fileName };
        var passwords = new List<string> { "99999999differentid" };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        PasswordVerification.UpdatePassword(fileName);

        // Assert
        var updatedFileNames = SettingsService.FileNames;
        var updatedPasswords = SettingsService.Passwords;

        updatedFileNames.Should().BeEmpty();
        updatedPasswords.Should().Contain("99999999differentid");
    }

    [Fact]
    public void UpdatePassword_WhenFileNameWithIdIsTooShort_ShouldNotCrash()
    {
        // Arrange
        var fileNames = new List<string> { "short" };
        var passwords = new List<string> { "12345678pass" };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        var act = () => PasswordVerification.UpdatePassword("anyfile.txt");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UpdatePassword_WhenPasswordWithIdIsTooShort_ShouldNotCrash()
    {
        // Arrange
        const string fileName = "document.txt";
        const string fileId = "12345678";

        var fileNames = new List<string> { fileId + fileName };
        var passwords = new List<string> { "short" }; // Too short to extract ID

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        var act = () => PasswordVerification.UpdatePassword(fileName);

        // Assert
        act.Should().NotThrow();

        var updatedFileNames = SettingsService.FileNames;
        updatedFileNames.Should().BeEmpty();
    }

    [Fact]
    public void UpdatePassword_ShouldPersistChangesToSettings()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string fileId = "12345678";

        var fileNames = new List<string> { fileId + fileName };
        var passwords = new List<string> { fileId + "password" };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        PasswordVerification.UpdatePassword(fileName);

        // Assert - Changes should be persisted via SettingsService
        var persistedFileNames = SettingsService.FileNames;
        var persistedPasswords = SettingsService.Passwords;

        persistedFileNames.Should().BeEmpty();
        persistedPasswords.Should().BeEmpty();
    }

    #endregion

    #region Cleanup

    public PasswordVerificationTests()
    {
        // Clear settings before each test
        SettingsService.FileNames = new List<string>();
        SettingsService.Passwords = new List<string>();
    }

    #endregion
}
