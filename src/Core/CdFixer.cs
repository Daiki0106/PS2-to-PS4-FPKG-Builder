using System;
using System.IO;
using System.Text;

namespace PS2toPS4Builder.Core
{
    public static class CdFixer
    {
        private const int CdPvdOffset = 37664; // Offset del Primary Volume Descriptor en CD 2352 (16 * 2352 + 32)
        private const int LimgSectorSize = 16384; // 16 KB

        public static bool IsPs2Cd(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length < 40000) return false;

                    // Comprobar si el sector 16 corresponde al modo 2352 de CD
                    fs.Seek(CdPvdOffset, SeekOrigin.Begin);
                    byte[] buffer = new byte[11];
                    int read = fs.Read(buffer, 0, buffer.Length);
                    if (read == 11)
                    {
                        string sysId = Encoding.ASCII.GetString(buffer);
                        if (sysId == "PLAYSTATION")
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool HasLimg(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length < LimgSectorSize) return false;

                    fs.Seek(fs.Length - LimgSectorSize, SeekOrigin.Begin);
                    byte[] buffer = new byte[4];
                    int read = fs.Read(buffer, 0, buffer.Length);
                    if (read == 4)
                    {
                        string magic = Encoding.ASCII.GetString(buffer);
                        return magic == "LIMG";
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool ApplyLimgFix(string sourceFile, string destFile, string limgTemplatePath)
        {
            try
            {
                // 1. Copiar imagen ISO / BIN al destino
                File.Copy(sourceFile, destFile, true);

                // 2. Extraer el conteo de sectores del disco CD (offset 37740 en PVD)
                byte[] sectorCountBytes = new byte[4];
                using (FileStream fs = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.Seek(37740, SeekOrigin.Begin);
                    fs.Read(sectorCountBytes, 0, 4);
                }

                // 3. Preparar cabecera LIMG (16 KB)
                byte[] limgBlock = new byte[LimgSectorSize];
                if (!string.IsNullOrEmpty(limgTemplatePath) && File.Exists(limgTemplatePath))
                {
                    byte[] templateBytes = File.ReadAllBytes(limgTemplatePath);
                    Array.Copy(templateBytes, limgBlock, Math.Min(templateBytes.Length, LimgSectorSize));
                }
                else
                {
                    // Fallback nativo
                    limgBlock[0] = 0x4C; // 'L'
                    limgBlock[1] = 0x49; // 'I'
                    limgBlock[2] = 0x4D; // 'M'
                    limgBlock[3] = 0x47; // 'G'
                    limgBlock[7] = 0x02; // Mode 2
                    limgBlock[14] = 0x09; // Sector 2352 (0x0930)
                    limgBlock[15] = 0x30;
                }

                // Inyectar el conteo de sectores en el LIMG
                Array.Copy(sectorCountBytes, 0, limgBlock, 8, 4);

                // 4. Anexar LIMG al final del archivo destino
                using (FileStream fsDest = new FileStream(destFile, FileMode.Append, FileAccess.Write))
                {
                    fsDest.Write(limgBlock, 0, limgBlock.Length);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
