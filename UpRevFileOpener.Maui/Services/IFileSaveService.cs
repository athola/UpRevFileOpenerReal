namespace UpRevFileOpener.Services;

/// <summary>
/// Interface for platform-specific file save operations
/// </summary>
public interface IFileSaveService
{
    /// <summary>
    /// Show a file save dialog and return the selected file path
    /// </summary>
    Task<string?> SaveFileAsync(string suggestedFileName, string fileExtension = ".UpRev");
}
