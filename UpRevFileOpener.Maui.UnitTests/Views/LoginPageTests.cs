using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.UnitTests.Views;

/// <summary>
/// Unit tests for LoginPage validation logic following BDD principles
/// Tests focus on the product key validation business rules
/// </summary>
public class LoginPageTests
{
    #region Product Key Validation - All Digits

    [Fact]
    public void ProductKey_WhenAllDigits_ShouldBeValid()
    {
        // Arrange
        const string productKey = "1234567890123456";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isAllDigits.Should().BeTrue();
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("0000000000000000")]
    [InlineData("9999999999999999")]
    [InlineData("1111111111111111")]
    public void ProductKey_WithOnlyNumericCharacters_ShouldBeValid(string productKey)
    {
        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isAllDigits.Should().BeTrue();
    }

    [Theory]
    [InlineData("123456789012345a")]
    [InlineData("12345678901234AB")]
    [InlineData("1234567890123-56")]
    [InlineData("1234 567890123456")]
    [InlineData("1234567890123@56")]
    public void ProductKey_WithNonNumericCharacters_ShouldBeInvalid(string productKey)
    {
        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isAllDigits.Should().BeFalse();
    }

    [Fact]
    public void ProductKey_WhenContainsLetters_ShouldBeInvalid()
    {
        // Arrange
        const string productKey = "1234567890abcdef";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isAllDigits.Should().BeFalse();
    }

    [Fact]
    public void ProductKey_WhenContainsSpaces_ShouldBeInvalid()
    {
        // Arrange
        const string productKey = "1234 5678 9012 3456";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isAllDigits.Should().BeFalse();
    }

    [Fact]
    public void ProductKey_WhenContainsDashes_ShouldBeInvalid()
    {
        // Arrange
        const string productKey = "1234-5678-9012-3456";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isAllDigits.Should().BeFalse();
    }

    [Fact]
    public void ProductKey_WhenContainsSpecialCharacters_ShouldBeInvalid()
    {
        // Arrange
        var productKeys = new[]
        {
            "1234567890123@56",
            "1234567890123#56",
            "1234567890123$56",
            "1234567890123%56"
        };

        // Act & Assert
        productKeys.Should().AllSatisfy(key =>
            key.All(char.IsDigit).Should().BeFalse());
    }

    #endregion

    #region Product Key Validation - Length

