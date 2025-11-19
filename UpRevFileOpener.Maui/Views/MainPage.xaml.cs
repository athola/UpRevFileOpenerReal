using UpRevFileOpener.Services;
using System.Text;

namespace UpRevFileOpener;

public partial class MainPage : ContentPage
{
    private string? _currentFileName;
    private string? _checkProtection;
    private string? _lastOpenedFile;
    private string? _id;
    private bool _isPasswordForOpen = true;

    public MainPage()
    {
        InitializeComponent();
        InitializeFontPickers();
        LoadRecentFiles();
    }

    private void InitializeFontPickers()
    {
        // Populate font families (simplified for cross-platform)
        var fontFamilies = new List<string>
        {
            "Arial", "Courier New", "Georgia", "Times New Roman",
            "Verdana", "Trebuchet MS", "Comic Sans MS"
        };
        comboFontFamily.ItemsSource = fontFamilies;
        comboFontFamily.SelectedIndex = 0;

        // Populate font sizes
        var fontSizes = new List<string>
        {
            "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72"
        };
        comboFontSize.ItemsSource = fontSizes;
        comboFontSize.SelectedIndex = 4; // Default to 12
    }

    private void LoadRecentFiles()
    {
        // TODO: Dynamically populate recent files menu
        // This requires custom menu item creation which is more complex in MAUI
    }

