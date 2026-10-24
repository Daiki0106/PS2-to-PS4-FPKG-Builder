using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS2toPS4Builder.Core
{
    public static class FormatHelper
    {
        public static readonly string FileFilter = 
            "Imágenes de PS2 (*.iso;*.bin;*.img;*.mdf;*.gz;*.7z;*.zip;*.chd)|*.iso;*.bin;*.img;*.mdf;*.gz;*.7z;*.zip;*.chd|" +
            "Archivos ISO (*.iso)|*.iso|" +
            "Archivos BIN (*.bin)|*.bin|" +
            "Archivos IMG/MDF (*.img;*.mdf)|*.img;*.mdf|" +
            "Archivos Comprimidos (*.gz;*.7z;*.zip;*.chd)|*.gz;*.7z;*.zip;*.chd|" +
            "Todos los archivos (*.*)|*.*";

        public static bool IsSupportedExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            string e = ext.ToLower();
            return e == ".iso" || e == ".bin" || e == ".img" || e == ".mdf" ||
                   e == ".gz" || e == ".7z" || e == ".zip" || e == ".chd";
        }

        public static bool IsDirectFormat(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            string e = ext.ToLower();
            return e == ".iso" || e == ".bin" || e == ".img" || e == ".mdf";
        }

        public static bool IsCompressedArchive(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            string e = ext.ToLower();
            return e == ".gz" || e == ".7z" || e == ".zip";
        }

        public static bool IsChd(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            return ext.ToLower() == ".chd";
        }

        public class DiscValidation
        {
            public bool IsValid;
            public string StatusText;
            public Color StatusColor;
            public string Category;
            public bool IsCheatOrUtility;
        }

        public static bool CheckDiskSpace(string path, long requiredBytes, out long availableBytes)
        {
            availableBytes = 0;
            try
            {
                string root = Path.GetPathRoot(Path.GetFullPath(path));
                DriveInfo drive = new DriveInfo(root);
                availableBytes = drive.AvailableFreeSpace;
                return availableBytes >= requiredBytes;
            }
            catch
            {
                return true;
            }
        }

        public static string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "PS2_Game";
            string clean = Regex.Replace(name, @"[\\/:*?""<>|]", " - ");
            clean = Regex.Replace(clean, @"\s+", " ").Trim();
            clean = clean.TrimEnd('.', ' ');
            return string.IsNullOrEmpty(clean) ? "PS2_Game" : clean;
        }

        private static readonly Regex CheatKeywordsRegex = new Regex(
            @"\((?:Unl|Unlicensed)\)|Ultimate\s+Codes|Action\s+Replay|\bAR\s*MAX\b|\bARMAX\b|GameShark|Code\s*Breaker|Swap\s*Magic|HD\s*Loader|Open\s*PS2\s*Loader|\bOPL\b|Cheat\s*Device|Xploder|Free\s*McBoot|\bFMCB\b|LaunchELF|uLaunchELF|\bCheat\s*Disc\b",
            RegexOptions.IgnoreCase);

        public static bool IsCheatOrUtility(string fileName, string filePath = null, string serial = null, string dbTitle = null)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            // 1. Coincidencia por nombre de archivo o palabras clave conocidas
            if (CheatKeywordsRegex.IsMatch(fileName))
            {
                return true;
            }

            // 2. Coincidencia por firma interna en el disco (.COD de Datel, etc.)
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    string ext = Path.GetExtension(filePath).ToLower();
                    if (IsDirectFormat(ext))
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] buf = new byte[Math.Min(fs.Length, 4 * 1024 * 1024)];
                            int read = fs.Read(buf, 0, buf.Length);
                            string text = Encoding.ASCII.GetString(buf, 0, read);

                            if (text.Contains(".COD;1") || text.Contains("ARMAX") || text.Contains("SWAPMAGIC") || text.Contains("CODER.ELF"))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch { }
            }

            // 3. Discordancia evidente (ej: archivo dice Spider-Man pero serial es TimeSplitters)
            if (!string.IsNullOrEmpty(dbTitle) && !string.IsNullOrEmpty(fileName))
            {
                string cleanFile = Regex.Replace(fileName, @"[_\-\.]", " ").ToLower();
                string cleanDb = Regex.Replace(dbTitle, @"[_\-\.]", " ").ToLower();

                // Si el nombre del archivo contiene palabras como "codes", "cheat", "hack", "loader"
                if (Regex.IsMatch(cleanFile, @"\b(codes|cheat|cheats|hack|hacks|loader|exploit)\b"))
                {
                    return true;
                }
            }

            return false;
        }

        public static DiscValidation ValidateDisc(long sizeInBytes, bool hasSystemCnf, bool isCompressed, bool isPs1 = false, bool isTruncated = false, bool isCheatOrUtility = false)
        {
            DiscValidation res = new DiscValidation();
            res.IsCheatOrUtility = isCheatOrUtility;

            // 0. SI la imagen está incompleta / truncada
            if (isTruncated)
            {
                res.IsValid = false;
                res.StatusText = "❌ Descarga incompleta";
                res.StatusColor = Color.FromArgb(231, 76, 60); // Rojo
                res.Category = "Incompleta / Dañada";
                return res;
            }

            // 1. SI es un juego de PlayStation 1 -> ❌ Es juego PS1 (No PS2)
            if (isPs1)
            {
                res.IsValid = false;
                res.StatusText = "❌ Es juego PS1 (No PS2)";
                res.StatusColor = Color.FromArgb(231, 76, 60); // Rojo
                res.Category = "Juego PS1";
                return res;
            }

            // 2. SI NO contiene SYSTEM.CNF -> ❌ No es juego de PS2
            if (!hasSystemCnf)
            {
                res.IsValid = false;
                res.StatusText = "❌ No es juego PS2";
                res.StatusColor = Color.FromArgb(231, 76, 60); // Rojo
                res.Category = "No es PS2";
                return res;
            }

            // 3. SI es un disco de trucos o utilidad no oficial
            if (isCheatOrUtility)
            {
                res.IsValid = true;
                res.StatusText = isCompressed ? "⚠️ Utilidad / Trucos (Comp.)" : "⚠️ Disco de Trucos / Utilidad";
                res.StatusColor = Color.FromArgb(230, 126, 34); // Naranja
                res.Category = "Disco de Trucos / Utilidad";
                return res;
            }

            // 2. SI pesa < 20 MB -> ❌ Probablemente no es un juego (demasiado pequeño)
            const long TwentyMb = 20L * 1024L * 1024L;
            if (sizeInBytes < TwentyMb)
            {
                res.IsValid = false;
                res.StatusText = "❌ Muy pequeño (< 20MB)";
                res.StatusColor = Color.FromArgb(231, 76, 60); // Rojo
                res.Category = "Muy pequeño (< 20MB)";
                return res;
            }

            const long SevenHundredMb = 700L * 1024L * 1024L;
            const long FourPointSevenGb = 5046586572L; // 4.7 GB

            // 3. SI pesa 20 MB – 700 MB -> ⚠️ Posible (juego de CD, puzzle, remake 2D)
            if (sizeInBytes >= TwentyMb && sizeInBytes < SevenHundredMb)
            {
                res.IsValid = true;
                res.StatusText = isCompressed ? "Listo (CD Comprimido)" : "Listo (CD / 2D)";
                res.StatusColor = Color.FromArgb(52, 152, 219); // Azul celeste / Cyan
                res.Category = "CD / 2D";
                return res;
            }

            // 4. SI pesa 700 MB – 4.7 GB -> ✅ Normal (CD o DVD)
            if (sizeInBytes >= SevenHundredMb && sizeInBytes <= FourPointSevenGb)
            {
                res.IsValid = true;
                res.StatusText = isCompressed ? "Listo (Comprimido)" : "Listo";
                res.StatusColor = Color.FromArgb(46, 204, 113); // Verde
                res.Category = "Normal";
                return res;
            }

            // 5. SI pesa > 4.7 GB -> ⚠️ DVD multi-capa o multi-disc
            res.IsValid = true;
            res.StatusText = isCompressed ? "Listo (DVD9 Comprimido)" : "Listo (DVD9 Doble Capa)";
            res.StatusColor = Color.FromArgb(241, 196, 15); // Amarillo / Dorado
            res.Category = "DVD9 Multi-capa";
            return res;
        }

        public class ArchiveFileInfo
        {
            public string InternalName = "";
            public long UncompressedSize = 0;
            public bool IsValidPs2Game = true;
            public string DetectedSerial = "";
            public bool HasSystemCnf = true;
        }

        public static ArchiveFileInfo InspectArchive(string archivePath, string toolsDir)
        {
            ArchiveFileInfo result = new ArchiveFileInfo();
            if (!File.Exists(archivePath)) return result;

            string sevenZip = Path.Combine(toolsDir, "7za.exe");
            if (!File.Exists(sevenZip)) return result;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = sevenZip,
                    Arguments = string.Format("l \"{0}\"", archivePath),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(3000);

                    string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool inListing = false;
                    long maxDiscSize = 0;
                    string bestDiscName = "";
                    bool foundSystemCnf = false;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("-------------------"))
                        {
                            inListing = !inListing;
                            continue;
                        }

                        if (inListing && line.Length > 53)
                        {
                            string dataPart = line.Substring(26).Trim();
                            string[] tokens = dataPart.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length >= 3)
                            {
                                long size;
                                if (long.TryParse(tokens[0], out size))
                                {
                                    string entryName = line.Substring(53).Trim();
                                    string entryExt = Path.GetExtension(entryName).ToLower();

                                    if (entryName.EndsWith("SYSTEM.CNF", StringComparison.OrdinalIgnoreCase))
                                    {
                                        foundSystemCnf = true;
                                    }

                                    if (IsDirectFormat(entryExt))
                                    {
                                        if (size > maxDiscSize)
                                        {
                                            maxDiscSize = size;
                                            bestDiscName = entryName;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (maxDiscSize > 0)
                    {
                        result.InternalName = bestDiscName;
                        result.UncompressedSize = maxDiscSize;
                        // Mínimo 20 MB para ser un juego real de PS2 (CD o DVD)
                        result.IsValidPs2Game = (maxDiscSize >= 20L * 1024L * 1024L);
                        result.HasSystemCnf = true; // El ISO interno se comprobará al extraer

                        string s = IsoReader.DetectSerialFromName(bestDiscName);
                        if (!string.IsNullOrEmpty(s)) result.DetectedSerial = s;
                    }
                    else
                    {
                        // Si no encontró imagen de disco interna o es archivo pequeño
                        FileInfo fi = new FileInfo(archivePath);
                        result.UncompressedSize = fi.Length;
                        result.IsValidPs2Game = (fi.Length >= 20L * 1024L * 1024L);
                        result.HasSystemCnf = foundSystemCnf;
                    }
                }
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Si el juego está en formato comprimido (.gz, .7z, .zip o .chd),
        /// lo descomprime automáticamente a una imagen temporal utilizable.
        /// </summary>
        public static async Task<string> PrepareImageAsync(string inputPath, string toolsDir, Action<string> log, Action<int> progress = null)
        {
            if (!File.Exists(inputPath)) return null;

            string ext = Path.GetExtension(inputPath).ToLower();

            // 1. Formatos directos 1:1
            if (IsDirectFormat(ext))
            {
                return inputPath;
            }

            // 2. Archivos comprimidos estándar (.gz, .7z, .zip)
            if (IsCompressedArchive(ext))
            {
                string sevenZip = Path.Combine(toolsDir, "7za.exe");
                if (!File.Exists(sevenZip))
                {
                    if (log != null) log("[ERROR] No se encontro '7za.exe' en la carpeta tools/ para descomprimir el archivo.");
                    return null;
                }

                string tempExtractDir = Path.Combine(Path.GetTempPath(), "PS2_Extract_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempExtractDir);

                if (log != null) log(string.Format("[INFO] Descomprimiendo archivo {0} (usando todos los nucleos de CPU)...", Path.GetFileName(inputPath)));

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = sevenZip,
                    Arguments = string.Format("x \"{0}\" -o\"{1}\" -y -aoa -mmt=on -bsp1", inputPath, tempExtractDir),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process proc = new Process())
                {
                    proc.StartInfo = psi;
                    proc.OutputDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(e.Data)) return;
                        Match m = Regex.Match(e.Data, @"(\d{1,3})%");
                        if (m.Success && progress != null)
                        {
                            int val;
                            if (int.TryParse(m.Groups[1].Value, out val))
                            {
                                progress(Math.Min(100, Math.Max(0, val)));
                            }
                        }
                    };
                    proc.ErrorDataReceived += (s, e) => { };

                    proc.Start();
                    try { proc.PriorityClass = ProcessPriorityClass.AboveNormal; } catch { }
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    await Task.Run(() => proc.WaitForExit());
                }

                if (progress != null) progress(100);

                // Buscar la imagen extraída
                string[] extracted = Directory.GetFiles(tempExtractDir, "*.*");
                foreach (string f in extracted)
                {
                    string fExt = Path.GetExtension(f).ToLower();
                    if (IsDirectFormat(fExt))
                    {
                        if (log != null) log(string.Format("[SUCCESS] Imagen extraida exitosamente: {0}", Path.GetFileName(f)));
                        return f;
                    }
                }

                if (log != null) log("[ERROR] No se encontro ninguna imagen ISO/BIN/IMG dentro del archivo comprimido.");
                return null;
            }

            // 3. Formato CHD (MAME / PCSX2)
            if (IsChd(ext))
            {
                string chdman = Path.Combine(toolsDir, "chdman.exe");
                if (!File.Exists(chdman))
                {
                    if (log != null)
                    {
                        log("[WARN] El archivo es formato CHD comprimido.");
                        log("[INFO] Para auto-descomprimir archivos .chd, coloca 'chdman.exe' dentro de la carpeta 'tools/'.");
                    }
                    return null;
                }

                string tempDir = Path.Combine(Path.GetTempPath(), "PS2_CHD_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDir);
                string outIso = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(inputPath) + ".iso");

                if (log != null) log(string.Format("[INFO] Descomprimiendo CHD con chdman: {0}...", Path.GetFileName(inputPath)));

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = chdman,
                    Arguments = string.Format("extractdvd -i \"{0}\" -o \"{1}\"", inputPath, outIso),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process proc = new Process())
                {
                    proc.StartInfo = psi;
                    proc.OutputDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(e.Data)) return;
                        Match m = Regex.Match(e.Data, @"(\d{1,3})%");
                        if (m.Success && progress != null)
                        {
                            int val;
                            if (int.TryParse(m.Groups[1].Value, out val))
                            {
                                progress(Math.Min(100, Math.Max(0, val)));
                            }
                        }
                    };
                    proc.ErrorDataReceived += (s, e) => { };

                    proc.Start();
                    try { proc.PriorityClass = ProcessPriorityClass.AboveNormal; } catch { }
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    await Task.Run(() => proc.WaitForExit());
                }

                if (progress != null) progress(100);

                if (File.Exists(outIso))
                {
                    if (log != null) log("[SUCCESS] Archivo CHD convertido a ISO temporal.");
                    return outIso;
                }
            }

            return null;
        }
    }
}
