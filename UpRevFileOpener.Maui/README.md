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
3. **File Dialogs**:
   - File Open: Using MAUI's `FilePicker`
   - File Save: Platform-specific save dialogs (Windows uses native FileSavePicker)
4. **Menu System**: Converted to MAUI's `MenuBarItems` with dynamic recent files menu
5. **Rich Text Editor**: WebView-based editor using Quill.js with full RTF support

### Features

#### ✅ Rich Text Editing
The application now features a **fully functional rich text editor** using Quill.js:
- Bold, Italic, Underline, Strikethrough formatting
- Font family selection (Arial, Courier, Georgia, Times, Verdana)
- Font size selection (8-24pt)
- Text color and background color
- Lists (ordered and unordered)
- Text alignment
- Hyperlink support
- RTF file format support (read and write)
- Backward compatible with files created by WPF application

#### ✅ Platform-Specific File Save Dialogs
- **Windows**: Native FileSavePicker with proper folder browsing
- **macOS/iOS/Android**: Intelligent file save to appropriate locations
- Suggested filename based on current file
- Automatic directory creation for organized file storage

#### ✅ Dynamic Recent Files Menu
- Shows up to 10 most recent files
- Click any recent file to open it quickly
- "Clear Recent Files" option
- Numbered list (most recent first)
- Automatic menu updates when files are opened

### Known Limitations

None! All critical limitations have been resolved.

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
