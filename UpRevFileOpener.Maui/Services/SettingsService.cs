using System.Text.Json;

namespace UpRevFileOpener.Services;

/// <summary>
/// Service to manage application settings using MAUI Preferences
/// Replaces the old Properties.Settings system from WPF
/// </summary>
public static class SettingsService
{
    private const string RecentItemsKey = "RecentItems";
    private const string PasswordsKey = "Passwords";
    private const string FileNamesKey = "FileNames";
    private const string ProductKeyEnteredKey = "ProductKeyEntered";

    public static List<string> RecentItems
    {
        get
        {
            var json = Preferences.Get(RecentItemsKey, "[]");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        set
        {
            var json = JsonSerializer.Serialize(value);
            Preferences.Set(RecentItemsKey, json);
        }
    }

    public static List<string> Passwords
    {
        get
        {
            var json = Preferences.Get(PasswordsKey, "[]");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        set
        {
            var json = JsonSerializer.Serialize(value);
            Preferences.Set(PasswordsKey, json);
        }
    }

    public static List<string> FileNames
    {
        get
        {
            var json = Preferences.Get(FileNamesKey, "[]");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        set
        {
            var json = JsonSerializer.Serialize(value);
            Preferences.Set(FileNamesKey, json);
        }
    }

    public static bool ProductKeyEntered
    {
        get => Preferences.Get(ProductKeyEnteredKey, false);
        set => Preferences.Set(ProductKeyEnteredKey, value);
    }

    public static void Save()
    {
        // In MAUI, Preferences are automatically saved
        // This method is kept for compatibility with the old API
    }

    public static void Reload()
    {
        // In MAUI, Preferences are automatically loaded
        // This method is kept for compatibility with the old API
    }
}
