namespace UpRevFileOpener
{
    class PasswordVerification
    {
        public static string verifyPassword(string password)
        {
            string filesNotPasswordProtected = "Not protected";
            string passwordVerified = "Verified";
            string passwordNotVerified = "Not Verified";
            bool verified = false;
            if (Properties.Settings.Default.Passwords != null)
            {
                foreach (string passwordWithId in Properties.Settings.Default.Passwords)
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

        public static bool isPasswordProtected(string fileName)
        {
            bool passwordProtected = false;
            foreach (string file in Properties.Settings.Default.FileNames)
            {
                if (fileName == file.Substring(8))
                {
                    passwordProtected = true;
                }
            }
            return passwordProtected;
        }

        public static void updatePassword(string fileName)
        {
            string removeId = "";
            string removeFileName = "";
            string removePassword = "";
            foreach (string file in Properties.Settings.Default.FileNames)
            {
                if (fileName == file.Substring(8))
                {
                    removeId = file.Substring(0, 8);
                    removeFileName = file;
                    break;
                }
            }
            foreach (string password in Properties.Settings.Default.Passwords)
            {
                if (removeId == password.Substring(0,8))
                {
                    removePassword = password;
                }
            }
            Properties.Settings.Default.FileNames.Remove(removeFileName);
            Properties.Settings.Default.Passwords.Remove(removePassword);
        }
    }
}
