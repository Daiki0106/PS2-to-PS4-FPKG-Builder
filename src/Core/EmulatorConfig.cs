using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PS2toPS4Builder.Core
{
    public static class EmulatorConfig
    {
        public static void GenerateConfigFile(string targetPath, bool widescreen, bool fastLoad, string customArgs, string serial = null, bool hasLuaScript = false)
        {
            string baseConfig = "";
            if (File.Exists(targetPath))
            {
                try
                {
                    baseConfig = File.ReadAllText(targetPath, Encoding.UTF8);
                }
                catch { }
            }

            if (string.IsNullOrEmpty(baseConfig))
            {
                // Plantilla oficial estándar completa (imprescindible para que el emulador arranque)
                StringBuilder defaultSb = new StringBuilder();
                defaultSb.AppendLine("--path-vmc=\"/tmp/vmc\"");
                defaultSb.AppendLine("--path-emulog=\"/tmp/recordings\"");
                defaultSb.AppendLine("--host-osd=0");
                defaultSb.AppendLine("--host-audio=1");
                defaultSb.AppendLine("--host-display-mode=full");
                defaultSb.AppendLine("--ps2-title-id=SLES-00000");
                defaultSb.AppendLine("--ps2-lang=system");
                defaultSb.AppendLine("--gs-uprender=2x2");
                defaultSb.AppendLine("--gs-upscale=EdgeSmooth");
                defaultSb.AppendLine("--rom=\"PS20220WD20050620.crack\"");
                baseConfig = defaultSb.ToString();
            }

            // 1. Limpiar cualquier parámetro inválido anterior si estuviera presente
            baseConfig = Regex.Replace(baseConfig, @"(?m)^\s*--ps2-aspect-ratio=.*$\r?\n?", "");
            baseConfig = Regex.Replace(baseConfig, @"(?m)^\s*--cdvd-fast-read=.*$\r?\n?", "");

            // 2. Asignar el Serial oficial de PS2 si se conoce (ej: SLES-55669)
            if (!string.IsNullOrEmpty(serial))
            {
                string normSerial = serial.Replace("_", "-").Trim().ToUpper();
                if (baseConfig.Contains("--ps2-title-id="))
                {
                    baseConfig = Regex.Replace(baseConfig, @"--ps2-title-id=\S+", "--ps2-title-id=" + normSerial);
                }
                else
                {
                    baseConfig += "\r\n--ps2-title-id=" + normSerial;
                }
            }

            // 3. Configurar relación de aspecto oficial (full para 16:9 o normal para 4:3)
            string displayMode = widescreen ? "--host-display-mode=full" : "--host-display-mode=normal";
            if (baseConfig.Contains("--host-display-mode="))
            {
                baseConfig = Regex.Replace(baseConfig, @"--host-display-mode=\S+", displayMode);
            }
            else
            {
                baseConfig += "\r\n" + displayMode;
            }

            // 4. Si hay script Lua inyectado, indicárselo al emulador
            if (hasLuaScript)
            {
                if (!baseConfig.Contains("--config-local-lua="))
                {
                    baseConfig += "\r\n--config-local-lua=\"/app0/config.lua\"";
                }
            }

            // 5. Agregar argumentos y parches comunitarios si existen
            if (!string.IsNullOrEmpty(customArgs) && customArgs.Trim().Length > 0)
            {
                baseConfig += "\r\n\r\n# Parches y configuracion comunitaria\r\n" + customArgs.Trim();
            }

            File.WriteAllText(targetPath, baseConfig.Trim() + "\r\n", Encoding.UTF8);
        }
    }
}
