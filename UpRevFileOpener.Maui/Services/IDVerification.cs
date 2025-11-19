namespace UpRevFileOpener.Services;

public static class IDVerification
{
    public static string GetId()
    {
        Random rand = new Random();
        int integer = rand.Next(10000000);
        string id = integer.ToString("D8");
        return id;
    }

    public static string GetIdFromFileNames(string fileName)
    {
        string noFiles = "No files";
        var fileNames = SettingsService.FileNames;

        if (fileNames != null && fileNames.Count > 0)
        {
            foreach (string fileNameWithId in fileNames)
            {
                if (fileNameWithId.Length > 8)
                {
                    string fileNameWithoutId = fileNameWithId.Substring(8);
                    if (fileName == fileNameWithoutId)
                    {
                        string fileId = fileNameWithId.Substring(0, 8);
                        return fileId;
                    }
                }
            }
        }
        return noFiles;
    }
}