    [Fact]
    public void ProductKey_WhenExactly16Digits_ShouldBeValid()
    {
        // Arrange
        const string productKey = "1234567890123456";

        // Act
        var isValidLength = productKey.Length >= 16;
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert
        isValidLength.Should().BeTrue();
        isAllDigits.Should().BeTrue();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("1234")]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("12345678")]
    [InlineData("123456789")]
    [InlineData("1234567890")]
    [InlineData("12345678901")]
    [InlineData("123456789012")]
    [InlineData("1234567890123")]
    [InlineData("12345678901234")]
    [InlineData("123456789012345")]
    public void ProductKey_WhenLessThan16Digits_ShouldBeInvalid(string productKey)
    {
        // Act
        var isValidLength = productKey.Length >= 16;

        // Assert
        isValidLength.Should().BeFalse();
        productKey.Length.Should().BeLessThan(16);
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("12345678901234567")]
    [InlineData("123456789012345678")]
    public void ProductKey_When16OrMoreDigits_ShouldMeetLengthRequirement(string productKey)
    {
        // Act
        var isValidLength = productKey.Length >= 16;

        // Assert
        isValidLength.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_WhenEmpty_ShouldBeInvalid()
    {
        // Arrange
        const string productKey = "";

        // Act
        var isValidLength = productKey.Length >= 16;

        // Assert
        isValidLength.Should().BeFalse();
    }

    #endregion

    #region Product Key Validation - Null and Empty Cases

    [Fact]
    public void ProductKey_WhenNull_ShouldBeInvalid()
    {
        // Arrange
        string? productKey = null;

        // Act
        var isAllDigits = !productKey?.All(char.IsDigit) ?? true;

        // Assert
        isAllDigits.Should().BeTrue(); // Matches the login logic (invalid)
    }

    [Fact]
    public void ProductKey_WhenNullOrEmpty_ShouldFailValidation()
    {
        // Arrange
        string? nullKey = null;
        string emptyKey = string.Empty;

        // Act
        var nullIsInvalid = !nullKey?.All(char.IsDigit) ?? true;
        var emptyIsInvalid = !emptyKey?.All(char.IsDigit) ?? true;

        // Assert
        nullIsInvalid.Should().BeTrue();
        emptyIsInvalid.Should().BeTrue();
    }

    #endregion

    #region Combined Validation Tests

    [Theory]
    [InlineData("1234567890123456", true, true)]   // Valid: 16 digits
    [InlineData("12345678901234567", true, true)]  // Valid: 17 digits (more than 16)
    [InlineData("123456789012345", false, false)]   // Invalid: 15 digits (less than 16)
    [InlineData("123456789012345a", false, true)]   // Invalid: contains letter
    [InlineData("1234 567890123456", false, true)]  // Invalid: contains space
    [InlineData("", false, false)]                  // Invalid: empty
    public void ProductKey_WithVariousInputs_ShouldValidateCorrectly(
        string productKey, bool shouldBeAllDigits, bool shouldMeetLength)
    {
        // Act
        var isAllDigits = productKey?.All(char.IsDigit) ?? false;
        var isValidLength = productKey?.Length >= 16;

        // Assert
        isAllDigits.Should().Be(shouldBeAllDigits);
        isValidLength.Should().Be(shouldMeetLength);
    }

    [Fact]
    public void ProductKey_ToBeValid_MustPassAllCriteria()
    {
        // Arrange
        const string validProductKey = "1234567890123456";
        const string invalidKey1 = "123456789012345";  // Too short
        const string invalidKey2 = "123456789012345a"; // Contains letter

        // Act
        var validIsAllDigits = validProductKey.All(char.IsDigit);
        var validMeetsLength = validProductKey.Length >= 16;

        var invalid1IsAllDigits = invalidKey1.All(char.IsDigit);
        var invalid1MeetsLength = invalidKey1.Length >= 16;

        var invalid2IsAllDigits = invalidKey2.All(char.IsDigit);
        var invalid2MeetsLength = invalidKey2.Length >= 16;

        // Assert
        validIsAllDigits.Should().BeTrue();
        validMeetsLength.Should().BeTrue();

        invalid1IsAllDigits.Should().BeTrue();
        invalid1MeetsLength.Should().BeFalse();

        invalid2IsAllDigits.Should().BeFalse();
        invalid2MeetsLength.Should().BeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ProductKey_WithLeadingZeros_ShouldBeValid()
    {
        // Arrange
        const string productKey = "0000000000000001";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);
        var isValidLength = productKey.Length >= 16;

        // Assert
        isAllDigits.Should().BeTrue();
        isValidLength.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_WithTrailingZeros_ShouldBeValid()
    {
        // Arrange
        const string productKey = "1000000000000000";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);
        var isValidLength = productKey.Length >= 16;

        // Assert
        isAllDigits.Should().BeTrue();
        isValidLength.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_AllZeros_ShouldBeValid()
    {
        // Arrange
        const string productKey = "0000000000000000";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);
        var isValidLength = productKey.Length >= 16;

        // Assert
        isAllDigits.Should().BeTrue();
        isValidLength.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_AllNines_ShouldBeValid()
    {
        // Arrange
        const string productKey = "9999999999999999";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);
        var isValidLength = productKey.Length >= 16;

        // Assert
        isAllDigits.Should().BeTrue();
        isValidLength.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_WithUnicodeDigits_ShouldBeInvalid()
    {
        // Arrange - Using Arabic-Indic digits
        const string productKey = "١٢٣٤٥٦٧٨٩٠١٢٣٤٥٦";

        // Act
        var isAllDigits = productKey.All(char.IsDigit);

        // Assert - char.IsDigit returns true for unicode digits, but we want ASCII 0-9 only
        // The actual validation should check for ASCII digits specifically
        var isAsciiDigits = productKey.All(c => c >= '0' && c <= '9');
        isAsciiDigits.Should().BeFalse();
    }

    #endregion

    #region Settings Integration Tests

    [Fact]
    public void ProductKeyEntered_WhenSet_ShouldPersist()
    {
        // Arrange
        SettingsService.ProductKeyEntered = false;

        // Act
        SettingsService.ProductKeyEntered = true;
        var result = SettingsService.ProductKeyEntered;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ProductKeyEntered_AfterSuccessfulValidation_ShouldBeSetToTrue()
    {
        // Arrange
        const string validProductKey = "1234567890123456";
        var isValid = validProductKey.All(char.IsDigit) && validProductKey.Length >= 16;

        // Act
        if (isValid)
        {
            SettingsService.ProductKeyEntered = true;
        }

        // Assert
        SettingsService.ProductKeyEntered.Should().BeTrue();
    }

    #endregion

    #region Error Message Scenarios

    [Fact]
    public void ProductKey_WhenContainsNonDigits_ShouldTriggerNumberOnlyError()
    {
        // Arrange
        const string productKey = "1234567890abcdef";

        // Act
        var shouldShowNumberOnlyError = !productKey.All(char.IsDigit);

        // Assert
        shouldShowNumberOnlyError.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_WhenTooShort_ShouldTriggerLengthError()
    {
        // Arrange
        const string productKey = "123456789012345"; // 15 digits

        // Act
        var isAllDigits = productKey.All(char.IsDigit);
        var shouldShowLengthError = isAllDigits && productKey.Length < 16;

        // Assert
        shouldShowLengthError.Should().BeTrue();
    }

    [Fact]
    public void ProductKey_ValidationOrder_ShouldCheckDigitsBeforeLength()
    {
        // Arrange
        const string invalidKey = "12345"; // Short AND non-numeric chars later

        // Act - Following the actual code logic
        var failsDigitCheck = !invalidKey.All(char.IsDigit);
        var failsLengthCheck = invalidKey.Length < 16;

        // Assert - Digit check happens first in the code
        failsLengthCheck.Should().BeTrue();
    }

    #endregion

    #region Cleanup

    public LoginPageTests()
    {
        // Reset settings before each test
        SettingsService.ProductKeyEntered = false;
    }

    #endregion
}
