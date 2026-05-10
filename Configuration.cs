using System;
using System.IO;
using System.Text.Json;

namespace LockDownBypass
{
    public class Configuration
    {
        public HotkeyConfig Hotkeys { get; set; }
        public bool StealthMode { get; set; }
        public bool AutoStart { get; set; }

        public static Configuration Load()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    return JsonSerializer.Deserialize<Configuration>(json) ?? CreateDefault();
                }
                catch
                {
                    return CreateDefault();
                }
            }

            var config = CreateDefault();
            config.Save();
            return config;
        }

        public void Save()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(configPath, json);
        }

        private static Configuration CreateDefault()
        {
            return new Configuration
            {
                Hotkeys = new HotkeyConfig
                {
                    NextTab = "Ctrl+Shift+T",
                    PrevTab = "Ctrl+Shift+W",
                    Minimize = "Ctrl+Shift+M",
                    ExitTool = "Ctrl+Shift+Q"
                },
                StealthMode = true,
                AutoStart = false
            };
        }
    }

    public class HotkeyConfig
    {
        public string NextTab { get; set; }
        public string PrevTab { get; set; }
        public string Minimize { get; set; }
        public string ExitTool { get; set; }
    }
}
