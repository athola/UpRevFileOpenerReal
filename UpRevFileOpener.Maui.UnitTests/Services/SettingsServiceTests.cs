using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.UnitTests.Services;

/// <summary>
/// Unit tests for SettingsService following BDD principles
/// Note: These tests validate the service behavior and JSON serialization logic
/// </summary>
public class SettingsServiceTests
{
    #region RecentItems Tests

    [Fact]
    public void RecentItems_WhenSetAndGet_ShouldPersistCorrectly()
    {
        // Arrange
        var expectedItems = new List<string>
        {
            "file1.txt",
            "file2.rtf",
            "file3.doc"
        };

        // Act
        SettingsService.RecentItems = expectedItems;
        var actualItems = SettingsService.RecentItems;

        // Assert
        actualItems.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public void RecentItems_WhenSetToEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        SettingsService.RecentItems = emptyList;
        var result = SettingsService.RecentItems;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RecentItems_WhenSetWithMultipleItems_ShouldMaintainOrder()
    {
        // Arrange
        var items = new List<string> { "first.txt", "second.txt", "third.txt" };

        // Act
        SettingsService.RecentItems = items;
        var result = SettingsService.RecentItems;

        // Assert
        result.Should().ContainInOrder(items);
    }

    [Fact]
    public void RecentItems_WhenSetWithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var items = new List<string>
        {
            "file with spaces.txt",
            "file-with-dashes.doc",
            "file_with_underscores.rtf",
            "file(with)parentheses.pdf"
        };

        // Act
        SettingsService.RecentItems = items;
        var result = SettingsService.RecentItems;

        // Assert
        result.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void RecentItems_WhenAccessedMultipleTimes_ShouldReturnConsistentData()
    {
        // Arrange
        var items = new List<string> { "file1.txt", "file2.txt" };
        SettingsService.RecentItems = items;

        // Act
        var firstAccess = SettingsService.RecentItems;
        var secondAccess = SettingsService.RecentItems;

        // Assert
        firstAccess.Should().BeEquivalentTo(secondAccess);
    }

    #endregion

    #region Passwords Tests

    [Fact]
    public void Passwords_WhenSetAndGet_ShouldPersistCorrectly()
    {
        // Arrange
        var expectedPasswords = new List<string>
        {
            "12345678password1",
            "87654321password2",
            "11111111password3"
        };

        // Act
        SettingsService.Passwords = expectedPasswords;
        var actualPasswords = SettingsService.Passwords;

        // Assert
        actualPasswords.Should().BeEquivalentTo(expectedPasswords);
    }

    [Fact]
    public void Passwords_WhenSetToEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        SettingsService.Passwords = emptyList;
        var result = SettingsService.Passwords;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Passwords_WhenSetWithMultipleEntries_ShouldMaintainOrder()
    {
        // Arrange
        var passwords = new List<string>
        {
            "11111111first",
            "22222222second",
            "33333333third"
        };

        // Act
        SettingsService.Passwords = passwords;
        var result = SettingsService.Passwords;

        // Assert
        result.Should().ContainInOrder(passwords);
    }

    [Fact]
    public void Passwords_WhenUpdated_ShouldReflectChanges()
    {
        // Arrange
        var initialPasswords = new List<string> { "12345678pass1" };
        SettingsService.Passwords = initialPasswords;

        var updatedPasswords = new List<string> { "12345678pass1", "87654321pass2" };

        // Act
        SettingsService.Passwords = updatedPasswords;
        var result = SettingsService.Passwords;

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("87654321pass2");
    }

    #endregion

    #region FileNames Tests

    [Fact]
    public void FileNames_WhenSetAndGet_ShouldPersistCorrectly()
    {
        // Arrange
        var expectedFileNames = new List<string>
        {
            "12345678file1.txt",
            "87654321file2.rtf",
            "11111111file3.doc"
        };

        // Act
        SettingsService.FileNames = expectedFileNames;
        var actualFileNames = SettingsService.FileNames;

        // Assert
        actualFileNames.Should().BeEquivalentTo(expectedFileNames);
    }

    [Fact]
    public void FileNames_WhenSetToEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        SettingsService.FileNames = emptyList;
        var result = SettingsService.FileNames;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FileNames_WhenSetWithMultipleEntries_ShouldMaintainOrder()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "11111111first.txt",
            "22222222second.txt",
            "33333333third.txt"
        };

        // Act
        SettingsService.FileNames = fileNames;
        var result = SettingsService.FileNames;

