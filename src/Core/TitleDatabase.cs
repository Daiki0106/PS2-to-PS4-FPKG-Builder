using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PS2toPS4Builder.Core
{
    public static class TitleDatabase
    {
        private static Dictionary<string, string> _titles = null;
        private static Dictionary<string, string> _titleToSerial = null;

        public static void Initialize(string csvPath)
        {
            if (_titles != null) return;
            _titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _titleToSerial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(csvPath)) return;

            try
            {
                using (StreamReader reader = new StreamReader(csvPath, Encoding.Default))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line)) continue;
                        string[] parts = line.Split(';');
                        if (parts.Length >= 2)
                        {
                            string id = parts[0].Trim();
                            string name = parts[1].Trim();
                            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                            {
                                if (!_titles.ContainsKey(id))
                                {
                                    _titles[id] = name;
                                }

                                string cleanName = CleanTitle(name);
                                if (!_titleToSerial.ContainsKey(cleanName))
                                {
                                    _titleToSerial[cleanName] = id;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static string GetTitle(string serial)
        {
            if (_titles == null || string.IsNullOrEmpty(serial)) return null;

            if (_titles.ContainsKey(serial))
                return _titles[serial];

            string normalized = serial.Replace("-", "_");
            if (_titles.ContainsKey(normalized))
                return _titles[normalized];

            return null;
        }

        public static string FindSerialByKeywords(string text)
        {
            if (_titleToSerial == null || string.IsNullOrEmpty(text)) return null;

            string clean = CleanTitle(text);
            if (clean.Length < 3) return null;

            // 1. Búsqueda directa exacta
            if (_titleToSerial.ContainsKey(clean))
                return _titleToSerial[clean];

            // 2. Búsqueda por coincidencia de frase completa
            foreach (var kvp in _titleToSerial)
            {
                string dbKey = kvp.Key;
                if (dbKey.Length >= 6)
                {
                    if (clean == dbKey || clean.StartsWith(dbKey + " ") || clean.EndsWith(" " + dbKey))
                    {
                        return kvp.Value;
                    }
                }
            }

            // 3. Búsqueda inteligente por tokens/palabras coincidentes
            string[] queryWords = clean.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (queryWords.Length == 0) return null;

            string bestMatchSerial = null;
            int bestMatchScore = 0;

            foreach (var kvp in _titleToSerial)
            {
                string dbKey = kvp.Key;
                string[] dbWords = dbKey.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (dbWords.Length == 0) continue;

                int matches = 0;
                int significantQueryWords = 0;

                foreach (string qw in queryWords)
                {
                    if (qw.Length <= 2) continue; // Ignorar partículas como 'de', 'la', 'of', 'in'
                    significantQueryWords++;
                    foreach (string dw in dbWords)
                    {
                        if (qw == dw)
                        {
                            matches++;
                            break;
                        }
                    }
                }

                if (significantQueryWords == 0) continue;

                // Debe coincidir al menos el 70% de las palabras y mínimo 2 palabras significativas (evita falsos positivos con 'Super', 'The', etc.)
                if (matches >= 2 && matches >= Math.Ceiling(significantQueryWords * 0.7) && matches >= Math.Ceiling(dbWords.Length * 0.5))
                {
                    if (matches > bestMatchScore)
                    {
                        bestMatchScore = matches;
                        bestMatchSerial = kvp.Value;
                    }
                }
            }

            return bestMatchSerial;
        }

        private static string CleanTitle(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            // Quitar texto entre paréntesis o corchetes: (USA), [v1.0], (PS2), etc.
            string res = Regex.Replace(text, @"\([^)]*\)|\[[^\]]*\]", " ");
            res = Regex.Replace(res, @"[^a-zA-Z0-9\s]", " ");
            res = Regex.Replace(res, @"\s+", " ").Trim().ToLower();

            // Quitar etiquetas comunes que no son del nombre del juego
            res = Regex.Replace(res, @"\b(ps2|playstation 2|dvd|iso|rip|usa|europe|eur|japan|jap|pal|ntsc|espanol|spanish)\b", " ").Trim();
            res = Regex.Replace(res, @"\s+", " ").Trim();
            return res;
        }
    }
}
