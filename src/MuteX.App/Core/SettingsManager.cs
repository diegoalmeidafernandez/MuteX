using System;
using System.IO;
using Newtonsoft.Json;

namespace MuteX.App.Core
{
    public class AppSettings
    {
        public uint HotKeyModifier { get; set; } = 0;
        public uint HotKeyKey { get; set; } = 0x78;
        public bool StartWithWindows { get; set; } = false;
    }

    public class SettingsManager
    {
        private readonly string _configPath;
        public AppSettings Settings { get; private set; }

        public SettingsManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "MuteX");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            _configPath = Path.Combine(folder, "settings.json");
            Load();
        }

        public void Load()
        {
            if (!File.Exists(_configPath))
            {
                Settings = new AppSettings();
                Save();
            }
            else
            {
                var json = File.ReadAllText(_configPath);
                Settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(_configPath, json);
        }
    }
}