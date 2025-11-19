# UpRev File Opener - Migration to .NET MAUI

## Overview

This document describes the migration of the UpRev File Opener application from WPF (.NET Framework 4.8) to .NET MAUI (.NET 8.0), enabling cross-platform support for Windows, macOS, Linux, iOS, and Android.

## Migration Date

November 2025

## Projects in Solution

1. **UpRevFileOpener** (Original)
   - Framework: WPF on .NET Framework 4.8
   - Platform: Windows only
   - Status: Maintained for backward compatibility

2. **UpRevFileOpener.Maui** (New)
   - Framework: .NET MAUI on .NET 8.0
   - Platforms: Windows, macOS, iOS, Android, Linux (experimental)
   - Status: Primary development branch

## Key Changes

### 1. Project Structure

#### Original (WPF)
```
UpRevFileOpener/
├── MainWindow.xaml
├── Login.xaml
├── App.xaml
├── IDVerification.cs
├── PasswordVerification.cs
├── Properties/
└── Images/
```

#### New (MAUI)
```
UpRevFileOpener.Maui/
├── Platforms/          # Platform-specific code
├── Resources/          # Unified resources
├── Services/           # Business logic
├── Views/              # UI pages
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs
```

### 2. Framework Migrations

| Component | WPF | MAUI |
|-----------|-----|------|
| UI Framework | System.Windows | Microsoft.Maui.Controls |
| Settings | Properties.Settings | Preferences API |
| File Dialogs | OpenFileDialog/SaveFileDialog | FilePicker |
| Rich Text | RichTextBox | Editor (plain text) |
| Windows | Window class | ContentPage/Shell |
| Popups | Popup control | Grid overlay |

### 3. Code Changes

#### Settings Migration

**Before (WPF):**
```csharp
Properties.Settings.Default.RecentItems.Add(fileName);
Properties.Settings.Default.Save();
Properties.Settings.Default.Reload();
```

**After (MAUI):**
```csharp
var recentItems = SettingsService.RecentItems;
recentItems.Add(fileName);
SettingsService.RecentItems = recentItems;
```

**Implementation:** Settings are now stored as JSON in `Preferences` with a wrapper service (`SettingsService.cs`).

#### File Operations

**Before (WPF):**
```csharp
OpenFileDialog openFile = new OpenFileDialog();
openFile.Filter = "UpRev Files|*.UpRev";
if (openFile.ShowDialog() == true)
{
    string fileName = openFile.FileName;
}
```

**After (MAUI):**
```csharp
var customFileType = new FilePickerFileType(
    new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".UpRev" } },
        // ... other platforms
    });
var result = await FilePicker.PickAsync(new PickOptions { FileTypes = customFileType });
if (result != null)
{
    string fileName = result.FullPath;
}
```

#### UI Structure

**Before (WPF):**
```xml
<Window x:Class="UpRevFileOpener.MainWindow">
    <DockPanel>
        <Menu DockPanel.Dock="Top">
            <MenuItem Header="_File">
                <MenuItem Header="_Open" Click="openFile" />
            </MenuItem>
        </Menu>
        <RichTextBox Name="rtbEditor" />
    </DockPanel>
</Window>
```

**After (MAUI):**
```xml
<ContentPage x:Class="UpRevFileOpener.MainPage">
    <ContentPage.MenuBarItems>
        <MenuBarItem Text="File">
            <MenuFlyoutItem Text="Open" Clicked="OnOpenFile" />
        </MenuBarItem>
    </ContentPage.MenuBarItems>
    <Editor x:Name="textEditor" />
</ContentPage>
```

### 4. Business Logic Migration

The core business logic classes (`IDVerification` and `PasswordVerification`) were migrated with minimal changes:

