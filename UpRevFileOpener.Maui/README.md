# UpRev File Opener - .NET MAUI Edition

This is the cross-platform version of the UpRev File Opener application, migrated from WPF to .NET MAUI.

## Platform Support

This application can build and run on:
- **Windows** (Windows 10 version 1809 or later)
- **macOS** (macOS 10.15 or later via Mac Catalyst)
- **iOS** (iOS 11.0 or later)
- **Android** (Android 5.0 / API 21 or later)
- **Linux** (via community extensions - experimental)

## Requirements

To build and run this application, you need:

- .NET 8.0 SDK or later
- Visual Studio 2022 17.8+ (Windows/Mac) or Visual Studio Code with C# Dev Kit
- Platform-specific workloads:
  - For Android: Android SDK
  - For iOS/macOS: Xcode (on macOS)
  - For Windows: Windows App SDK

### Installing Workloads

Run the following command to install required workloads:

```bash
dotnet workload install maui
```

## Building the Application

### Windows
```bash
dotnet build -f net8.0-windows10.0.19041.0
```

### macOS (Mac Catalyst)
```bash
dotnet build -f net8.0-maccatalyst
```

### Android
```bash
dotnet build -f net8.0-android
```

### iOS
```bash
dotnet build -f net8.0-ios
```

## Running the Application

### Windows
```bash
dotnet run -f net8.0-windows10.0.19041.0
```

### Android (with emulator)
```bash
dotnet run -f net8.0-android
```

### iOS (with simulator on macOS)
```bash
dotnet run -f net8.0-ios
```

## Migration Notes

### What Was Changed

1. **UI Framework**: Migrated from WPF XAML to .NET MAUI XAML
2. **Settings Storage**: Changed from `Properties.Settings` to `Preferences` API
3. **File Dialogs**: Using MAUI's `FilePicker` instead of WPF's `OpenFileDialog`/`SaveFileDialog`
4. **Menu System**: Converted to MAUI's `MenuBarItems` (desktop) with toolbar buttons (mobile)
5. **Text Editor**: Using MAUI `Editor` control (see limitations below)

### Known Limitations

#### Rich Text Editing
The original WPF application used `RichTextBox` which supports full RTF (Rich Text Format) with formatting like bold, italic, underline, fonts, etc. **MAUI does not have a built-in rich text editor control**.

Current implementation:
- Uses basic `Editor` control for plain text
- Formatting buttons are present but show an alert about the limitation
- Font family and size can be applied to the entire editor content

**Recommended Solutions**:
1. Use a third-party rich text editor component:
   - Syncfusion Rich Text Editor
   - DevExpress MAUI Controls
   - Custom WebView-based HTML editor

2. Convert RTF files to a different format:
   - HTML (can be edited in WebView)
   - Markdown (with preview)
   - JSON with formatting metadata

#### File Saving
MAUI's `FilePicker` doesn't provide a true "Save As" dialog on all platforms. The current implementation:
- Prompts for a filename
- Saves to the app's data directory
- Displays the saved file path

For a better experience, consider:
- Platform-specific file save implementations
- Cloud storage integration (OneDrive, Google Drive, etc.)
- Share functionality to export files

#### Recent Files Menu
Recent files are stored in preferences but aren't dynamically displayed in the menu due to MAUI menu limitations. Consider:
- Creating a dedicated "Recent Files" page
- Using a popup or bottom sheet for recent files selection

### Security Considerations

The password protection system has been migrated but consider:
- Using stronger encryption (current system uses simple ID-based obfuscation)
- Implementing proper key derivation (PBKDF2, Argon2)
- Storing passwords securely using platform-specific secure storage

## Project Structure

```
UpRevFileOpener.Maui/
├── Platforms/           # Platform-specific code
│   ├── Android/
│   ├── iOS/
│   ├── MacCatalyst/
│   └── Windows/
├── Resources/           # App resources
│   ├── AppIcon/        # Application icon
│   ├── Fonts/          # Custom fonts
│   ├── Images/         # Image assets
│   ├── Splash/         # Splash screen
│   └── Styles/         # XAML styles
├── Services/            # Business logic
│   ├── IDVerification.cs
│   ├── PasswordVerification.cs
│   └── SettingsService.cs
├── Views/               # UI pages
│   ├── LoginPage.xaml
│   ├── LoginPage.xaml.cs
│   ├── MainPage.xaml
│   └── MainPage.xaml.cs
├── App.xaml            # Application definition
├── App.xaml.cs
├── AppShell.xaml       # App shell/navigation
├── AppShell.xaml.cs
└── MauiProgram.cs      # App configuration
```

## Differences from WPF Version

| Feature | WPF Version | MAUI Version |
|---------|-------------|--------------|
| Rich Text Editing | ✅ Full RTF support | ⚠️ Plain text only (needs 3rd party) |
| File Dialogs | ✅ Native Windows dialogs | ⚠️ Cross-platform picker |
| Settings | Properties.Settings | Preferences API |
| Window Management | Multiple windows | Single window/page navigation |
| Recent Files Menu | Dynamic menu items | Stored but not in menu |
| Font Formatting | Per-selection | Entire editor |

## Future Enhancements

1. **Implement proper rich text editing**
   - Integrate a third-party rich text editor
   - Or implement WebView-based HTML editing

2. **Improve file management**
   - Platform-specific save dialogs
   - Cloud storage integration
   - File type associations

3. **Enhanced mobile experience**
   - Touch-optimized UI
   - Gestures for common actions
   - Mobile-specific navigation patterns

4. **Cross-platform file sync**
   - Cloud backup/sync
   - Multi-device support

## License

Same as the original UpRev File Opener application.

## Support

For issues specific to the MAUI migration, please check:
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [.NET MAUI GitHub Issues](https://github.com/dotnet/maui/issues)
