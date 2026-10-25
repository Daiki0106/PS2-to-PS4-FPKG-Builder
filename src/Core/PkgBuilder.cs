using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS2toPS4Builder.Core
{
    public class BuildOptions
    {
        public string IsoPath;
        public string GameTitle;
        public string TitleId;
        public string ContentId;
        public string Ps2Serial;
        public string IconPath;
        public string BackgroundPath;
        public bool Widescreen;
        public bool FastLoad;
        public string CustomConfig;
        public string LuaScriptPath;
        public string TemplatePath;
        public string OrbisToolPath;
        public string OutputPkgPath;

        public BuildOptions()
        {
            IsoPath = "";
            GameTitle = "";
            TitleId = "CUSA00001";
            ContentId = "";
            Ps2Serial = "";
            IconPath = "";
            BackgroundPath = "";
            Widescreen = false;
            FastLoad = true;
            CustomConfig = "";
            LuaScriptPath = "";
            TemplatePath = "";
            OrbisToolPath = "";
            OutputPkgPath = "";
        }
    }

    public class PkgBuilder
    {
        public event Action<int> OnProgress;
        public event Action<string> OnLog;

        private void TriggerLog(string message)
        {
            Action<string> handler = OnLog;
            if (handler != null) handler(message);
        }

        private void TriggerProgress(int percent)
        {
            Action<int> handler = OnProgress;
            if (handler != null) handler(percent);
        }

        public async Task<bool> BuildAsync(BuildOptions options)
        {
            return await Task.Run(() => BuildInternal(options));
        }

        private bool BuildInternal(BuildOptions options)
        {
            string tempDir = "";
            try
            {
                TriggerLog("[INFO] Iniciando verificacion previa de archivos...");

                if (!File.Exists(options.IsoPath))
                    throw new FileNotFoundException("No se encontro la imagen ISO de PS2.", options.IsoPath);

                if (!File.Exists(options.OrbisToolPath))
                    throw new FileNotFoundException("No se encontro 'orbis-pub-cmd.exe'. Configura su ruta en Tools.", options.OrbisToolPath);

                if (!Directory.Exists(options.TemplatePath))
                    throw new DirectoryNotFoundException("No se encontro la carpeta de plantilla del emulador: " + options.TemplatePath);

                // 1. Directorio temporal de trabajo
                tempDir = Path.Combine(Path.GetTempPath(), "PS2toPS4_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDir);
                TriggerLog("[INFO] Directorio temporal creado: " + tempDir);

                TriggerProgress(5);

                // 2. Copiar archivos base de plantilla
                TriggerLog("[INFO] Copiando archivos base del emulador de PS2...");
                CopyDirectory(options.TemplatePath, tempDir);

                string sceSysDir = Path.Combine(tempDir, "sce_sys");
                string imageDir = Path.Combine(tempDir, "image");
                if (!Directory.Exists(sceSysDir)) Directory.CreateDirectory(sceSysDir);
                if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);

                TriggerProgress(15);

                // 3. Copiar la ISO con autodetección de CD y parche LIMG
                string destIsoPath = Path.Combine(imageDir, "disc01.iso");
                string limgBinPath = Path.Combine(Path.GetDirectoryName(options.OrbisToolPath), "LIMG.bin");

                bool isTempExtracted = options.IsoPath.IndexOf("PS2_Extract_", StringComparison.OrdinalIgnoreCase) >= 0;

                if (CdFixer.IsPs2Cd(options.IsoPath) && !CdFixer.HasLimg(options.IsoPath))
                {
                    TriggerLog("[INFO] ¡Juego en formato CD detectado! Aplicando parche LIMG para compatibilidad PS4...");
                    bool limgOk = CdFixer.ApplyLimgFix(options.IsoPath, destIsoPath, limgBinPath);
                    if (limgOk)
                    {
                        TriggerLog("[SUCCESS] Parche LIMG inyectado exitosamente (CD -> DVD compatible).");
                        if (isTempExtracted)
                        {
                            try { File.Delete(options.IsoPath); } catch { }
                        }
                    }
                    else
                    {
                        TriggerLog("[WARN] No se pudo inyectar LIMG, copiando como imagen estándar.");
                        File.Copy(options.IsoPath, destIsoPath, true);
                    }
                }
                else
                {
                    TriggerLog("[INFO] Copiando imagen ISO al paquete (image/disc01.iso)...");
                    File.Copy(options.IsoPath, destIsoPath, true);
                }

                TriggerProgress(35);

                // 4. Copiar y auto-adaptar imagenes multimedia (icon0.png y pic1.png)
                if (!string.IsNullOrEmpty(options.IconPath) && File.Exists(options.IconPath))
                {
                    TriggerLog("[INFO] Auto-redimensionando icono a 512x512 PNG...");
                    ImageHelper.ConvertAndResize(options.IconPath, Path.Combine(sceSysDir, "icon0.png"), 512, 512);
                    TriggerLog("[INFO] Icono personalizado icon0.png configurado correctamente.");
                }

                if (!string.IsNullOrEmpty(options.BackgroundPath) && File.Exists(options.BackgroundPath))
                {
                    TriggerLog("[INFO] Auto-redimensionando fondo a 1920x1080 PNG...");
                    ImageHelper.ConvertAndResize(options.BackgroundPath, Path.Combine(sceSysDir, "pic1.png"), 1920, 1080);
                    TriggerLog("[INFO] Fondo personalizado pic1.png configurado correctamente.");
                }

                // 5. Generar archivo config-emu-ps4.txt
                string configPath = Path.Combine(tempDir, "config-emu-ps4.txt");
                bool hasLua = !string.IsNullOrEmpty(options.LuaScriptPath) && File.Exists(options.LuaScriptPath);
                EmulatorConfig.GenerateConfigFile(configPath, options.Widescreen, options.FastLoad, options.CustomConfig, options.Ps2Serial, hasLua);
                TriggerLog("[INFO] Configuracion del emulador generada.");

                // 5b. Copiar script Lua si existe
                if (!string.IsNullOrEmpty(options.LuaScriptPath) && File.Exists(options.LuaScriptPath))
                {
                    try
                    {
                        string luaDest = Path.Combine(tempDir, "config.lua");
                        File.Copy(options.LuaScriptPath, luaDest, true);
                        TriggerLog(string.Format("[INFO] Script de memoria Lua ({0}) inyectado en el paquete.", Path.GetFileName(options.LuaScriptPath)));
                    }
                    catch (Exception ex)
                    {
                        TriggerLog(string.Format("[WARN] No se pudo copiar script Lua: {0}", ex.Message));
                    }
                }

                // 6. Generar param.sfo
                string sfoXmlPath = Path.Combine(tempDir, "param.sfo.xml");
                string outputSfo = Path.Combine(sceSysDir, "param.sfo");
                string sfoXmlContent = SfoHelper.GenerateSfoXml(options.TitleId, options.GameTitle, options.ContentId);
                File.WriteAllText(sfoXmlPath, sfoXmlContent);

                TriggerLog("[INFO] Creando metadatos param.sfo...");
                bool sfoOk = SfoHelper.BuildSfo(options.OrbisToolPath, sfoXmlPath, outputSfo);
                if (!sfoOk && !File.Exists(outputSfo))
                {
                    TriggerLog("[WARN] No se pudo crear param.sfo con sfo_create. Verifique orbis-pub-cmd.");
                }

                // Eliminar archivo XML temporal antes de escanear archivos
                if (File.Exists(sfoXmlPath)) File.Delete(sfoXmlPath);

                TriggerProgress(45);

                // 7. Generar proyecto GP4
                string toolsDir = Path.GetDirectoryName(options.OrbisToolPath);
                string gengp4Path = Path.Combine(toolsDir, "gengp4.exe");
                string gp4Path = tempDir + ".gp4";

                if (File.Exists(gengp4Path))
                {
                    TriggerLog("[INFO] Generando estructura de proyecto GP4 con gengp4...");
                    ProcessStartInfo gengp4Psi = new ProcessStartInfo
                    {
                        FileName = gengp4Path,
                        Arguments = string.Format("\"{0}\"", tempDir),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using (Process gp4Proc = new Process())
                    {
                        gp4Proc.StartInfo = gengp4Psi;
                        gp4Proc.Start();
                        gp4Proc.WaitForExit();
                    }

                    if (File.Exists(gp4Path))
                    {
                        // Asegurar default_id="0" para compatibilidad con orbis-pub-cmd
                        string gp4Text = File.ReadAllText(gp4Path);
                        gp4Text = gp4Text.Replace("default_id=\"1\"", "default_id=\"0\"");
                        File.WriteAllText(gp4Path, gp4Text);
                    }
                }

                if (!File.Exists(gp4Path))
                {
                    gp4Path = Path.Combine(tempDir, "project.gp4");
                    TriggerLog("[INFO] Generando estructura GP4 integrada...");
                    Gp4Project.GenerateGp4(gp4Path, options.ContentId, tempDir);
                }

                TriggerProgress(55);

                // 8. Compilar con orbis-pub-cmd
                string outDir = Path.GetDirectoryName(options.OutputPkgPath);
                if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
                {
                    try { Directory.CreateDirectory(outDir); } catch { }
                }

                TriggerLog("[INFO] Compilando imagen PKG final...");
                bool buildSuccess = RunOrbisPubCmd(options.OrbisToolPath, gp4Path, options.OutputPkgPath);

                // Limpiar archivo gp4 temporal si se creo fuera de tempDir
                try
                {
                    if (File.Exists(tempDir + ".gp4")) File.Delete(tempDir + ".gp4");
                }
                catch { }

                if (buildSuccess && File.Exists(options.OutputPkgPath))
                {
                    TriggerProgress(100);
                    TriggerLog("[SUCCESS] ¡PKG creado exitosamente en: " + options.OutputPkgPath + "!");
                    return true;
                }
                else
                {
                    TriggerLog("[ERROR] Error durante la compilacion de la imagen PKG.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                TriggerLog("[ERROR CRITICO] " + ex.Message);
                return false;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                        TriggerLog("[INFO] Archivos temporales eliminados.");
                    }
                }
                catch { }
            }
        }

        private bool RunOrbisPubCmd(string orbisPath, string gp4Path, string outputPkgPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = orbisPath,
                Arguments = string.Format("img_create \"{0}\" \"{1}\"", gp4Path, outputPkgPath),
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = psi;

                process.OutputDataReceived += (s, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data)) return;
                    TriggerLog(e.Data);

                    // Escuchar porcentajes o progreso de barras
                    Match match = Regex.Match(e.Data, @"\[\s*(\d{1,3})%\]");
                    if (match.Success)
                    {
                        int percent;
                        if (int.TryParse(match.Groups[1].Value, out percent))
                        {
                            int scaled = 55 + (int)(percent * 0.44);
                            TriggerProgress(scaled);
                        }
                    }
                    else if (e.Data.Contains("Writing internal image"))
                    {
                        TriggerProgress(85);
                    }
                    else if (e.Data.Contains("Calculating image digest"))
                    {
                        TriggerProgress(95);
                    }
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        TriggerLog("[STDERR] " + e.Data);
                    }
                };

                process.Start();
                try { process.PriorityClass = ProcessPriorityClass.AboveNormal; } catch { }
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                string dirToCreate = dir.Replace(sourceDir, destDir);
                if (!Directory.Exists(dirToCreate)) Directory.CreateDirectory(dirToCreate);
            }

            foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                string fileToCopy = file.Replace(sourceDir, destDir);
                File.Copy(file, fileToCopy, true);
            }
        }
    }
}
