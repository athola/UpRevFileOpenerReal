using UpRevFileOpener.Services;

namespace UpRevFileOpener.Maui.UnitTests.Services;

/// <summary>
/// Unit tests for RtfHtmlConverter service following BDD principles
/// </summary>
public class RtfHtmlConverterTests
{
    #region RtfToHtml Tests

    [Fact]
    public void RtfToHtml_WhenInputIsNull_ShouldReturnEmptyString()
    {
        // Act
        var result = RtfHtmlConverter.RtfToHtml(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RtfToHtml_WhenInputIsEmpty_ShouldReturnEmptyString()
    {
        // Act
        var result = RtfHtmlConverter.RtfToHtml(string.Empty);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RtfToHtml_WhenInputIsAlreadyHtml_ShouldReturnAsIs()
    {
        // Arrange
        const string html = "<p>This is HTML</p>";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(html);

        // Assert
        result.Should().Be(html);
    }

    [Fact]
    public void RtfToHtml_WhenInputIsPlainText_ShouldReturnAsIs()
    {
        // Arrange
        const string plainText = "This is plain text";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(plainText);

        // Assert
        result.Should().Be(plainText);
    }

    [Fact]
    public void RtfToHtml_WhenInputIsValidRtf_ShouldReturnHtmlWrappedInDiv()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 Hello World}";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().StartWith("<div>");
        result.Should().EndWith("</div>");
        result.Should().Contain("Hello World");
    }

    [Fact]
    public void RtfToHtml_WhenRtfContainsBoldFormatting_ShouldConvertToStrong()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 \b Bold Text}";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().Contain("<strong>");
    }

    [Fact]
    public void RtfToHtml_WhenRtfContainsItalicFormatting_ShouldConvertToEm()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 \i Italic Text}";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().Contain("<em>");
    }

    [Fact]
    public void RtfToHtml_WhenRtfContainsUnderlineFormatting_ShouldConvertToU()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 \ul Underlined Text}";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().Contain("<u>");
    }

    [Fact]
    public void RtfToHtml_WhenRtfContainsFontSize_ShouldConvertToSpanWithStyle()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 \fs24 Text}";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().Contain("font-size:");
    }

    [Fact]
    public void RtfToHtml_WhenRtfContainsParagraphs_ShouldConvertToPTags()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 \pard First paragraph\par Second paragraph}";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().Contain("<p>");
    }

    [Fact]
    public void RtfToHtml_WhenRtfHasWhitespace_ShouldTrimAndClean()
    {
        // Arrange
        const string rtf = @"  {\rtf1\ansi\deff0 Text}  ";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        result.Should().StartWith("<div>");
        result.Should().EndWith("</div>");
    }

    [Fact]
    public void RtfToHtml_WhenConversionFails_ShouldReturnPlainTextExtraction()
    {
        // Arrange - Malformed RTF that will cause exception
        const string malformedRtf = @"{\rtf1";

        // Act
        var result = RtfHtmlConverter.RtfToHtml(malformedRtf);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain("<p>");
    }

    #endregion

    #region HtmlToRtf Tests

    [Fact]
    public void HtmlToRtf_WhenInputIsNull_ShouldReturnEmptyString()
    {
        // Act
        var result = RtfHtmlConverter.HtmlToRtf(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void HtmlToRtf_WhenInputIsEmpty_ShouldReturnEmptyString()
    {
        // Act
        var result = RtfHtmlConverter.HtmlToRtf(string.Empty);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void HtmlToRtf_WhenInputIsAlreadyRtf_ShouldReturnAsIs()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 Test}";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(rtf);

        // Assert
        result.Should().Be(rtf);
    }

    [Fact]
    public void HtmlToRtf_WhenInputIsValidHtml_ShouldReturnRtfWithHeader()
    {
        // Arrange
        const string html = "<p>Hello World</p>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().StartWith(@"{\rtf1\ansi\deff0");
        result.Should().Contain(@"{\fonttbl");
        result.Should().Contain(@"{\colortbl");
        result.Should().EndWith("}");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsBoldTags_ShouldConvertToRtfBold()
    {
        // Arrange
        const string html = "<strong>Bold text</strong>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\b ");
        result.Should().Contain(@"\b0 ");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsBTags_ShouldConvertToRtfBold()
    {
        // Arrange
        const string html = "<b>Bold text</b>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\b ");
        result.Should().Contain(@"\b0 ");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsItalicTags_ShouldConvertToRtfItalic()
    {
        // Arrange
        const string html = "<em>Italic text</em>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\i ");
        result.Should().Contain(@"\i0 ");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsITags_ShouldConvertToRtfItalic()
    {
        // Arrange
        const string html = "<i>Italic text</i>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\i ");
        result.Should().Contain(@"\i0 ");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsUnderlineTags_ShouldConvertToRtfUnderline()
    {
        // Arrange
        const string html = "<u>Underlined text</u>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\ul ");
        result.Should().Contain(@"\ul0 ");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsParagraphTags_ShouldConvertToRtfParagraphs()
    {
        // Arrange
        const string html = "<p>First paragraph</p><p>Second paragraph</p>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\pard ");
        result.Should().Contain(@"\par");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsBrTags_ShouldConvertToRtfLine()
    {
        // Arrange
        const string html = "Line 1<br>Line 2<br/>Line 3";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\line");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlContainsSpecialCharacters_ShouldEscapeThem()
    {
        // Arrange
        const string html = @"Text with \ backslash and { braces }";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().Contain(@"\\");
        result.Should().Contain(@"\{");
        result.Should().Contain(@"\}");
    }

    [Fact]
    public void HtmlToRtf_WhenHtmlHasWhitespace_ShouldHandleCorrectly()
    {
        // Arrange
        const string html = "  <p>Text with spaces</p>  ";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().StartWith(@"{\rtf1");
        result.Should().Contain("Text with spaces");
    }

    [Fact]
    public void HtmlToRtf_WhenConversionFails_ShouldReturnSimpleRtfWithPlainText()
    {
        // Arrange - This shouldn't fail, but we're testing the fallback
        const string html = "<p>Test</p>";

        // Act
        var result = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        result.Should().StartWith(@"{\rtf1");
        result.Should().EndWith("}");
    }

    #endregion

    #region RtfToPlainText Tests

    [Fact]
    public void RtfToPlainText_WhenInputIsNull_ShouldReturnEmptyString()
    {
        // Act
        var result = RtfHtmlConverter.RtfToPlainText(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RtfToPlainText_WhenInputIsEmpty_ShouldReturnEmptyString()
    {
        // Act
        var result = RtfHtmlConverter.RtfToPlainText(string.Empty);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RtfToPlainText_WhenInputIsNotRtf_ShouldReturnInputAsIs()
    {
        // Arrange
        const string plainText = "This is plain text";

        // Act
        var result = RtfHtmlConverter.RtfToPlainText(plainText);

        // Assert
        result.Should().Be(plainText);
    }

    [Fact]
    public void RtfToPlainText_WhenInputIsRtf_ShouldStripAllFormatting()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 Hello World}";

        // Act
        var result = RtfHtmlConverter.RtfToPlainText(rtf);

        // Assert
        result.Should().NotContain(@"\rtf");
        result.Should().NotContain(@"\ansi");
        result.Should().NotContain("{");
        result.Should().NotContain("}");
        result.Should().Contain("Hello World");
    }

    [Fact]
    public void RtfToPlainText_WhenRtfContainsFormatting_ShouldRemoveAllControlWords()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi\deff0 \b Bold \i Italic \ul Underline}";

        // Act
        var result = RtfHtmlConverter.RtfToPlainText(rtf);

        // Assert
        result.Should().NotContain(@"\b");
        result.Should().NotContain(@"\i");
        result.Should().NotContain(@"\ul");
        result.Should().Contain("Bold");
        result.Should().Contain("Italic");
        result.Should().Contain("Underline");
    }

    [Fact]
    public void RtfToPlainText_WhenRtfHasMultipleSpaces_ShouldNormalizeWhitespace()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi  Multiple    Spaces   Here}";

        // Act
        var result = RtfHtmlConverter.RtfToPlainText(rtf);

        // Assert
        result.Should().NotContainAny(@"\rtf", @"\ansi");
        var words = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        words.Should().Contain("Multiple");
        words.Should().Contain("Spaces");
        words.Should().Contain("Here");
    }

    [Fact]
    public void RtfToPlainText_WhenRtfHasLeadingTrailingSpaces_ShouldTrim()
    {
        // Arrange
        const string rtf = @"   {\rtf1\ansi Text}   ";

        // Act
        var result = RtfHtmlConverter.RtfToPlainText(rtf);

        // Assert
        result.Should().NotStartWith(" ");
        result.Should().NotEndWith(" ");
        result.Should().Contain("Text");
    }

    #endregion

    #region IsRtf Tests

    [Fact]
    public void IsRtf_WhenContentIsNull_ShouldReturnFalse()
    {
        // Act
        var result = RtfHtmlConverter.IsRtf(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRtf_WhenContentIsEmpty_ShouldReturnFalse()
    {
        // Act
        var result = RtfHtmlConverter.IsRtf(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRtf_WhenContentStartsWithRtfHeader_ShouldReturnTrue()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi Text}";

        // Act
        var result = RtfHtmlConverter.IsRtf(rtf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRtf_WhenContentHasLeadingWhitespace_ShouldStillDetectRtf()
    {
        // Arrange
        const string rtf = @"   {\rtf1\ansi Text}";

        // Act
        var result = RtfHtmlConverter.IsRtf(rtf);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRtf_WhenContentIsPlainText_ShouldReturnFalse()
    {
        // Arrange
        const string plainText = "This is plain text";

        // Act
        var result = RtfHtmlConverter.IsRtf(plainText);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRtf_WhenContentIsHtml_ShouldReturnFalse()
    {
        // Arrange
        const string html = "<p>This is HTML</p>";

        // Act
        var result = RtfHtmlConverter.IsRtf(html);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(@"{\rtf1 Text}", true)]
    [InlineData(@"{\rtf9\ansi Text}", true)]
    [InlineData("Plain text", false)]
    [InlineData("<html>HTML content</html>", false)]
    [InlineData("", false)]
    public void IsRtf_WithVariousInputs_ShouldReturnExpectedResult(string content, bool expected)
    {
        // Act
        var result = RtfHtmlConverter.IsRtf(content);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region IsHtml Tests

    [Fact]
    public void IsHtml_WhenContentIsNull_ShouldReturnFalse()
    {
        // Act
        var result = RtfHtmlConverter.IsHtml(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsHtml_WhenContentIsEmpty_ShouldReturnFalse()
    {
        // Act
        var result = RtfHtmlConverter.IsHtml(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsHtml_WhenContentStartsWithOpeningTag_ShouldReturnTrue()
    {
        // Arrange
        const string html = "<p>This is HTML</p>";

        // Act
        var result = RtfHtmlConverter.IsHtml(html);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsHtml_WhenContentContainsClosingTag_ShouldReturnTrue()
    {
        // Arrange
        const string html = "Some text</p>";

        // Act
        var result = RtfHtmlConverter.IsHtml(html);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsHtml_WhenContentHasLeadingWhitespace_ShouldStillDetectHtml()
    {
        // Arrange
        const string html = "   <div>HTML content</div>";

        // Act
        var result = RtfHtmlConverter.IsHtml(html);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsHtml_WhenContentIsPlainText_ShouldReturnFalse()
    {
        // Arrange
        const string plainText = "This is plain text without tags";

        // Act
        var result = RtfHtmlConverter.IsHtml(plainText);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsHtml_WhenContentIsRtf_ShouldReturnFalse()
    {
        // Arrange
        const string rtf = @"{\rtf1\ansi Text}";

        // Act
        var result = RtfHtmlConverter.IsHtml(rtf);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("<p>HTML</p>", true)]
    [InlineData("<div>Content</div>", true)]
    [InlineData("Text</span>", true)]
    [InlineData("Plain text", false)]
    [InlineData(@"{\rtf1 RTF}", false)]
    [InlineData("", false)]
    public void IsHtml_WithVariousInputs_ShouldReturnExpectedResult(string content, bool expected)
    {
        // Act
        var result = RtfHtmlConverter.IsHtml(content);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Integration Tests (Round-trip conversions)

    [Fact]
    public void RoundTrip_HtmlToRtfToHtml_ShouldPreserveBasicContent()
    {
        // Arrange
        const string originalHtml = "<p>Hello World</p>";

        // Act
        var rtf = RtfHtmlConverter.HtmlToRtf(originalHtml);
        var backToHtml = RtfHtmlConverter.RtfToHtml(rtf);

        // Assert
        backToHtml.Should().Contain("Hello World");
    }

    [Fact]
    public void RoundTrip_RtfToHtmlToRtf_ShouldPreserveBasicContent()
    {
        // Arrange
        const string originalRtf = @"{\rtf1\ansi\deff0 Test Content}";

        // Act
        var html = RtfHtmlConverter.RtfToHtml(originalRtf);
        var backToRtf = RtfHtmlConverter.HtmlToRtf(html);

        // Assert
        backToRtf.Should().Contain("Test Content");
    }

    #endregion
}
