using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PS2toPS4Builder.Core
{
    public static class SfoHelper
    {
        public static string GenerateSfoXml(string titleId, string gameTitle, string contentId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
            sb.AppendLine("<paramsfo>");
            sb.AppendLine("  <param key=\"APP_TYPE\">1</param>");
            sb.AppendLine("  <param key=\"APP_VER\">01.00</param>");
            sb.AppendLine("  <param key=\"ATTRIBUTE\">0</param>");
            sb.AppendLine("  <param key=\"CATEGORY\">gd</param>");
            sb.AppendLine(string.Format("  <param key=\"CONTENT_ID\">{0}</param>", contentId));
            sb.AppendLine("  <param key=\"DOWNLOAD_DATA_SIZE\">0</param>");
            sb.AppendLine("  <param key=\"FORMAT\">obs</param>");
            sb.AppendLine("  <param key=\"PARENTAL_LEVEL\">5</param>");
            sb.AppendLine("  <param key=\"REMOTE_PLAY_KEY_ASSIGN\">1</param>");
            sb.AppendLine("  <param key=\"SYSTEM_VER\">0</param>");
            sb.AppendLine(string.Format("  <param key=\"TITLE\">{0}</param>", gameTitle));
            sb.AppendLine(string.Format("  <param key=\"TITLE_ID\">{0}</param>", titleId));
            sb.AppendLine("  <param key=\"VERSION\">01.00</param>");
            sb.AppendLine("</paramsfo>");
            return sb.ToString();
        }

        public static bool BuildSfo(string orbisPubCmdPath, string xmlPath, string outputSfoPath)
        {
            if (!File.Exists(orbisPubCmdPath)) return false;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = orbisPubCmdPath,
                Arguments = string.Format("sfo_create \"{0}\" \"{1}\"", xmlPath, outputSfoPath),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0 && File.Exists(outputSfoPath);
            }
        }
    }
}
