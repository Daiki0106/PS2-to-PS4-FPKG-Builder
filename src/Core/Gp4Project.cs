using System;
using System.IO;
using System.Security;
using System.Text;

namespace PS2toPS4Builder.Core
{
    public static class Gp4Project
    {
        public static void GenerateGp4(string projectFilePath, string contentId, string stagingDirectory)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
            sb.AppendLine("<psproject fmt=\"gp4\" version=\"1000\">");
            sb.AppendLine("  <volume>");
            sb.AppendLine("    <volume_type>pkg_ps4_app</volume_type>");
            sb.AppendLine("    <volume_id>PS4VOLUME</volume_id>");
            sb.AppendLine("    <package_sub_type>0</package_sub_type>");
            sb.AppendLine(string.Format("    <content_id>{0}</content_id>", SecurityElement.Escape(contentId ?? "")));
            sb.AppendLine("    <passcode>00000000000000000000000000000000</passcode>");
            sb.AppendLine("  </volume>");
            sb.AppendLine("  <files>");

            string[] allFiles = Directory.GetFiles(stagingDirectory, "*.*", SearchOption.AllDirectories);
            foreach (string file in allFiles)
            {
                string relativePath = file.Substring(stagingDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string targetPath = relativePath.Replace('\\', '/');

                sb.AppendLine(string.Format("    <file targpath=\"{0}\" origpath=\"{1}\" />", 
                    SecurityElement.Escape(targetPath), 
                    SecurityElement.Escape(file)));
            }

            sb.AppendLine("  </files>");
            sb.AppendLine("</psproject>");

            File.WriteAllText(projectFilePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
