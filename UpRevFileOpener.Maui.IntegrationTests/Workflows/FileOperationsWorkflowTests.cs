using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.IntegrationTests.Workflows;

/// <summary>
/// Integration tests for file operations workflows following BDD principles
/// Tests the interaction between multiple services for complete file operations
/// </summary>
public class FileOperationsWorkflowTests
{
    #region File Save and Load Workflow

    [Fact]
    public void Workflow_SaveFileWithoutPassword_ShouldNotRequirePassword()
    {
        // Arrange
        const string fileName = "document.rtf";
        SettingsService.FileNames = new List<string>();

        // Act
        var isPasswordProtected = PasswordVerification.IsPasswordProtected(fileName);

        // Assert
        isPasswordProtected.Should().BeFalse();
    }

    [Fact]
    public void Workflow_SaveFileWithPassword_ShouldMarkAsProtected()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string fileId = IDVerification.GetId();
        const string password = "mypassword";

        var fileNames = new List<string> { fileId + fileName };
        var passwords = new List<string> { fileId + password };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        var isPasswordProtected = PasswordVerification.IsPasswordProtected(fileName);

        // Assert
        isPasswordProtected.Should().BeTrue();
    }

    [Fact]
    public void Workflow_CompleteFileSaveWithPassword_ShouldStoreAllInformation()
    {
        // Arrange
        const string fileName = "mydocument.rtf";
        const string password = "secretpass";

        // Act - Simulate the save workflow
        var fileId = IDVerification.GetId();
        var fileNameWithId = fileId + fileName;
        var passwordWithId = fileId + password;

        var fileNames = new List<string> { fileNameWithId };
        var passwords = new List<string> { passwordWithId };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Assert - Verify all pieces are stored correctly
        var isProtected = PasswordVerification.IsPasswordProtected(fileName);
        var retrievedId = IDVerification.GetIdFromFileNames(fileName);
        var passwordVerified = PasswordVerification.VerifyPassword(passwordWithId);

        isProtected.Should().BeTrue();
        retrievedId.Should().Be(fileId);
        passwordVerified.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_RemovePasswordFromFile_ShouldUpdateAllSettings()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string fileId = "12345678";
        const string password = "password123";

        var fileNames = new List<string> { fileId + fileName };
        var passwords = new List<string> { fileId + password };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act - Remove password protection
        PasswordVerification.UpdatePassword(fileName);

        // Assert
        var isStillProtected = PasswordVerification.IsPasswordProtected(fileName);
        var retrievedId = IDVerification.GetIdFromFileNames(fileName);
        var storedFileNames = SettingsService.FileNames;
        var storedPasswords = SettingsService.Passwords;

        isStillProtected.Should().BeFalse();
        retrievedId.Should().Be("No files");
        storedFileNames.Should().BeEmpty();
        storedPasswords.Should().BeEmpty();
    }

    #endregion

    #region Recent Files Workflow

    [Fact]
    public void Workflow_OpenFile_ShouldAddToRecentItems()
    {
        // Arrange
        const string filePath = "/path/to/document.rtf";
        var recentItems = new List<string>();

        // Act - Simulate opening a file
        recentItems.Add(filePath);
        SettingsService.RecentItems = recentItems;

        // Assert
        var storedRecentItems = SettingsService.RecentItems;
        storedRecentItems.Should().Contain(filePath);
        storedRecentItems.Should().HaveCount(1);
    }

    [Fact]
    public void Workflow_OpenMultipleFiles_ShouldMaintainRecentList()
    {
        // Arrange
        var filePaths = new List<string>
        {
            "/path/to/file1.rtf",
            "/path/to/file2.rtf",
            "/path/to/file3.rtf"
        };

        // Act - Simulate opening multiple files
        var recentItems = new List<string>();
        foreach (var path in filePaths)
        {
            recentItems.Insert(0, path); // Insert at beginning for most recent
        }
        SettingsService.RecentItems = recentItems;

        // Assert
        var storedRecentItems = SettingsService.RecentItems;
        storedRecentItems.Should().HaveCount(3);
        storedRecentItems[0].Should().Be("/path/to/file3.rtf"); // Most recent
    }

    [Fact]
    public void Workflow_RecentItems_ShouldLimitTo10Items()
    {
        // Arrange
        var recentItems = new List<string>();
        for (int i = 1; i <= 15; i++)
        {
            recentItems.Insert(0, $"/path/to/file{i}.rtf");
        }

        // Act - Keep only the 10 most recent
        var limitedRecentItems = recentItems.Take(10).ToList();
        SettingsService.RecentItems = limitedRecentItems;

        // Assert
        var storedRecentItems = SettingsService.RecentItems;
        storedRecentItems.Should().HaveCount(10);
        storedRecentItems[0].Should().Be("/path/to/file15.rtf"); // Most recent
        storedRecentItems.Should().NotContain("/path/to/file1.rtf"); // Oldest dropped
    }

    #endregion

    #region RTF/HTML Conversion Workflow

    [Fact]
    public void Workflow_SaveAsRtf_ShouldConvertHtmlToRtf()
    {
        // Arrange
        const string htmlContent = "<p><strong>Bold text</strong> and <em>italic text</em></p>";

        // Act
        var rtfContent = RtfHtmlConverter.HtmlToRtf(htmlContent);
        var isRtf = RtfHtmlConverter.IsRtf(rtfContent);

        // Assert
        isRtf.Should().BeTrue();
        rtfContent.Should().Contain(@"\rtf");
        rtfContent.Should().Contain(@"\b "); // Bold marker
        rtfContent.Should().Contain(@"\i "); // Italic marker
    }

    [Fact]
    public void Workflow_LoadRtfFile_ShouldConvertToHtmlForEditor()
    {
        // Arrange
        const string rtfContent = @"{\rtf1\ansi\deff0 \b Bold text \i Italic text}";

        // Act
        var htmlContent = RtfHtmlConverter.RtfToHtml(rtfContent);
        var isHtml = RtfHtmlConverter.IsHtml(htmlContent);

        // Assert
        isHtml.Should().BeTrue();
        htmlContent.Should().Contain("<");
        htmlContent.Should().Contain(">");
    }

    [Fact]
    public void Workflow_RoundTripConversion_ShouldPreserveContent()
    {
        // Arrange
        const string originalText = "Test content with formatting";
        const string originalHtml = $"<p>{originalText}</p>";

        // Act - Convert HTML to RTF and back
        var rtf = RtfHtmlConverter.HtmlToRtf(originalHtml);
        var htmlBack = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        htmlBack.Should().Contain(originalText);
    }

    [Fact]
    public void Workflow_DetectFileFormat_ShouldIdentifyCorrectly()
    {
        // Arrange
        const string rtfContent = @"{\rtf1\ansi Test}";
        const string htmlContent = "<p>Test</p>";
        const string plainText = "Test";

        // Act
        var rtfDetected = RtfHtmlConverter.IsRtf(rtfContent);
        var htmlDetected = RtfHtmlConverter.IsHtml(htmlContent);
        var rtfNotHtml = RtfHtmlConverter.IsHtml(rtfContent);
        var htmlNotRtf = RtfHtmlConverter.IsRtf(htmlContent);
        var plainNotRtf = RtfHtmlConverter.IsRtf(plainText);
        var plainNotHtml = RtfHtmlConverter.IsHtml(plainText);

        // Assert
        rtfDetected.Should().BeTrue();
        htmlDetected.Should().BeTrue();
        rtfNotHtml.Should().BeFalse();
        htmlNotRtf.Should().BeFalse();
        plainNotRtf.Should().BeFalse();
        plainNotHtml.Should().BeFalse();
    }

    #endregion

    #region Multiple Files with Passwords Workflow

    [Fact]
    public void Workflow_MultipleFilesWithDifferentPasswords_ShouldMaintainSeparately()
    {
        // Arrange
        const string file1 = "document1.rtf";
        const string file2 = "document2.rtf";
        const string id1 = "11111111";
        const string id2 = "22222222";
        const string password1 = "password1";
        const string password2 = "password2";

        var fileNames = new List<string>
        {
            id1 + file1,
            id2 + file2
        };
        var passwords = new List<string>
        {
            id1 + password1,
            id2 + password2
        };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        var file1Protected = PasswordVerification.IsPasswordProtected(file1);
        var file2Protected = PasswordVerification.IsPasswordProtected(file2);
        var file1Id = IDVerification.GetIdFromFileNames(file1);
        var file2Id = IDVerification.GetIdFromFileNames(file2);
        var password1Verified = PasswordVerification.VerifyPassword(id1 + password1);
        var password2Verified = PasswordVerification.VerifyPassword(id2 + password2);

        // Assert
        file1Protected.Should().BeTrue();
        file2Protected.Should().BeTrue();
        file1Id.Should().Be(id1);
        file2Id.Should().Be(id2);
        password1Verified.Should().Be("Verified");
        password2Verified.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_RemoveOnePassword_ShouldNotAffectOthers()
    {
        // Arrange
        const string file1 = "document1.rtf";
        const string file2 = "document2.rtf";
        const string id1 = "11111111";
        const string id2 = "22222222";

        var fileNames = new List<string>
        {
            id1 + file1,
            id2 + file2
        };
        var passwords = new List<string>
        {
            id1 + "password1",
            id2 + "password2"
        };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act - Remove password from file1
        PasswordVerification.UpdatePassword(file1);

        // Assert
        var file1Protected = PasswordVerification.IsPasswordProtected(file1);
        var file2Protected = PasswordVerification.IsPasswordProtected(file2);
        var storedFileNames = SettingsService.FileNames;
        var storedPasswords = SettingsService.Passwords;

        file1Protected.Should().BeFalse();
        file2Protected.Should().BeTrue();
        storedFileNames.Should().NotContain(id1 + file1);
        storedFileNames.Should().Contain(id2 + file2);
        storedPasswords.Should().HaveCount(1);
    }

    #endregion

    #region Error Scenarios

    [Fact]
    public void Workflow_VerifyWrongPassword_ShouldReturnNotVerified()
    {
        // Arrange
        const string fileName = "document.rtf";
        const string fileId = "12345678";
        const string correctPassword = "correctpass";
        const string wrongPassword = "wrongpass";

        var fileNames = new List<string> { fileId + fileName };
        var passwords = new List<string> { fileId + correctPassword };

        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;

        // Act
        var wrongResult = PasswordVerification.VerifyPassword(fileId + wrongPassword);
        var correctResult = PasswordVerification.VerifyPassword(fileId + correctPassword);

        // Assert
        wrongResult.Should().Be("Not Verified");
        correctResult.Should().Be("Verified");
    }

    [Fact]
    public void Workflow_AccessNonExistentFile_ShouldReturnNoFiles()
    {
        // Arrange
        SettingsService.FileNames = new List<string>
        {
            "12345678existingfile.rtf"
        };

        // Act
        var result = IDVerification.GetIdFromFileNames("nonexistent.rtf");

        // Assert
        result.Should().Be("No files");
    }

    #endregion

    #region Cleanup

    public FileOperationsWorkflowTests()
    {
        // Clear all settings before each test
        SettingsService.RecentItems = new List<string>();
        SettingsService.FileNames = new List<string>();
        SettingsService.Passwords = new List<string>();
        SettingsService.ProductKeyEntered = false;
    }

    #endregion
}
