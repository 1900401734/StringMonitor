using Microsoft.Win32;
using System.Diagnostics;

namespace TCPServer
{
    public static class AutoLaunch
    {
        private const string Key = "TCPServer";

        public static void AutoStart(bool enable)
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                try
                {
                    if (enable)
                    {
                        key?.SetValue(Key, Application.ExecutablePath);
                    }
                    else
                    {
                        key?.DeleteValue(Key, false); // Corrected to delete the value when disabling
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