- Made static methods follow PascalCase naming convention (C# standards)
- Updated to use `SettingsService` instead of `Properties.Settings`
- Added null safety checks for cross-platform robustness

### 5. Known Limitations and Workarounds

#### Rich Text Editing

**Limitation:** MAUI doesn't have a built-in rich text editor comparable to WPF's RichTextBox.

**Current Solution:** Using plain text `Editor` control.

**Recommended Solutions:**
1. Integrate third-party component (Syncfusion, DevExpress)
2. Use WebView with HTML editor (e.g., TinyMCE, Quill)
3. Implement custom renderer for platform-specific rich text controls

**Example WebView approach:**
```csharp
<WebView x:Name="htmlEditor" />
```
```csharp
string html = @"
<!DOCTYPE html>
<html>
<head>
    <script src='https://cdn.tiny.cloud/1/YOUR-API-KEY/tinymce/5/tinymce.min.js'></script>
    <script>tinymce.init({ selector: 'textarea' });</script>
</head>
<body>
    <textarea id='editor'></textarea>
</body>
</html>";
htmlEditor.Source = new HtmlWebViewSource { Html = html };
```

#### File Save Dialog

**Limitation:** MAUI's FilePicker doesn't provide a "Save As" dialog.

**Current Solution:** Prompt for filename and save to app data directory.

**Recommended Solutions:**
```csharp
#if WINDOWS
// Use Windows-specific file save dialog
var savePicker = new Windows.Storage.Pickers.FileSavePicker();
// ... configure and show
#elif ANDROID
// Use Android Storage Access Framework
// ... implementation
#elif IOS || MACCATALYST
// Use iOS document picker
// ... implementation
#endif
```

#### Recent Files Menu

**Limitation:** Dynamic menu items are complex in MAUI.

**Current Solution:** Recent files stored in preferences but not displayed in menu.

**Recommended Solutions:**
1. Create dedicated Recent Files page
2. Use CollectionView with file list
3. Implement custom popup/bottom sheet

## Platform-Specific Considerations

### Windows
- Fully supported with MenuBar
- File operations work as expected
- Consider using WinUI 3 controls for advanced features

### macOS (Mac Catalyst)
- Menu bar integrates with system menu
- File picker uses native macOS dialogs
- App sandboxing may require entitlements for file access

### iOS/iPadOS
- No menu bar on phone (use toolbar)
- File picker uses iOS document picker
- Consider iOS-specific UI patterns (tab bar, navigation bar)

### Android
- Menu bar shown as toolbar/action bar
- File picker uses system file browser
- Handle storage permissions properly
- Consider Material Design guidelines

### Linux (Community)
- Experimental support via community projects
- GTK or Avalonia backends
- Limited testing and support

## Building and Testing

### Prerequisites
```bash
# Install .NET 8.0 SDK
# Install MAUI workload
dotnet workload install maui

# For specific platforms, also install:
# - Visual Studio 2022 17.8+ (Windows, iOS, Android)
# - Xcode (macOS, iOS)
# - Android SDK (Android)
```

### Build Commands

```bash
# Windows
dotnet build -f net8.0-windows10.0.19041.0

# macOS
dotnet build -f net8.0-maccatalyst

# iOS
dotnet build -f net8.0-ios

# Android
dotnet build -f net8.0-android

# Build all targets
dotnet build
```

### Run Commands

```bash
# Windows
dotnet run -f net8.0-windows10.0.19041.0

# Android (requires emulator or device)
dotnet run -f net8.0-android

# iOS (requires simulator or device, macOS only)
dotnet run -f net8.0-ios
```

## Migration Checklist

- [x] Create MAUI project structure
- [x] Migrate business logic (IDVerification, PasswordVerification)
- [x] Create SettingsService for preferences
- [x] Convert Login page to MAUI
- [x] Convert MainWindow to MainPage
- [x] Copy and configure resources (images, icons)
- [x] Create platform-specific files
- [x] Update solution file
- [x] Create documentation
- [ ] Add OpenSans fonts
- [ ] Test on Windows
- [ ] Test on macOS
- [ ] Test on Android
- [ ] Test on iOS
- [ ] Implement proper rich text editing
- [ ] Implement platform-specific file save dialogs
- [ ] Add recent files UI
- [ ] Enhance password encryption
- [ ] Add unit tests
- [ ] Create CI/CD pipeline

## Testing Strategy

### Unit Tests
```csharp
[Fact]
public void IDVerification_GetId_Returns8Digits()
{
    var id = IDVerification.GetId();
    Assert.Equal(8, id.Length);
    Assert.True(int.TryParse(id, out _));
}
```

### UI Tests
Consider using:
- Appium for cross-platform UI testing
- Platform-specific frameworks (XCTest, Espresso)

### Manual Testing Checklist

For each platform:
- [ ] App launches successfully
- [ ] Product key validation works
- [ ] File open works
- [ ] Password protection works
- [ ] File save works
- [ ] Recent files are stored
- [ ] Text editing works
- [ ] Font selection works
- [ ] App settings persist

## Performance Considerations

1. **Startup Time**
   - MAUI apps may have longer startup on first launch
   - Consider using splash screen effectively
   - Lazy-load heavy components

2. **Memory Usage**
   - Monitor memory on mobile devices
   - Dispose of file streams properly
   - Use weak references for cached data

3. **File I/O**
   - Async operations for file loading
   - Consider streaming for large files
   - Cache file metadata

## Security Enhancements

### Current System
- Simple ID-based password obfuscation
- Passwords stored in preferences (JSON)

### Recommended Improvements

```csharp
using System.Security.Cryptography;
using Microsoft.Maui.Storage;

public static class SecurePasswordService
{
    public static async Task<string> HashPasswordAsync(string password)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            saltSize: 16,
            iterations: 10000,
            HashAlgorithmName.SHA256);

        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    public static async Task StorePasswordSecurelyAsync(string key, string password)
    {
        await SecureStorage.SetAsync(key, password);
    }

    public static async Task<string> GetPasswordSecurelyAsync(string key)
    {
        return await SecureStorage.GetAsync(key);
    }
}
```

## Future Roadmap

### Phase 1: Core Functionality (Current)
- ✅ Basic MAUI migration
- ✅ Login and authentication
- ✅ File open/save (basic)
- ✅ Settings migration

### Phase 2: Enhanced Editing
- [ ] Integrate rich text editor
- [ ] Add formatting toolbar
- [ ] Support RTF import/export
- [ ] Implement undo/redo

### Phase 3: Platform Optimization
- [ ] Platform-specific file dialogs
- [ ] iOS/Android UI adaptations
- [ ] Tablet-optimized layouts
- [ ] Keyboard shortcuts

### Phase 4: Cloud Features
- [ ] Cloud storage integration
- [ ] Cross-device sync
- [ ] File sharing
- [ ] Collaboration features

## Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [MAUI GitHub Repository](https://github.com/dotnet/maui)
- [MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui)
- [Platform-Specific Services](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/)

## Contributors

- Migration performed by: Claude Code
- Original WPF Application: UpRev Team

## License

Same license as the original UpRev File Opener application.
