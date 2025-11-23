using System.Text;
using System.Text.RegularExpressions;

namespace UpRevFileOpener.Services;

/// <summary>
/// Service to convert between RTF and HTML formats
/// Note: This is a simplified converter. For production use, consider using a library like RtfPipe
/// </summary>
public static class RtfHtmlConverter
{
    /// <summary>
    /// Convert RTF to HTML (simplified conversion)
    /// </summary>
    public static string RtfToHtml(string rtf)
    {
        if (string.IsNullOrEmpty(rtf))
            return string.Empty;

        // If it's already HTML or plain text, return as-is
        if (!rtf.TrimStart().StartsWith("{\\rtf"))
        {
            return rtf;
        }

        try
        {
            // This is a very basic RTF parser
            // For production, consider using: https://github.com/erdomke/RtfPipe
            var html = new StringBuilder();
            html.Append("<div>");

            // Remove RTF header and control words
            var text = Regex.Replace(rtf, @"\\rtf\d+", "");
            text = Regex.Replace(text, @"\\ansi", "");
            text = Regex.Replace(text, @"\\deff\d+", "");
            text = Regex.Replace(text, @"\\fonttbl[^}]*}", "");
            text = Regex.Replace(text, @"\\colortbl[^}]*}", "");

            // Convert basic formatting
            text = ConvertBasicFormatting(text);

            // Remove remaining control words
            text = Regex.Replace(text, @"\\[a-z]+\d*\s?", "");

            // Remove braces
            text = text.Replace("{", "").Replace("}", "");

            // Clean up whitespace
            text = text.Trim();

            html.Append(text);
            html.Append("</div>");

            return html.ToString();
        }
        catch (Exception)
        {
            // If conversion fails, return plain text extraction
            return ExtractPlainTextFromRtf(rtf);
        }
    }

    /// <summary>
    /// Convert HTML to RTF (simplified conversion)
    /// </summary>
    public static string HtmlToRtf(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // If it's already RTF, return as-is
        if (html.TrimStart().StartsWith("{\\rtf"))
        {
            return html;
        }

        try
        {
            var rtf = new StringBuilder();

            // RTF header
            rtf.AppendLine(@"{\rtf1\ansi\deff0");

            // Font table
            rtf.AppendLine(@"{\fonttbl{\f0\fswiss Arial;}{\f1\fmodern Courier New;}{\f2\froman Times New Roman;}}");

            // Color table
            rtf.AppendLine(@"{\colortbl;\red0\green0\blue0;\red255\green0\blue0;\red0\green255\blue0;\red0\green0\blue255;}");

            // Convert HTML to RTF
            var content = ConvertHtmlToRtfContent(html);
            rtf.Append(content);

            // Close RTF
            rtf.AppendLine("}");

            return rtf.ToString();
        }
        catch (Exception)
        {
            // If conversion fails, create a simple RTF with the HTML as plain text
            var plainText = StripHtmlTags(html);
            return $"{{\\rtf1\\ansi\\deff0{{\\fonttbl{{\\f0\\fswiss Arial;}}}}\\f0\\fs24 {plainText}}}";
        }
    }

    private static string ConvertBasicFormatting(string rtf)
    {
        // Bold
        rtf = Regex.Replace(rtf, @"\\b\s+([^\\}]+)", "<strong>$1</strong>");

        // Italic
        rtf = Regex.Replace(rtf, @"\\i\s+([^\\}]+)", "<em>$1</em>");

        // Underline
        rtf = Regex.Replace(rtf, @"\\ul\s+([^\\}]+)", "<u>$1</u>");

        // Font size (fs20 = 10pt, fs24 = 12pt, etc - RTF uses half-points)
        rtf = Regex.Replace(rtf, @"\\fs(\d+)\s+", m =>
        {
            var halfPoints = int.Parse(m.Groups[1].Value);
            var points = halfPoints / 2;
            return $"<span style='font-size:{points}px'>";
        });

        // Paragraphs
        rtf = rtf.Replace(@"\par", "</p><p>");
        rtf = rtf.Replace(@"\pard", "<p>");

        return rtf;
    }

    private static string ConvertHtmlToRtfContent(string html)
    {
        var content = html;

        // Remove HTML tags but preserve formatting
        content = Regex.Replace(content, @"<strong>|<b>", @"\b ");
        content = Regex.Replace(content, @"</strong>|</b>", @"\b0 ");

        content = Regex.Replace(content, @"<em>|<i>", @"\i ");
        content = Regex.Replace(content, @"</em>|</i>", @"\i0 ");

        content = Regex.Replace(content, @"<u>", @"\ul ");
        content = Regex.Replace(content, @"</u>", @"\ul0 ");

        // Paragraphs
        content = Regex.Replace(content, @"<p>", @"\pard ");
        content = Regex.Replace(content, @"</p>", @"\par" + Environment.NewLine);
        content = Regex.Replace(content, @"<br\s*/?>", @"\line" + Environment.NewLine);

        // Remove remaining HTML tags
        content = Regex.Replace(content, @"<[^>]+>", "");

        // Escape special RTF characters
        content = content.Replace("\\", "\\\\");
        content = content.Replace("{", "\\{");
        content = content.Replace("}", "\\}");

        return content;
    }

    private static string ExtractPlainTextFromRtf(string rtf)
    {
        // Remove all RTF control words and groups
        var text = Regex.Replace(rtf, @"\\[a-z]+\d*\s?", "");
        text = text.Replace("{", "").Replace("}", "");
        text = text.Trim();

        // Wrap in simple HTML
        return $"<p>{text}</p>";
    }

    private static string StripHtmlTags(string html)
    {
        return Regex.Replace(html, @"<[^>]+>", "");
    }

    /// <summary>
    /// Extract plain text from RTF content
    /// </summary>
    public static string RtfToPlainText(string rtf)
    {
        if (string.IsNullOrEmpty(rtf))
            return string.Empty;

        if (!rtf.TrimStart().StartsWith("{\\rtf"))
            return rtf;

        // Remove all RTF control words and formatting
        var text = Regex.Replace(rtf, @"\\[a-z]+\d*\s?", " ");
        text = text.Replace("{", "").Replace("}", "");
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }

    /// <summary>
    /// Check if content is RTF format
    /// </summary>
    public static bool IsRtf(string content)
    {
        return !string.IsNullOrEmpty(content) && content.TrimStart().StartsWith("{\\rtf");
    }

    /// <summary>
    /// Check if content is HTML format
    /// </summary>
    public static bool IsHtml(string content)
    {
        return !string.IsNullOrEmpty(content) &&
               (content.TrimStart().StartsWith("<") || content.Contains("</"));
    }
}
