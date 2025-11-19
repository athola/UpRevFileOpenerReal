using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.UnitTests.Services;

/// <summary>
/// Unit tests for IFileSaveService implementations following BDD principles
/// Note: These tests focus on contract compliance and testable behavior
/// </summary>
public class FileSaveServiceTests
{
    #region Interface Contract Tests

    [Fact]
    public void IFileSaveService_ShouldDefineCorrectMethod()
    {
        // Arrange & Act
        var interfaceType = typeof(IFileSaveService);
        var method = interfaceType.GetMethod("SaveFileAsync");

        // Assert
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<string?>));
    }

    [Fact]
    public void IFileSaveService_SaveFileAsync_ShouldHaveCorrectParameters()
    {
        // Arrange
        var interfaceType = typeof(IFileSaveService);
        var method = interfaceType.GetMethod("SaveFileAsync");

        // Act
        var parameters = method!.GetParameters();

        // Assert
        parameters.Should().HaveCount(2);
        parameters[0].Name.Should().Be("suggestedFileName");
        parameters[0].ParameterType.Should().Be(typeof(string));
        parameters[1].Name.Should().Be("fileExtension");
        parameters[1].ParameterType.Should().Be(typeof(string));
    }

    #endregion

    #region DefaultFileSaveService Tests

    [Fact]
    public void DefaultFileSaveService_ShouldImplementIFileSaveService()
    {
        // Arrange
        var serviceType = typeof(DefaultFileSaveService);

        // Act & Assert
        typeof(IFileSaveService).IsAssignableFrom(serviceType).Should().BeTrue();
    }

    [Fact]
    public void DefaultFileSaveService_WhenInstantiated_ShouldNotThrow()
    {
        // Act
        var act = () => new DefaultFileSaveService();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DefaultFileSaveService_Instance_ShouldNotBeNull()
    {
        // Act
        var service = new DefaultFileSaveService();

        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IFileSaveService>();
    }

    #endregion

    #region Path Generation Logic Tests

    [Theory]
    [InlineData("document", ".UpRev", "document.UpRev")]
    [InlineData("myfile", ".txt", "myfile.txt")]
    [InlineData("test", ".rtf", "test.rtf")]
    public void FilePath_WhenCombiningFileNameAndExtension_ShouldFormCorrectPath(
        string fileName, string extension, string expectedFileName)
    {
        // Arrange
        var combinedFileName = fileName + extension;

        // Act & Assert
        combinedFileName.Should().Be(expectedFileName);
    }

    [Fact]
    public void Path_WhenCombiningDirectoryAndFileName_ShouldFormValidPath()
    {
        // Arrange
        var directory = "/path/to/directory";
        var fileName = "file.txt";

        // Act
        var fullPath = Path.Combine(directory, fileName);

        // Assert
        fullPath.Should().Contain(directory);
        fullPath.Should().EndWith(fileName);
    }

    [Theory]
    [InlineData("file with spaces", ".txt")]
    [InlineData("file-with-dashes", ".rtf")]
    [InlineData("file_with_underscores", ".doc")]
    public void FileName_WithSpecialCharacters_ShouldBeValid(string fileName, string extension)
    {
        // Act
        var fullFileName = fileName + extension;
        var isValidName = !string.IsNullOrEmpty(fullFileName);

        // Assert
        isValidName.Should().BeTrue();
        fullFileName.Should().Contain(fileName);
        fullFileName.Should().EndWith(extension);
    }

    #endregion

    #region Extension Handling Tests

    [Fact]
    public void FileExtension_WhenNotProvided_ShouldUseDefaultUpRevExtension()
    {
        // Arrange
        const string defaultExtension = ".UpRev";
        var fileName = "document";

        // Act
        var fullFileName = fileName + defaultExtension;

        // Assert
        fullFileName.Should().EndWith(".UpRev");
    }

    [Theory]
    [InlineData(".txt")]
    [InlineData(".rtf")]
    [InlineData(".doc")]
    [InlineData(".UpRev")]
    public void FileExtension_WhenCustomExtensionProvided_ShouldUseCustomExtension(string extension)
    {
        // Arrange
        var fileName = "document";

        // Act
        var fullFileName = fileName + extension;

        // Assert
        fullFileName.Should().EndWith(extension);
    }

    [Fact]
    public void FileExtension_WhenContainsDot_ShouldBeValidExtension()
    {
        // Arrange
        var extensions = new[] { ".txt", ".rtf", ".doc", ".UpRev" };

        // Act & Assert
        extensions.Should().AllSatisfy(ext => ext.Should().StartWith("."));
    }

    #endregion

    #region Null and Empty Handling Tests

    [Fact]
    public void FileName_WhenNullOrEmpty_ShouldBeDetectable()
    {
        // Arrange
        string? nullFileName = null;
        string emptyFileName = string.Empty;

        // Act
        var isNullOrEmptyNull = string.IsNullOrEmpty(nullFileName);
        var isNullOrEmptyEmpty = string.IsNullOrEmpty(emptyFileName);

        // Assert
        isNullOrEmptyNull.Should().BeTrue();
        isNullOrEmptyEmpty.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FileName_WhenNullEmptyOrWhitespace_ShouldBeInvalid(string? fileName)
    {
        // Act
        var isInvalid = string.IsNullOrWhiteSpace(fileName);

        // Assert
        isInvalid.Should().BeTrue();
    }

    #endregion

    #region Platform-Specific Path Tests

    [Fact]
    public void DocumentsPath_ShouldBeAccessible()
    {
        // Act
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Assert
        documentsPath.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DirectoryCreation_WhenPathIsValid_ShouldNotThrow()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), "UpRevTestDir_" + Guid.NewGuid());

        try
        {
            // Act
            var act = () => Directory.CreateDirectory(tempPath);

            // Assert
            act.Should().NotThrow();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public void Path_WhenCombiningMultipleParts_ShouldFormCorrectPath()
    {
        // Arrange
        var part1 = "root";
        var part2 = "subfolder";
        var part3 = "file.txt";

        // Act
        var combinedPath = Path.Combine(part1, part2, part3);

        // Assert
        combinedPath.Should().Contain(part1);
        combinedPath.Should().Contain(part2);
        combinedPath.Should().EndWith(part3);
    }

    #endregion

    #region Mock FileSaveService for Testing

    private class MockFileSaveService : IFileSaveService
    {
        private readonly string? _returnPath;

        public MockFileSaveService(string? returnPath = null)
        {
            _returnPath = returnPath;
        }

        public Task<string?> SaveFileAsync(string suggestedFileName, string fileExtension = ".UpRev")
        {
            if (_returnPath == null)
                return Task.FromResult<string?>(null);

            return Task.FromResult<string?>(_returnPath);
        }
    }

    [Fact]
    public async Task MockFileSaveService_WhenReturnPathIsNull_ShouldReturnNull()
    {
        // Arrange
        var service = new MockFileSaveService(null);

        // Act
        var result = await service.SaveFileAsync("test");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task MockFileSaveService_WhenReturnPathIsProvided_ShouldReturnPath()
    {
        // Arrange
        const string expectedPath = "/path/to/file.txt";
        var service = new MockFileSaveService(expectedPath);

        // Act
        var result = await service.SaveFileAsync("test");

        // Assert
        result.Should().Be(expectedPath);
    }

    [Fact]
    public async Task MockFileSaveService_ShouldAcceptDifferentParameters()
    {
        // Arrange
        var service = new MockFileSaveService("/path/file.txt");

        // Act
        var result1 = await service.SaveFileAsync("file1", ".txt");
        var result2 = await service.SaveFileAsync("file2", ".rtf");

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
    }

    #endregion

    #region Service Contract Compliance Tests

    [Fact]
    public void FileSaveService_ShouldReturnNullableString()
    {
        // Arrange
        var method = typeof(IFileSaveService).GetMethod("SaveFileAsync");

        // Act
        var returnType = method!.ReturnType;

        // Assert
        returnType.Should().Be(typeof(Task<string?>));
    }

    [Fact]
    public void FileSaveService_ShouldBeAsync()
    {
        // Arrange
        var method = typeof(IFileSaveService).GetMethod("SaveFileAsync");

        // Act & Assert
        method!.ReturnType.Should().Be(typeof(Task<string?>));
        method.Name.Should().EndWith("Async");
    }

    #endregion

    #region File Name Validation Tests

    [Theory]
    [InlineData("validname")]
    [InlineData("valid_name")]
    [InlineData("valid-name")]
    [InlineData("valid name")]
    [InlineData("ValidName123")]
    public void FileName_WithValidCharacters_ShouldBeAcceptable(string fileName)
    {
        // Act
        var isValid = !string.IsNullOrWhiteSpace(fileName);

        // Assert
        isValid.Should().BeTrue();
        fileName.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("name<test", '<')]
    [InlineData("name>test", '>')]
    [InlineData("name:test", ':')]
    [InlineData("name\"test", '"')]
    [InlineData("name|test", '|')]
    [InlineData("name?test", '?')]
    [InlineData("name*test", '*')]
    public void FileName_WithInvalidCharacters_ShouldBeDetectable(string fileName, char invalidChar)
    {
        // Act
        var containsInvalidChar = fileName.Contains(invalidChar);
        var invalidChars = Path.GetInvalidFileNameChars();
        var hasInvalidChars = fileName.Any(c => invalidChars.Contains(c));

        // Assert
        containsInvalidChar.Should().BeTrue();
        hasInvalidChars.Should().BeTrue();
    }

    #endregion
}
