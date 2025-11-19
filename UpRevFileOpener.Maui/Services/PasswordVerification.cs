namespace UpRevFileOpener.Services;

public static class PasswordVerification
{
    public static string VerifyPassword(string password)
    {
        string filesNotPasswordProtected = "Not protected";
        string passwordVerified = "Verified";
        string passwordNotVerified = "Not Verified";
        bool verified = false;

        var passwords = SettingsService.Passwords;

        if (passwords != null && passwords.Count > 0)
        {
            foreach (string passwordWithId in passwords)
            {
                if (password == passwordWithId)
                {
                    verified = true;
                }
            }
            if (verified)
            {
                return passwordVerified;
            }
            else
            {
                return passwordNotVerified;
            }
        }
        else
        {
            return filesNotPasswordProtected;
        }
    }

    public static bool IsPasswordProtected(string fileName)
    {
        bool passwordProtected = false;
        var fileNames = SettingsService.FileNames;

        if (fileNames == null || fileNames.Count == 0)
            return false;

        foreach (string file in fileNames)
        {
            if (file.Length > 8 && fileName == file.Substring(8))
            {
                passwordProtected = true;
            }
        }
        return passwordProtected;
    }

    public static void UpdatePassword(string fileName)
    {
        string removeId = "";
        string removeFileName = "";
        string removePassword = "";

        var fileNames = SettingsService.FileNames;
        var passwords = SettingsService.Passwords;

        foreach (string file in fileNames)
        {
            if (file.Length > 8 && fileName == file.Substring(8))
            {
                removeId = file.Substring(0, 8);
                removeFileName = file;
                break;
            }
        }

        foreach (string password in passwords)
        {
            if (password.Length > 8 && removeId == password.Substring(0, 8))
            {
                removePassword = password;
            }
        }

        fileNames.Remove(removeFileName);
        passwords.Remove(removePassword);

        // Save changes back to preferences
        SettingsService.FileNames = fileNames;
        SettingsService.Passwords = passwords;
    }
}