        // Assert
        result.Should().ContainInOrder(fileNames);
    }

    [Fact]
    public void FileNames_WhenContainsSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var fileNames = new List<string>
        {
            "12345678file with spaces.txt",
            "87654321file-with-dashes.doc"
        };

        // Act
        SettingsService.FileNames = fileNames;
        var result = SettingsService.FileNames;

        // Assert
        result.Should().BeEquivalentTo(fileNames);
    }

    #endregion

    #region ProductKeyEntered Tests

    [Fact]
    public void ProductKeyEntered_WhenSetToTrue_ShouldReturnTrue()
    {
        // Act
        SettingsService.ProductKeyEntered = true;
        var result = SettingsService.ProductKeyEntered;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ProductKeyEntered_WhenSetToFalse_ShouldReturnFalse()
    {
        // Act
        SettingsService.ProductKeyEntered = false;
        var result = SettingsService.ProductKeyEntered;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ProductKeyEntered_WhenToggledMultipleTimes_ShouldReflectLatestValue()
    {
        // Act
        SettingsService.ProductKeyEntered = true;
        var firstResult = SettingsService.ProductKeyEntered;

        SettingsService.ProductKeyEntered = false;
        var secondResult = SettingsService.ProductKeyEntered;

        SettingsService.ProductKeyEntered = true;
        var thirdResult = SettingsService.ProductKeyEntered;

        // Assert
        firstResult.Should().BeTrue();
        secondResult.Should().BeFalse();
        thirdResult.Should().BeTrue();
    }

    #endregion

    #region Save and Reload Tests

    [Fact]
    public void Save_WhenCalled_ShouldNotThrowException()
    {
        // Act
        var act = () => SettingsService.Save();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Reload_WhenCalled_ShouldNotThrowException()
    {
        // Act
        var act = () => SettingsService.Reload();

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region List Management Scenarios

    [Fact]
    public void RecentItems_WhenAddingItems_ShouldAcceptUpTo10Items()
    {
        // Arrange
        var items = new List<string>();
        for (int i = 1; i <= 10; i++)
        {
            items.Add($"file{i}.txt");
        }

        // Act
        SettingsService.RecentItems = items;
        var result = SettingsService.RecentItems;

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public void FileNames_WhenModifyingList_ShouldUpdateCorrectly()
    {
        // Arrange
        var initial = new List<string> { "12345678file1.txt", "87654321file2.txt" };
        SettingsService.FileNames = initial;

        var retrieved = SettingsService.FileNames;
        retrieved.Add("11111111file3.txt");

        // Act
        SettingsService.FileNames = retrieved;
        var updated = SettingsService.FileNames;

        // Assert
        updated.Should().HaveCount(3);
        updated.Should().Contain("11111111file3.txt");
    }

    [Fact]
    public void Passwords_WhenRemovingItems_ShouldUpdateCorrectly()
    {
        // Arrange
        var initial = new List<string>
        {
            "12345678pass1",
            "87654321pass2",
            "11111111pass3"
        };
        SettingsService.Passwords = initial;

        var retrieved = SettingsService.Passwords;
        retrieved.Remove("87654321pass2");

        // Act
        SettingsService.Passwords = retrieved;
        var updated = SettingsService.Passwords;

        // Assert
        updated.Should().HaveCount(2);
        updated.Should().NotContain("87654321pass2");
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void RecentItems_WhenSetWithDuplicates_ShouldStoreDuplicates()
    {
        // Arrange
        var items = new List<string> { "file.txt", "file.txt", "other.txt" };

        // Act
        SettingsService.RecentItems = items;
        var result = SettingsService.RecentItems;

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public void FileNames_WhenSetWithVeryLongStrings_ShouldHandleCorrectly()
    {
        // Arrange
        var longFileName = "12345678" + new string('a', 500) + ".txt";
        var fileNames = new List<string> { longFileName };

        // Act
        SettingsService.FileNames = fileNames;
        var result = SettingsService.FileNames;

        // Assert
        result.Should().Contain(longFileName);
    }

    [Fact]
    public void Passwords_WhenSetWithUnicodeCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var passwords = new List<string>
        {
            "12345678пароль",  // Cyrillic
            "87654321密码",     // Chinese
            "11111111パスワード" // Japanese
        };

        // Act
        SettingsService.Passwords = passwords;
        var result = SettingsService.Passwords;

        // Assert
        result.Should().BeEquivalentTo(passwords);
    }

    #endregion

    #region Cleanup

    public SettingsServiceTests()
    {
        // Clear all settings before each test
        SettingsService.RecentItems = new List<string>();
        SettingsService.Passwords = new List<string>();
        SettingsService.FileNames = new List<string>();
        SettingsService.ProductKeyEntered = false;
    }

    #endregion
}
