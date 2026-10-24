using System;
using System.Collections.Generic;
using System.IO;

namespace PS2toPS4Builder.Core
{
    public static class ConfigDatabase
    {
        private static string _configsDir = null;
        private static HashSet<string> _availableSerials = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static void Initialize(string configsDir)
        {
            _configsDir = configsDir;
            _availableSerials.Clear();

            if (!Directory.Exists(configsDir)) return;

            try
            {
                string[] files = Directory.GetFiles(configsDir, "*.*");
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".txt" || ext == ".lua")
                    {
                        string name = Path.GetFileNameWithoutExtension(file).ToUpper();
                        if (name != "DEFAULT")
                        {
                            _availableSerials.Add(name);
                        }
                    }
                }
            }
            catch { }
        }

        public static string NormalizeSerial(string serial)
        {
            if (string.IsNullOrEmpty(serial)) return "";
            return serial.Replace("_", "-").Replace(".", "").Trim().ToUpper();
        }

        public static bool HasConfig(string serial)
        {
            string norm = NormalizeSerial(serial);
            return !string.IsNullOrEmpty(norm) && _availableSerials.Contains(norm);
        }

        public static string GetConfigText(string serial)
        {
            if (string.IsNullOrEmpty(_configsDir)) return null;
            string norm = NormalizeSerial(serial);
            if (string.IsNullOrEmpty(norm)) return null;

            string txtPath = Path.Combine(_configsDir, norm + ".txt");
            if (File.Exists(txtPath))
            {
                try
                {
                    return File.ReadAllText(txtPath);
                }
                catch { }
            }
            return null;
        }

        public static string GetLuaPath(string serial)
        {
            if (string.IsNullOrEmpty(_configsDir)) return null;
            string norm = NormalizeSerial(serial);
            if (string.IsNullOrEmpty(norm)) return null;

            string luaPath = Path.Combine(_configsDir, norm + ".lua");
            if (File.Exists(luaPath))
            {
                return luaPath;
            }
            return null;
        }

        public static string GetDefaultConfigText()
        {
            if (string.IsNullOrEmpty(_configsDir)) return null;
            string defPath = Path.Combine(_configsDir, "default.txt");
            if (File.Exists(defPath))
            {
                try
                {
                    return File.ReadAllText(defPath);
                }
                catch { }
            }
            return null;
        }
    }
}
