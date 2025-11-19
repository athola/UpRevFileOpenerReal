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
    private bool _isReadOnly = true;
    private bool _editorReady = false;
    private readonly IFileSaveService _fileSaveService;

    public MainPage(IFileSaveService fileSaveService)
    {
        _fileSaveService = fileSaveService;
        InitializeComponent();
        InitializeFontPickers();
        LoadRecentFiles();
        InitializeEditor();
    }

    private async void InitializeEditor()
    {
        try
        {
            // Load the rich text editor HTML
            var htmlSource = new HtmlWebViewSource();

            // Read the editor HTML file
            using var stream = await FileSystem.OpenAppPackageFileAsync("editor.html");
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            htmlSource.Html = html;
            richTextEditor.Source = htmlSource;

            // Set up WebView navigation handlers
            richTextEditor.Navigated += OnEditorNavigated;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load rich text editor: {ex.Message}", "OK");
        }
    }

    private void OnEditorNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (e.Result == WebNavigationResult.Success)
        {
            _editorReady = true;
            // Set initial read-only state
            SetEditorReadOnly(true);
        }
    }

    private void InitializeFontPickers()
    {
        // Populate font families (matching Quill.js fonts)
        var fontFamilies = new List<string>
        {
            "Arial", "Courier", "Georgia", "Times", "Verdana"
        };
        comboFontFamily.ItemsSource = fontFamilies;
        comboFontFamily.SelectedIndex = 0;

        // Populate font sizes
        var fontSizes = new List<string>
        {
            "8", "9", "10", "11", "12", "14", "16", "18", "20", "24"
        };
        comboFontSize.ItemsSource = fontSizes;
        comboFontSize.SelectedIndex = 4; // Default to 12
    }

    private void LoadRecentFiles()
    {
        try
        {
            // Clear existing recent file items
            recentFilesMenu.Clear();

            var recentItems = SettingsService.RecentItems;

            if (recentItems == null || recentItems.Count == 0)
            {
                // Add a disabled "No recent files" item
                var noFilesItem = new MenuFlyoutItem
                {
                    Text = "No recent files",
                    IsEnabled = false
                };
                recentFilesMenu.Add(noFilesItem);
                return;
            }

            // Add recent files in reverse order (most recent first)
            for (int i = recentItems.Count - 1; i >= 0; i--)
            {
                var filePath = recentItems[i];
                var fileName = Path.GetFileName(filePath);

                var menuItem = new MenuFlyoutItem
                {
                    Text = $"{recentItems.Count - i}. {fileName}",
                    CommandParameter = filePath
                };

                menuItem.Clicked += OnRecentFileClicked;
                recentFilesMenu.Add(menuItem);
            }

            // Add separator and clear history option
            recentFilesMenu.Add(new MenuFlyoutSeparator());

            var clearHistoryItem = new MenuFlyoutItem
            {
                Text = "Clear Recent Files"
            };
            clearHistoryItem.Clicked += OnClearRecentFiles;
            recentFilesMenu.Add(clearHistoryItem);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recent files: {ex.Message}");
        }
    }

    private void OnRecentFileClicked(object? sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem menuItem && menuItem.CommandParameter is string filePath)
        {
            _lastOpenedFile = filePath;
            CheckOpenFile(_lastOpenedFile);
        }
    }

    private async void OnClearRecentFiles(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Clear Recent Files",
            "Are you sure you want to clear all recent files?",
            "Yes",
            "No");

        if (confirm)
        {
            SettingsService.RecentItems = new List<string>();
            LoadRecentFiles();
        }
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
        SetEditorReadOnly(true);
        _isReadOnly = true;
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
            if (!_editorReady)
            {
                await DisplayAlert("Error", "Editor is not ready yet. Please try again.", "OK");
                return;
            }

            // Read the file content
            var content = await File.ReadAllTextAsync(fileName);

            // Convert RTF to HTML if necessary
            string htmlContent;
            if (RtfHtmlConverter.IsRtf(content))
            {
                htmlContent = RtfHtmlConverter.RtfToHtml(content);
            }
            else if (RtfHtmlConverter.IsHtml(content))
            {
                htmlContent = content;
            }
            else
            {
                // Plain text - wrap in paragraph
                htmlContent = $"<p>{content.Replace("\n", "</p><p>")}</p>";
            }

            // Escape quotes for JavaScript
            var escapedHtml = htmlContent
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "")
                .Replace("\n", "\\n");

            // Set content in editor
            await richTextEditor.EvaluateJavaScriptAsync($"setContent('{escapedHtml}');");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error loading file: {ex.Message}", "OK");
        }
    }

    private async Task<string> GetEditorContent()
    {
        if (!_editorReady)
            return string.Empty;

        try
        {
            var html = await richTextEditor.EvaluateJavaScriptAsync("getContent();");
            return html?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async void SetEditorReadOnly(bool isReadOnly)
    {
        if (!_editorReady)
            return;

        try
        {
            await richTextEditor.EvaluateJavaScriptAsync($"setReadOnly({(isReadOnly ? "true" : "false")});");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting read-only: {ex.Message}");
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
            // Get content from editor
            var htmlContent = await GetEditorContent();

            if (string.IsNullOrEmpty(htmlContent))
            {
                await DisplayAlert("Error", "No content to save", "OK");
                return;
            }

            // Convert HTML to RTF
            var rtfContent = RtfHtmlConverter.HtmlToRtf(htmlContent);

            // Use platform-specific file save dialog
            string suggestedFileName = _currentFileName != null
                ? Path.GetFileNameWithoutExtension(_currentFileName)
                : "Untitled";

            string? filePath = await _fileSaveService.SaveFileAsync(suggestedFileName, ".UpRev");

            if (string.IsNullOrEmpty(filePath))
                return; // User cancelled

            // Save the file
            await File.WriteAllTextAsync(filePath, rtfContent);

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

            _currentFileName = filePath;
            await DisplayAlert("Success", $"File saved to: {filePath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error saving file: {ex.Message}", "OK");
        }
    }

    private void OnEditFile(object sender, EventArgs e)
    {
        SetEditorReadOnly(false);
        _isReadOnly = false;

        // Enable formatting buttons
        btnBold.IsEnabled = true;
        btnItalic.IsEnabled = true;
        btnUnderline.IsEnabled = true;
        comboFontFamily.IsEnabled = true;
        comboFontSize.IsEnabled = true;
    }

    private async void OnCloseFile(object sender, EventArgs e)
    {
        if (_editorReady)
        {
            await richTextEditor.EvaluateJavaScriptAsync("clear();");
        }

        SetEditorReadOnly(true);
        _isReadOnly = true;
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
    private async void OnBoldClicked(object sender, EventArgs e)
    {
        if (_editorReady && !_isReadOnly)
        {
            await richTextEditor.EvaluateJavaScriptAsync("applyBold();");
        }
    }

    private async void OnItalicClicked(object sender, EventArgs e)
    {
        if (_editorReady && !_isReadOnly)
        {
            await richTextEditor.EvaluateJavaScriptAsync("applyItalic();");
        }
    }

    private async void OnUnderlineClicked(object sender, EventArgs e)
    {
        if (_editorReady && !_isReadOnly)
        {
            await richTextEditor.EvaluateJavaScriptAsync("applyUnderline();");
        }
    }

    private async void OnFontFamilyChanged(object sender, EventArgs e)
    {
        if (comboFontFamily.SelectedItem != null && !_isReadOnly && _editorReady)
        {
            var font = comboFontFamily.SelectedItem.ToString()!.ToLower();
            await richTextEditor.EvaluateJavaScriptAsync($"setFontFamily('{font}');");
        }
    }

    private async void OnFontSizeChanged(object sender, EventArgs e)
    {
        if (comboFontSize.SelectedItem != null && !_isReadOnly && _editorReady)
        {
            var size = comboFontSize.SelectedItem.ToString();
            await richTextEditor.EvaluateJavaScriptAsync($"setFontSize('{size}px');");
        }
    }
}