    private async void OnOpenFile(object sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".UpRev" } },
                    { DevicePlatform.macOS, new[] { "UpRev" } },
                    { DevicePlatform.iOS, new[] { "public.data" } },
                    { DevicePlatform.Android, new[] { "application/octet-stream" } }
                });

            var options = new PickOptions
            {
                PickerTitle = "Please select an UpRev file",
                FileTypes = customFileType
            };

            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                _lastOpenedFile = result.FullPath;
                CheckOpenFile(_lastOpenedFile);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error opening file: {ex.Message}", "OK");
        }
    }

    private void CheckOpenFile(string fileName)
    {
        _id = IDVerification.GetIdFromFileNames(fileName);
        if (_id != "No files")
        {
            ShowPasswordPrompt(true); // true = for opening
        }
        else
        {
            OpenFileActions(fileName);
        }
    }

    private async void OpenFileActions(string fileName)
    {
        await LoadFileIntoEditor(fileName);
        textEditor.IsReadOnly = true;
        editItem.IsEnabled = true;
        saveItem.IsEnabled = true;
        closeItem.IsEnabled = true;
        _currentFileName = fileName;
        UpdateRecentItems(fileName);

        // Disable formatting buttons when read-only
        btnBold.IsEnabled = false;
        btnItalic.IsEnabled = false;
        btnUnderline.IsEnabled = false;
        comboFontFamily.IsEnabled = false;
        comboFontSize.IsEnabled = false;
    }

    private async Task LoadFileIntoEditor(string fileName)
    {
        try
        {
            // Note: In the original WPF app, this loaded RTF format
            // In MAUI, we'll load as plain text or implement custom RTF parsing
            // For simplicity, loading as plain text here
            var text = await File.ReadAllTextAsync(fileName);
            textEditor.Text = text;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error loading file: {ex.Message}", "OK");
        }
    }

    private void UpdateRecentItems(string fileName)
    {
        var recentItems = SettingsService.RecentItems;

        if (recentItems.Count < 10 && !recentItems.Contains(fileName))
        {
            recentItems.Add(fileName);
        }
        else if (!recentItems.Contains(fileName))
        {
            recentItems.RemoveAt(0);
            recentItems.Add(fileName);
        }

        SettingsService.RecentItems = recentItems;
        LoadRecentFiles();
    }

    private void ShowPasswordPrompt(bool isForOpen)
    {
        _isPasswordForOpen = isForOpen;
        passwordPromptLabel.Text = isForOpen
            ? "Enter password to open file:"
            : "Enter password to protect file:";
        passwordEntry.Text = "";
        passwordOverlay.IsVisible = true;
        passwordEntry.Focus();
    }

    private async void OnPasswordOk(object sender, EventArgs e)
    {
        if (_isPasswordForOpen)
        {
            // Opening file with password
            if (!string.IsNullOrEmpty(_id))
            {
                string password = _id + passwordEntry.Text;
                _checkProtection = PasswordVerification.VerifyPassword(password);

                if (_checkProtection == "Verified" || _checkProtection == "Not protected")
                {
                    passwordOverlay.IsVisible = false;
                    if (!string.IsNullOrEmpty(_lastOpenedFile))
                    {
                        OpenFileActions(_lastOpenedFile);
                    }
                }
                else
                {
                    passwordOverlay.IsVisible = false;
                    await DisplayAlert("Error", "Incorrect password. Try again.", "OK");
                }
            }
        }
        else
        {
            // Saving file with password
            if (passwordEntry.Text?.Length < 6)
            {
                await DisplayAlert("Error", "Password must be 6 or more characters", "OK");
                passwordEntry.Text = "";
                return;
            }

            passwordOverlay.IsVisible = false;
            await SaveFileWithPassword();
        }
    }

    private void OnPasswordCancel(object sender, EventArgs e)
    {
        passwordOverlay.IsVisible = false;
        passwordEntry.Text = "";
    }

    private async void OnSaveFile(object sender, EventArgs e)
    {
        ShowPasswordPrompt(false); // false = for saving
    }

    private async Task SaveFileWithPassword()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Save UpRev File"
            });

            // Note: MAUI FilePicker doesn't have a true "Save" dialog
            // This is a limitation - on some platforms, you may need to use platform-specific code
            // For now, we'll use a workaround

            string? fileName = await DisplayPromptAsync("Save File", "Enter filename (without extension):");
            if (string.IsNullOrEmpty(fileName))
                return;

            // Get the app's document directory
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName + ".UpRev");

            // Save the file
            await File.WriteAllTextAsync(filePath, textEditor.Text);

            // Handle password protection
            if (PasswordVerification.IsPasswordProtected(filePath))
            {
                PasswordVerification.UpdatePassword(filePath);
            }

            string saveId = IDVerification.GetId();
            string newFileName = saveId + filePath;
            string newPassword = saveId + passwordEntry.Text;

            var fileNames = SettingsService.FileNames;
            var passwords = SettingsService.Passwords;

            fileNames.Add(newFileName);
            passwords.Add(newPassword);

            SettingsService.FileNames = fileNames;
            SettingsService.Passwords = passwords;

            await DisplayAlert("Success", $"File saved to: {filePath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error saving file: {ex.Message}", "OK");
        }
    }

    private void OnEditFile(object sender, EventArgs e)
    {
        textEditor.IsReadOnly = false;

        // Enable formatting buttons
        btnBold.IsEnabled = true;
        btnItalic.IsEnabled = true;
        btnUnderline.IsEnabled = true;
        comboFontFamily.IsEnabled = true;
        comboFontSize.IsEnabled = true;
    }

    private void OnCloseFile(object sender, EventArgs e)
    {
        textEditor.Text = "";
        textEditor.IsReadOnly = true;
        editItem.IsEnabled = false;
        saveItem.IsEnabled = false;
        closeItem.IsEnabled = false;
        _currentFileName = null;

        // Disable formatting buttons
        btnBold.IsEnabled = false;
        btnItalic.IsEnabled = false;
        btnUnderline.IsEnabled = false;
        comboFontFamily.IsEnabled = false;
        comboFontSize.IsEnabled = false;
    }

    private void OnExitApp(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }

    // Formatting button handlers
    // Note: MAUI Editor doesn't support rich text formatting like WPF RichTextBox
    // These are placeholders for potential future implementation with a rich text editor component
    private async void OnBoldClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Note", "Rich text formatting is not available in this version. Consider using a third-party rich text editor component.", "OK");
    }

    private async void OnItalicClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Note", "Rich text formatting is not available in this version. Consider using a third-party rich text editor component.", "OK");
    }

    private async void OnUnderlineClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Note", "Rich text formatting is not available in this version. Consider using a third-party rich text editor component.", "OK");
    }

    private void OnFontFamilyChanged(object sender, EventArgs e)
    {
        if (comboFontFamily.SelectedItem != null && !textEditor.IsReadOnly)
        {
            textEditor.FontFamily = comboFontFamily.SelectedItem.ToString();
        }
    }

    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        if (comboFontSize.SelectedItem != null && !textEditor.IsReadOnly)
        {
            if (double.TryParse(comboFontSize.SelectedItem.ToString(), out double size))
            {
                textEditor.FontSize = size;
            }
        }
    }
}
