namespace UpRevFileOpener.Services;

/// <summary>
/// Default file save service implementation for platforms that don't have native save dialogs
/// Uses a simple prompt for filename
/// </summary>
public class DefaultFileSaveService : IFileSaveService
{
    public async Task<string?> SaveFileAsync(string suggestedFileName, string fileExtension = ".UpRev")
    {
        // Use DisplayPromptAsync through the main page or app shell
        var page = Application.Current?.MainPage;
        if (page == null)
            return null;

        string? fileName = await page.DisplayPromptAsync(
            "Save File",
            "Enter filename (without extension):",
            initialValue: suggestedFileName);

        if (string.IsNullOrEmpty(fileName))
            return null;

        // Determine save location based on platform
#if ANDROID || IOS
        // Mobile platforms use app data directory
        return Path.Combine(FileSystem.AppDataDirectory, fileName + fileExtension);
#else
        // Other platforms try to use Documents folder
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var uprevFolder = Path.Combine(documentsPath, "UpRev Files");
        Directory.CreateDirectory(uprevFolder);
        return Path.Combine(uprevFolder, fileName + fileExtension);
#endif
    }
}
