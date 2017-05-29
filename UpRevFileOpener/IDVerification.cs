using System;

namespace UpRevFileOpener
{
    class IDVerification
    {

        public static string getId()
        {
            Random rand = new Random();
            int integer = rand.Next(10000000);
            string id = integer.ToString("D8");
            return id;
        }

        public static string getIdFromFileNames(string fileName)
        {
            string noFiles = "No files";
            if (Properties.Settings.Default.FileNames != null)
            {
                foreach (string fileNameWithId in Properties.Settings.Default.FileNames)
                {
                    string fileNameWithoutId = fileNameWithId.Substring(8);
                    if (fileName == fileNameWithoutId)
                    {
                        string fileId = fileNameWithId.Substring(0, 8);
                        return fileId;
                    }
                }
            }
            return noFiles;
        }
    }
}
