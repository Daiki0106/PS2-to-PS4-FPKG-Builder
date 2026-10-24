using System.IO;
using System.Text;

namespace PS2toPS4Builder.Core
{
    public static class EmulatorConfig
    {
        public static void GenerateConfigFile(string targetPath, bool widescreen, bool fastLoad, string customArgs)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# PS2 to PS4 Emulator Configuration");
            sb.AppendLine("# Generado automaticamente por PS2toPS4Builder");
            sb.AppendLine();

            if (widescreen)
            {
                sb.AppendLine("--ps2-aspect-ratio=16:9");
            }
            else
            {
                sb.AppendLine("--ps2-aspect-ratio=4:3");
            }

            if (fastLoad)
            {
                sb.AppendLine("--cdvd-fast-read=1");
            }

            if (!string.IsNullOrEmpty(customArgs) && customArgs.Trim().Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("# Argumentos personalizados");
                sb.AppendLine(customArgs.Trim());
            }

            File.WriteAllText(targetPath, sb.ToString(), Encoding.UTF8);
        }
    }
}
