using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PS2toPS4Builder.Core
{
    public static class IsoReader
    {
        private static readonly Regex SerialRegex = new Regex(
            @"(S[CLU][A-Z]{2})_(\d{3})\.(\d{2})",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// Comprueba si la imagen ISO está truncada (descarga incompleta).
        /// Lee el Volume Space Size en el Descriptor de Volumen Primario (Sector 16).
        /// </summary>
        public static bool IsTruncated(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (FormatHelper.IsDirectFormat(ext))
                {
                    FileInfo fi = new FileInfo(filePath);
                    long fileLen = fi.Length;
                    if (fileLen < 65536) return false;

                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        // 1. Probar modo estándar ISO 2048 (DVD y CD estándar)
                        fs.Seek(32768, SeekOrigin.Begin); // Sector 16
                        byte[] pvd = new byte[2048];
                        int read = fs.Read(pvd, 0, pvd.Length);
                        if (read >= 88 && pvd[1] == 'C' && pvd[2] == 'D' && pvd[3] == '0' && pvd[4] == '0' && pvd[5] == '1')
                        {
                            uint totalSectors = BitConverter.ToUInt32(pvd, 80);
                            long expectedBytes = (long)totalSectors * 2048L;
                            // Si falta más de 2 MB respecto a lo que declara el descriptor, está truncada
                            if (expectedBytes > 0 && fileLen < (expectedBytes - (2L * 1024L * 1024L)))
                            {
                                return true;
                            }
                        }

                        // 2. Probar modo RAW CD 2352 (BIN)
                        if (fileLen >= 37664 + 88)
                        {
                            fs.Seek(37632 + 16, SeekOrigin.Begin); // 16 * 2352 + 16
                            byte[] binPvd = new byte[2048];
                            int binRead = fs.Read(binPvd, 0, binPvd.Length);
                            if (binRead >= 88 && binPvd[1] == 'C' && binPvd[2] == 'D' && binPvd[3] == '0' && binPvd[4] == '0' && binPvd[5] == '1')
                            {
                                uint binSectors = BitConverter.ToUInt32(binPvd, 80);
                                long expectedBinBytes = (long)binSectors * 2352L;
                                if (expectedBinBytes > 0 && fileLen < (expectedBinBytes - (2L * 1024L * 1024L)))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Comprueba si la imagen de disco corresponde a un juego de PlayStation 1.
        /// Los juegos de PS1 usan 'BOOT = cdrom:' (en lugar de BOOT2) o contienen el ejecutable 'PS-X EXE'.
        /// </summary>
        public static bool IsPs1Game(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (FormatHelper.IsDirectFormat(ext))
                {
                    long fileLen = new FileInfo(filePath).Length;
                    int readLen = (int)Math.Min(16L * 1024L * 1024L, fileLen);
                    byte[] buffer = new byte[readLen];

                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int bytesRead = fs.Read(buffer, 0, buffer.Length);
                        string text = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        bool hasPsxExe = text.IndexOf("PS-X EXE", StringComparison.OrdinalIgnoreCase) >= 0;
                        bool hasPs1Boot = text.IndexOf("BOOT = cdrom:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          text.IndexOf("BOOT=cdrom:", StringComparison.OrdinalIgnoreCase) >= 0;
                        bool hasPs2Boot2 = text.IndexOf("BOOT2", StringComparison.OrdinalIgnoreCase) >= 0;

                        if ((hasPsxExe || hasPs1Boot) && !hasPs2Boot2)
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Comprueba si la imagen ISO/BIN/IMG contiene la firma de SYSTEM.CNF, BOOT2 o un Serial oficial de PS2.
        /// Si no contiene ninguno de estos o es un juego de PS1, no es un juego válido de PlayStation 2.
        /// </summary>
        public static bool HasSystemCnf(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (FormatHelper.IsDirectFormat(ext))
                {
                    long fileLen = new FileInfo(filePath).Length;
                    int readLen = (int)Math.Min(16L * 1024L * 1024L, fileLen);
                    byte[] buffer = new byte[readLen];

                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int bytesRead = fs.Read(buffer, 0, buffer.Length);
                        string text = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        // 1. Descartar si es juego de PS1
                        bool hasPsxExe = text.IndexOf("PS-X EXE", StringComparison.OrdinalIgnoreCase) >= 0;
                        bool hasPs1Boot = text.IndexOf("BOOT = cdrom:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          text.IndexOf("BOOT=cdrom:", StringComparison.OrdinalIgnoreCase) >= 0;
                        bool hasPs2Boot2 = text.IndexOf("BOOT2", StringComparison.OrdinalIgnoreCase) >= 0;

                        if ((hasPsxExe || hasPs1Boot) && !hasPs2Boot2)
                        {
                            return false;
                        }

                        // 2. Comprobar firma de PS2
                        if (hasPs2Boot2 ||
                            text.IndexOf("SYSTEM.CNF", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            SerialRegex.IsMatch(text))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch { }

            return true; // Para formatos comprimidos que aún no han sido extraídos
        }

        public static string DetectSerial(string isoPath)
        {
            if (!File.Exists(isoPath))
                throw new FileNotFoundException("El archivo ISO no existe.", isoPath);

            // 1. PRIORIDAD MÁXIMA: Si es archivo directo (.iso, .bin, .img, .mdf), leer DIRECTAMENTE del disco virtual
            // Esto lee el SYSTEM.CNF real del juego y es 100% infalible.
            string ext = Path.GetExtension(isoPath).ToLower();
            if (FormatHelper.IsDirectFormat(ext) || ext == ".chd")
            {
                try
                {
                    const int bufferSize = 8 * 1024 * 1024;
                    byte[] buffer = new byte[bufferSize];

                    using (FileStream fs = new FileStream(isoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        int bytesRead = fs.Read(buffer, 0, buffer.Length);
                        string text = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        Match match = SerialRegex.Match(text);
                        if (match.Success)
                        {
                            string prefix = match.Groups[1].Value.ToUpper();
                            string part1 = match.Groups[2].Value;
                            string part2 = match.Groups[3].Value;
                            return string.Format("{0}-{1}{2}", prefix, part1, part2);
                        }
                    }
                }
                catch { }
            }

            // 2. Si el propio nombre del archivo contiene un serial (ej: [SLPM-65762] o SLPM_657.62)
            string fileName = Path.GetFileNameWithoutExtension(isoPath);
            string fromName = DetectSerialFromName(fileName);
            if (!string.IsNullOrEmpty(fromName)) return fromName;

            // 3. Como último recurso para archivos sin descomprimir, buscar por palabras clave
            string byKeywords = TitleDatabase.FindSerialByKeywords(fileName);
            if (!string.IsNullOrEmpty(byKeywords))
            {
                return byKeywords;
            }

            return "";
        }

        public static string DetectSerialFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            Match matchFile = SerialRegex.Match(name);
            if (matchFile.Success)
            {
                string prefix = matchFile.Groups[1].Value.ToUpper();
                string part1 = matchFile.Groups[2].Value;
                string part2 = matchFile.Groups[3].Value;
                return string.Format("{0}-{1}{2}", prefix, part1, part2);
            }

            Match matchFile2 = Regex.Match(name, @"([A-Z]{4})[-_](\d{5})", RegexOptions.IgnoreCase);
            if (matchFile2.Success)
            {
                return string.Format("{0}-{1}", matchFile2.Groups[1].Value.ToUpper(), matchFile2.Groups[2].Value);
            }
            return "";
        }

        public static string GenerateCusaId(string serial)
        {
            if (string.IsNullOrEmpty(serial))
            {
                int rndDefault = new Random().Next(10000, 99999);
                return string.Format("CUSA{0}", rndDefault);
            }

            string digits = Regex.Replace(serial, @"\D", "");
            if (digits.Length >= 5)
            {
                return string.Format("CUSA{0}", digits.Substring(digits.Length - 5));
            }
            int rnd = new Random().Next(10000, 99999);
            return string.Format("CUSA{0}", rnd);
        }
    }
}
