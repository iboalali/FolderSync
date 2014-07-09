using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace FolderSync
{
 
    public static class ModifyRegistry
    {
        private static bool showError = false;
       
        private static bool ShowError
        {
            get { return showError; }
            set { showError = value; }
        }

        private static string subKey = "SOFTWARE\\" + Application.ProductName.ToUpper();
      
        public static string SubKey
        {
            get { return subKey; }
            set { subKey = value; }
        }

        private static RegistryKey baseRegistryKey = Registry.CurrentUser;
       
        public static RegistryKey BaseRegistryKey
        {
            get { return baseRegistryKey; }
            set { baseRegistryKey = value; }
        }

       
        public static string Read(string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }

      
        public static bool Write(string KeyName, object Value)
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // Save the value
                sk1.SetValue(KeyName, Value);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Writing registry " + KeyName);
                return false;
            }
        }

      
        private static bool DeleteKey(string KeyName)
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // If the RegistrySubKey doesn't exists -> (true)
                if (sk1 == null)
                    return true;
                else
                    sk1.DeleteValue(KeyName);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Deleting SubKey " + subKey);
                return false;
            }
        }

      
        private static  bool DeleteSubKeyTree()
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistryKey exists, I delete it
                if (sk1 != null)
                    rk.DeleteSubKeyTree(subKey);

                return true;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Deleting SubKey " + subKey);
                return false;
            }
        }

     
        private static int SubKeyCount()
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.SubKeyCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                ShowErrorMessage(e, "Retriving subkeys of " + subKey);
                return 0;
            }
        }

       
        private static int ValueCount()
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                // If the RegistryKey exists...
                if (sk1 != null)
                    return sk1.ValueCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
             
                ShowErrorMessage(e, "Retriving keys of " + subKey);
                return 0;
            }
        }

      
        private static void ShowErrorMessage(Exception e, string Title)
        {
            if (showError == true)
                MessageBox.Show(e.Message,
                                Title
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Error);
        }

        public static void AddFileToSendLaterList(string FileName)
        {   string [] x;
            string t = Read("NotSentFilesXX");
            if (t != null)
            {
                x = t.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] y = new string[x.Length + 1];
                Array.Copy(x, y, x.Length);
                y[x.Length] = FileName;
                Write("NotSentFilesXX",string.Join(",",y));
            }
            else
                Write("NotSentFilesXX", FileName);     

            
        }

        //pop a file from the files list 
        public static string GetFileFromSendLaterList()
        {
            try
            {
                string[] x = Read("NotSentFilesXX").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] y = new string[x.Length - 1];
                for (int i = 1; i < x.Length; i++)
                    y[i - 1] = x[i];
                Write("NotSentFilesXX", string.Join(",", y));
                if (x.Length == 0) return null;
                return x[0];
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteFileFromSendLaterList(string FileName)
        {

        }
        public static int GetFileCount()
        {
            return Convert.ToInt32( Read("FilesCountXX"));
        }
        public static void SetFileCount(int NewCount)
        {
            Write("FilesCountXX", NewCount.ToString());
        }

    }
}

