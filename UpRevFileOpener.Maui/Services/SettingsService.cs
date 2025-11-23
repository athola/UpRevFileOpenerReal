using System.Text.Json;

namespace UpRevFileOpener.Services;

/// <summary>
/// Service to manage application settings using MAUI Preferences
/// Replaces the old Properties.Settings system from WPF
/// Falls back to in-memory storage when MAUI Preferences is not available (e.g., in tests)
/// </summary>
public static class SettingsService
{
    private const string RecentItemsKey = "RecentItems";
    private const string PasswordsKey = "Passwords";
    private const string FileNamesKey = "FileNames";
    private const string ProductKeyEnteredKey = "ProductKeyEntered";

    // In-memory fallback storage for test environments
    private static readonly Dictionary<string, string> _inMemoryStorage = new();
    private static bool? _useInMemoryStorage;

    private static bool UseInMemoryStorage
    {
        get
        {
            if (_useInMemoryStorage.HasValue)
                return _useInMemoryStorage.Value;

            // Check if we're in a test environment first
            if (IsTestEnvironment())
            {
                _useInMemoryStorage = true;
                return true;
            }

            try
            {
                // Try to actually use Preferences to see if MAUI is initialized
                // Just accessing Preferences.Default may not throw even if MAUI isn't initialized
                Preferences.Default.ContainsKey("__test_key__");
                _useInMemoryStorage = false;
            }
            catch
            {
                _useInMemoryStorage = true;
            }
            return _useInMemoryStorage.Value;
        }
    }

    private static bool IsTestEnvironment()
    {
        try
        {
            // Check if any test framework assemblies are loaded
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var name = assembly.GetName().Name;
                if (name != null && (
                    name.Contains("xunit", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("nunit", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("mstest", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("testhost", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static string GetValue(string key, string defaultValue)
    {
        if (UseInMemoryStorage)
        {
            return _inMemoryStorage.TryGetValue(key, out var value) ? value : defaultValue;
        }
        return Preferences.Get(key, defaultValue);
    }

    private static void SetValue(string key, string value)
    {
        if (UseInMemoryStorage)
        {
            _inMemoryStorage[key] = value;
        }
        else
        {
            Preferences.Set(key, value);
        }
    }

    private static bool GetBoolValue(string key, bool defaultValue)
    {
        if (UseInMemoryStorage)
        {
            return _inMemoryStorage.TryGetValue(key, out var value) && bool.TryParse(value, out var result) ? result : defaultValue;
        }
        return Preferences.Get(key, defaultValue);
    }

    private static void SetBoolValue(string key, bool value)
    {
        if (UseInMemoryStorage)
        {
            _inMemoryStorage[key] = value.ToString();
        }
        else
        {
            Preferences.Set(key, value);
        }
    }

    public static List<string> RecentItems
    {
        get
        {
            var json = GetValue(RecentItemsKey, "[]");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        set
        {
            var json = JsonSerializer.Serialize(value);
            SetValue(RecentItemsKey, json);
        }
    }

    public static List<string> Passwords
    {
        get
        {
            var json = GetValue(PasswordsKey, "[]");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        set
        {
            var json = JsonSerializer.Serialize(value);
            SetValue(PasswordsKey, json);
        }
    }

    public static List<string> FileNames
    {
        get
        {
            var json = GetValue(FileNamesKey, "[]");
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        set
        {
            var json = JsonSerializer.Serialize(value);
            SetValue(FileNamesKey, json);
        }
    }

    public static bool ProductKeyEntered
    {
        get => GetBoolValue(ProductKeyEnteredKey, false);
        set => SetBoolValue(ProductKeyEnteredKey, value);
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

    /// <summary>
    /// Clear all in-memory storage (useful for test cleanup)
    /// </summary>
    public static void ClearInMemoryStorage()
    {
        _inMemoryStorage.Clear();
    }
}
