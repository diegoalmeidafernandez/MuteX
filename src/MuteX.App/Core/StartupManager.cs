using Microsoft.Win32;
using System;

namespace MuteX.App.Core
{
    public class StartupManager
    {
        private const string RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "MuteX";

        public void EnableStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true);
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
            key!.SetValue(APP_NAME, $"\"{exePath}\"");
        }

        public void DisableStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true);
            key!.DeleteValue(APP_NAME, false);
        }

        public bool IsStartupEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, false);
            return key?.GetValue(APP_NAME) != null;
        }
    }
}