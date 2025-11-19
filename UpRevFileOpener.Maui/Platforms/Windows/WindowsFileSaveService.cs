using Windows.Storage;
using Windows.Storage.Pickers;

namespace UpRevFileOpener.Services;

/// <summary>
/// Windows-specific implementation of file save service using WinUI FileSavePicker
/// </summary>
public class WindowsFileSaveService : IFileSaveService
{
    public async Task<string?> SaveFileAsync(string suggestedFileName, string fileExtension = ".UpRev")
    {
        try
        {
            var savePicker = new FileSavePicker();

            // Get the current window handle for the file picker
            var window = ((MauiWinUIWindow)Application.Current!.Windows[0].Handler.PlatformView!);
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            // Set up the file type choices
            savePicker.FileTypeChoices.Add("UpRev Files", new List<string> { fileExtension });
            savePicker.SuggestedFileName = suggestedFileName;

            // Default to Documents folder
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Show the save picker
            StorageFile? file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                return file.Path;
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing save picker: {ex.Message}");
            return null;
        }
    }
}
