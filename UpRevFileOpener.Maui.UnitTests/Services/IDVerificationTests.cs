using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.UnitTests.Services;

/// <summary>
/// Unit tests for IDVerification service following BDD principles
/// </summary>
public class IDVerificationTests
{
    #region GetId Tests

    [Fact]
    public void GetId_WhenCalled_ShouldReturnEightDigitString()
    {
        // Act
        var result = IDVerification.GetId();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveLength(8);
        result.Should().MatchRegex(@"^\d{8}$", "ID should contain exactly 8 digits");
    }

    [Fact]
    public void GetId_WhenCalledMultipleTimes_ShouldReturnDifferentIds()
    {
        // Arrange
        var ids = new HashSet<string>();
        const int iterations = 100;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            ids.Add(IDVerification.GetId());
        }

        // Assert
        ids.Should().HaveCountGreaterThan(1, "Generated IDs should have some variation");
    }

    [Fact]
    public void GetId_WhenCalled_ShouldReturnZeroPaddedId()
    {
        // Act - Call multiple times to potentially get a small number
        var ids = new List<string>();
        for (int i = 0; i < 50; i++)
        {
            ids.Add(IDVerification.GetId());
        }

        // Assert
        ids.Should().AllSatisfy(id =>
        {
            id.Should().HaveLength(8);
            id.Should().MatchRegex(@"^\d{8}$");
        });
    }

    #endregion

    #region GetIdFromFileNames Tests

    [Fact]
    public void GetIdFromFileNames_WhenFileNameExists_ShouldReturnCorrectId()
    {
        // Arrange
        const string expectedId = "12345678";
        const string fileName = "testfile.txt";
        const string fileNameWithId = expectedId + fileName;

        var fileNames = new List<string> { fileNameWithId };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames(fileName);

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void GetIdFromFileNames_WhenMultipleFilesExist_ShouldReturnCorrectIdForMatchingFile()
    {
        // Arrange
        const string expectedId = "98765432";
        const string targetFileName = "document.rtf";

        var fileNames = new List<string>
        {
            "11111111otherfile.txt",
            "22222222anotherfile.doc",
            expectedId + targetFileName,
            "33333333yetanother.pdf"
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames(targetFileName);

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNameDoesNotExist_ShouldReturnNoFiles()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "12345678file1.txt",
            "87654321file2.doc"
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames("nonexistent.txt");

        // Assert
        result.Should().Be("No files");
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNamesListIsEmpty_ShouldReturnNoFiles()
    {
        // Arrange
        SettingsService.FileNames = new List<string>();

        // Act
        var result = IDVerification.GetIdFromFileNames("anyfile.txt");

        // Assert
        result.Should().Be("No files");
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNamesListIsNull_ShouldReturnNoFiles()
    {
        // Arrange
        SettingsService.FileNames = null!;

        // Act
        var result = IDVerification.GetIdFromFileNames("anyfile.txt");

        // Assert
        result.Should().Be("No files");
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNameWithIdIsTooShort_ShouldSkipThatEntry()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "short",           // Too short, will be skipped
            "1234567",         // Only 7 chars, will be skipped
            "12345678valid.txt" // Valid entry
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames("valid.txt");

        // Assert
        result.Should().Be("12345678");
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNameHasExactly8Chars_ShouldReturnNoFiles()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "12345678" // Exactly 8 characters, substring would be empty
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames("");

        // Assert
        result.Should().Be("No files");
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNameContainsSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        const string fileName = "my-file_name (1).txt";
        const string expectedId = "55555555";
        var fileNames = new List<string>
        {
            expectedId + fileName
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames(fileName);

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNameContainsSpaces_ShouldHandleCorrectly()
    {
        // Arrange
        const string fileName = "my file with spaces.doc";
        const string expectedId = "44444444";
        var fileNames = new List<string>
        {
            expectedId + fileName
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames(fileName);

        // Assert
        result.Should().Be(expectedId);
    }

    [Fact]
    public void GetIdFromFileNames_WhenMultipleFilesWithSimilarNames_ShouldMatchExactName()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "11111111file.txt",
            "22222222file.txt.bak",
            "33333333myfile.txt"
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames("file.txt");

        // Assert
        result.Should().Be("11111111");
    }

    [Fact]
    public void GetIdFromFileNames_WhenFileNameIsEmptyString_ShouldReturnNoFiles()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "12345678somefile.txt"
        };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames("");

        // Assert
        result.Should().Be("No files");
    }

    [Theory]
    [InlineData("document.rtf", "12345678document.rtf", "12345678")]
    [InlineData("test.txt", "99999999test.txt", "99999999")]
    [InlineData("file", "00000000file", "00000000")]
    public void GetIdFromFileNames_WithVariousValidInputs_ShouldReturnCorrectId(
        string fileName, string storedFileNameWithId, string expectedId)
    {
        // Arrange
        var fileNames = new List<string> { storedFileNameWithId };
        SettingsService.FileNames = fileNames;

        // Act
        var result = IDVerification.GetIdFromFileNames(fileName);

        // Assert
        result.Should().Be(expectedId);
    }

    #endregion

    #region Cleanup

    public IDVerificationTests()
    {
        // Clear settings before each test
        SettingsService.FileNames = new List<string>();
        SettingsService.Passwords = new List<string>();
    }

    #endregion
}
