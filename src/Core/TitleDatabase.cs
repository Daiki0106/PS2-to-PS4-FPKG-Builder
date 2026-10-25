using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS2toPS4Builder.Core
{
    public static class TitleDatabase
    {
        private static Dictionary<string, string> _titles = null;
        private static Dictionary<string, string> _titleToSerial = null;
        private static string _savedCsvPath = null;

        public static void Initialize(string csvPath)
        {
            if (_titles != null) return;
            _titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _titleToSerial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _savedCsvPath = csvPath;

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

            // Probar formato con guion estándar: XXXX-YYYYY
            Match m = Regex.Match(serial.Trim().ToUpper(), @"([A-Z]{4})[-_]?(\d{3})\.?(\d{2})");
            if (m.Success)
            {
                string withDash = string.Format("{0}-{1}{2}", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
                if (_titles.ContainsKey(withDash))
                    return _titles[withDash];
            }

            string normalized = serial.Replace("-", "_");
            if (_titles.ContainsKey(normalized))
                return _titles[normalized];

            return null;
        }

        public static void RegisterTitle(string serial, string title)
        {
            if (string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(title)) return;
            if (_titles == null) _titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (_titleToSerial == null) _titleToSerial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _titles[serial] = title;
            string clean = CleanTitle(title);
            if (!string.IsNullOrEmpty(clean) && !_titleToSerial.ContainsKey(clean))
            {
                _titleToSerial[clean] = serial;
            }

            if (!string.IsNullOrEmpty(_savedCsvPath) && File.Exists(_savedCsvPath))
            {
                try
                {
                    File.AppendAllText(_savedCsvPath, string.Format("{0};{1}\r\n", serial, title), Encoding.Default);
                }
                catch { }
            }
        }

        public static async Task<string> FetchTitleOnlineAsync(string serial)
        {
            if (string.IsNullOrEmpty(serial)) return null;

            // Formatear serial estándar: XXXX-YYYYY
            string cleanSerial = serial.Trim().ToUpper();
            Match m = Regex.Match(cleanSerial, @"([A-Z]{4})[-_]?(\d{3})\.?(\d{2})");
            if (m.Success)
            {
                cleanSerial = string.Format("{0}-{1}{2}", m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }

            // Consultar PsxDataCenter
            string url = string.Format("https://psxdatacenter.com/psx2/games2/{0}.html", cleanSerial);
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 5000;
                req.ReadWriteTimeout = 5000;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) PS2toPS4Builder";

                using (WebResponse resp = await Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null))
                using (Stream stream = resp.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("windows-1252")))
                {
                    string html = await reader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(html))
                    {
                        // 1. Probar etiqueta <title>
                        Match tm = Regex.Match(html, @"<title>\s*(.*?)\s*(?:-\s*\((?:PAL|NTSC-U|NTSC-J|NTSC-K)\))?\s*</title>", RegexOptions.IgnoreCase);
                        string rawTitle = tm.Success ? tm.Groups[1].Value.Trim() : null;

                        // 2. Si no, probar OFFICIAL TITLE
                        if (string.IsNullOrEmpty(rawTitle))
                        {
                            Match om = Regex.Match(html, @"OFFICIAL TITLE.*?&nbsp;(.*?)</td", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            if (om.Success) rawTitle = om.Groups[1].Value.Trim();
                        }

                        if (!string.IsNullOrEmpty(rawTitle))
                        {
                            rawTitle = WebUtility.HtmlDecode(rawTitle).Trim();
                            // Si está en mayúsculas completas, formatear a título legible
                            if (rawTitle.Equals(rawTitle.ToUpperInvariant()) && rawTitle.Length > 4)
                            {
                                TextInfo ti = CultureInfo.InvariantCulture.TextInfo;
                                rawTitle = ti.ToTitleCase(rawTitle.ToLowerInvariant());
                            }

                            RegisterTitle(cleanSerial, rawTitle);
                            return rawTitle;
                        }
                    }
                }
            }
            catch { }

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
