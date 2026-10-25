using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS2toPS4Builder.Core;

namespace PS2toPS4Builder.UI
{
    public partial class MainForm : Form
    {
        private PkgBuilder _builder = new PkgBuilder();
        private string _appDir;
        private string _localArtPath = @"C:\Users\joamm\Downloads\OPLM_ART_2024_09\PS2";
        private string _currentCoverIcon = null;
        private string _currentDiscIcon = null;
        private bool _isBusy = false;
        private string _currentExtractedTempDir = null;

        public MainForm()
        {
            InitializeComponent();
            SetupApp();
        }

        private void SetupApp()
        {
            _appDir = AppDomain.CurrentDomain.BaseDirectory;

            // Inicializar base de datos de títulos oficiales de PS2
            string csvPath = Path.Combine(_appDir, "data", "titles.csv");
            TitleDatabase.Initialize(csvPath);

            // Inicializar base de datos de configuraciones comunitarias
            string configsDir = Path.Combine(_appDir, "data", "configs");
            ConfigDatabase.Initialize(configsDir);

            // Cargar emuladores disponibles en el ComboBox
            LoadEmulators();

            // Eventos del compilador
            _builder.OnLog += LogMessage;
            _builder.OnProgress += UpdateProgress;

            // Rutas por defecto
            txtOrbisPath.Text = Path.Combine(_appDir, "tools", "orbis-pub-cmd.exe");

            // Estado inicial del botón de compilación (desactivado hasta cargar una ISO válida)
            btnBuild.Enabled = false;
            btnBuild.BackColor = Color.FromArgb(60, 64, 70);
            btnBuild.Text = "SELECCIONA UNA ISO DE PS2 VÁLIDA";
            lblStatus.Text = "Arrastra un juego o haz clic en Examinar para comenzar.";

            // Desactivar botones de multimedia hasta tener ISO / Serial cargados
            btnOpenStudio.Enabled = false;
            btnOpenStudio.BackColor = Color.FromArgb(60, 64, 70);
            btnAutoCover.Enabled = false;

            // Cargar icono de la aplicación
            string icoPath = Path.Combine(_appDir, "app.ico");
            if (File.Exists(icoPath))
            {
                try { this.Icon = new Icon(icoPath); } catch { }
            }
            if (this.Icon == null)
            {
                try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            }

            // Punto 2: Forzar mayúsculas automáticas en Serial y Title ID
            txtSerial.CharacterCasing = CharacterCasing.Upper;
            txtTitleId.CharacterCasing = CharacterCasing.Upper;

            // Punto 1: Auto-actualizar nombre del PKG al editar el título del juego
            txtGameTitle.TextChanged += (s, e) =>
            {
                if (!_isBusy && !string.IsNullOrEmpty(txtOutputPath.Text))
                {
                    string dir = Path.GetDirectoryName(txtOutputPath.Text);
                    string newName = FormatHelper.SanitizeFileName(txtGameTitle.Text.Trim());
                    if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(dir))
                    {
                        txtOutputPath.Text = Path.Combine(dir, newName + ".pkg");
                    }
                }
            };

            // Punto 5: Configurar tooltips informativos
            SetupTooltips();

            // Soporte Drag & Drop
            this.AllowDrop = true;
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;

            // Deseleccionar campos al presionar Enter o Escape
            txtGameTitle.KeyDown += TextBox_EnterOrEscape_KeyDown;
            txtSerial.KeyDown += TextBox_EnterOrEscape_KeyDown;
            txtTitleId.KeyDown += TextBox_EnterOrEscape_KeyDown;
            txtOutputPath.KeyDown += TextBox_EnterOrEscape_KeyDown;

            // Deseleccionar al hacer clic afuera en cualquier parte de la ventana
            AttachUnfocusClick(this);
        }

        private void TextBox_EnterOrEscape_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                this.ActiveControl = null;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void AttachUnfocusClick(Control parent)
        {
            if (parent == null) return;

            if (!(parent is TextBox))
            {
                parent.MouseDown += (s, e) =>
                {
                    if (this.ActiveControl is TextBox)
                    {
                        this.ActiveControl = null;
                    }
                };
            }

            foreach (Control child in parent.Controls)
            {
                AttachUnfocusClick(child);
            }
        }

        private void SetupTooltips()
        {
            ToolTip tip = new ToolTip();
            tip.AutoPopDelay = 6000;
            tip.InitialDelay = 350;
            tip.ReshowDelay = 150;
            tip.ShowAlways = true;

            tip.SetToolTip(chkWidescreen, "Fuerza el formato panorámico 16:9 estirando o parchando el renderizado en PS4.");
            tip.SetToolTip(chkFastLoad, "Acelera significativamente la lectura del disco CDVD para cargas ultrarrápidas.");
            tip.SetToolTip(cmbEmulator, "Selecciona el motor de emulación base de Sony (Jak v2 es el más compatible con la mayoría de juegos).");
            tip.SetToolTip(btnOpenStudio, "Abre el editor integrado para copiar y pegar carátulas y fondos desde internet con Ctrl+V.");
            tip.SetToolTip(btnAutoCover, "Busca y descarga automáticamente la portada oficial y fondo panorámico desde internet.");
            tip.SetToolTip(pbIcon, "Haz clic para alternar entre la carátula frontal y la imagen del disco original.");
            tip.SetToolTip(btnBuild, "Compila el archivo FPKG final instalable en PlayStation 4.");
            tip.SetToolTip(btnOpenBatch, "Abre el conversor por lotes para procesar múltiples juegos de PS2 en cola automáticamente.");
            tip.SetToolTip(btnSendToPs4, "Envía e instala el PKG directamente a tu consola PS4 encendida por red local (WiFi/Ethernet).");
            tip.SetToolTip(btnTogglePaths, "Muestra u oculta las rutas técnicas avanzadas de orbis-pub-cmd y destino.");
            tip.SetToolTip(txtTitleId, "Identificador único del juego en PS4 (Formato oficial: CUSAxxxxx).");
            tip.SetToolTip(txtSerial, "Serial oficial del disco de PS2 (Ej: SLUS-21678 o SLES-50000).");
            tip.SetToolTip(txtGameTitle, "Título del juego visible en la interfaz de PS4 y en el nombre del archivo PKG.");
            tip.SetToolTip(lblPatchInfo, "Indica si se aplicará un parche comunitario optimizado (Widescreen, 60 FPS o corrección gráfica).");
        }

        private void LoadEmulators()
        {
            cmbEmulator.Items.Clear();

            string emusDir = Path.Combine(_appDir, "emus");
            if (Directory.Exists(emusDir))
            {
                string[] dirs = Directory.GetDirectories(emusDir);
                foreach (string dir in dirs)
                {
                    cmbEmulator.Items.Add(Path.GetFileName(dir));
                }
            }

            // Si hay carpeta template estándar
            string templateDir = Path.Combine(_appDir, "template");
            if (Directory.Exists(templateDir))
            {
                if (!cmbEmulator.Items.Contains("Plantilla por Defecto (Jak v2)"))
                {
                    cmbEmulator.Items.Insert(0, "Plantilla por Defecto (Jak v2)");
                }
            }

            if (cmbEmulator.Items.Count > 0)
            {
                cmbEmulator.SelectedIndex = 0;
            }
        }

        private string GetSelectedTemplatePath()
        {
            if (cmbEmulator.SelectedItem == null)
                return Path.Combine(_appDir, "template");

            string selected = cmbEmulator.SelectedItem.ToString();
            if (selected.StartsWith("Plantilla por Defecto"))
            {
                return Path.Combine(_appDir, "template");
            }

            string emuPath = Path.Combine(_appDir, "emus", selected);
            if (Directory.Exists(emuPath)) return emuPath;

            return Path.Combine(_appDir, "template");
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (_isBusy)
            {
                LogMessage("[WARN] ⚠️ Hay una operación en curso (descomprimiendo o compilando). Por favor espera.");
                return;
            }

            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    // Si se arrastran múltiples archivos o una carpeta, abrir Modo por Lotes
                    bool hasMultiple = files.Length > 1;
                    bool hasDirectory = false;
                    foreach (string item in files)
                    {
                        if (Directory.Exists(item)) { hasDirectory = true; break; }
                    }

                    if (hasMultiple || hasDirectory)
                    {
                        using (BatchForm bf = new BatchForm(_appDir, _localArtPath, txtOrbisPath.Text))
                        {
                            foreach (string item in files)
                            {
                                if (Directory.Exists(item)) bf.AddFolder(item);
                                else if (File.Exists(item)) bf.AddIso(item);
                            }
                            bf.ShowDialog(this);
                        }
                        return;
                    }

                    // Flujo normal de archivo único
                    string file = files[0];
                    string ext = Path.GetExtension(file).ToLower();

                    if (FormatHelper.IsSupportedExtension(ext))
                    {
                        LoadIso(file);
                    }
                    else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                    {
                        LoadImage(file);
                    }
                }
            }
        }

        private void btnOpenBatch_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                LogMessage("[WARN] ⚠️ Hay una operación en curso. Por favor espera.");
                return;
            }

            using (BatchForm bf = new BatchForm(_appDir, _localArtPath, txtOrbisPath.Text))
            {
                bf.ShowDialog(this);
            }
        }

        private void btnBrowseIso_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                LogMessage("[WARN] ⚠️ Hay una operación en curso. Por favor espera.");
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar Imagen de PS2 (ISO, BIN, IMG, MDF, GZ, 7Z, CHD)";
                ofd.Filter = FormatHelper.FileFilter;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadIso(ofd.FileName);
                }
            }
        }

        private void ClearMedia()
        {
            txtIconPath.Text = "";
            txtPicPath.Text = "";
            _currentCoverIcon = null;
            _currentDiscIcon = null;

            if (pbIcon.Image != null)
            {
                try { pbIcon.Image.Dispose(); } catch { }
                pbIcon.Image = null;
            }
            pbIcon.ImageLocation = null;

            if (pbPic.Image != null)
            {
                try { pbPic.Image.Dispose(); } catch { }
                pbPic.Image = null;
            }
            pbPic.ImageLocation = null;
        }

        private void SetUiBusyState(bool busy, string buttonText, Color buttonColor, string statusText = null)
        {
            _isBusy = busy;

            // Bloquear paneles completos (se ponen en gris y no son editables)
            groupBoxIso.Enabled = !busy;
            groupBoxMedia.Enabled = !busy;
            groupBoxSettings.Enabled = !busy;

            btnBuild.Enabled = !busy;
            btnBuild.Text = buttonText;
            btnBuild.BackColor = buttonColor;

            if (!string.IsNullOrEmpty(statusText))
            {
                lblStatus.Text = statusText;
            }
        }

        private async void LoadIso(string rawPath)
        {
            if (_isBusy)
            {
                LogMessage("[WARN] ⚠️ Ya hay una operación en curso. Por favor espera a que finalice.");
                return;
            }

            SetUiBusyState(true, "⏳ PREPARANDO JUEGO...", Color.FromArgb(41, 128, 185), string.Format("Descomprimiendo {0}...", Path.GetFileName(rawPath)));
            progressBar.Value = 0;
            lblProgress.Text = "0%";

            // Limpiar datos y multimedia del juego anterior
            txtGameTitle.Text = "";
            txtSerial.Text = "";
            txtTitleId.Text = "";
            txtOutputPath.Text = "";
            txtIsoPath.Text = string.Format("Cargando {0}...", Path.GetFileName(rawPath));
            lblPatchInfo.Text = "🛡️ Parche Comunitario: Pendiente...";
            lblPatchInfo.ForeColor = Color.DarkGray;
            ClearMedia();

            // Limpiar la extracción previa si existía para no acumular gigas
            if (!string.IsNullOrEmpty(_currentExtractedTempDir) && Directory.Exists(_currentExtractedTempDir))
            {
                try { Directory.Delete(_currentExtractedTempDir, true); } catch { }
                _currentExtractedTempDir = null;
            }

            string toolsDir = Path.Combine(_appDir, "tools");
            string isoPath = rawPath;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(rawPath);

            LogMessage("─────────────────────────────────────────────────────────────────────────────");
            LogMessage(string.Format("[INFO] 🎮 Preparando imagen: {0}", Path.GetFileName(rawPath)));

            bool loadSuccess = false;

            try
            {
                string ext = Path.GetExtension(rawPath).ToLower();
                if (FormatHelper.IsCompressedArchive(ext) || FormatHelper.IsChd(ext))
                {
                    lblStatus.Text = string.Format("Descomprimiendo {0}...", Path.GetFileName(rawPath));

                    isoPath = await FormatHelper.PrepareImageAsync(rawPath, toolsDir, LogMessage, p =>
                    {
                        if (InvokeRequired)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                progressBar.Value = Math.Min(100, Math.Max(0, p));
                                lblProgress.Text = string.Format("{0}%", p);
                            }));
                        }
                        else
                        {
                            progressBar.Value = Math.Min(100, Math.Max(0, p));
                            lblProgress.Text = string.Format("{0}%", p);
                        }
                    });
                    if (string.IsNullOrEmpty(isoPath) || !File.Exists(isoPath))
                    {
                        lblStatus.Text = "❌ No se encontró ninguna imagen de disco en el archivo.";
                        lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                        txtIsoPath.Text = "";
                        return;
                    }
                    _currentExtractedTempDir = Path.GetDirectoryName(isoPath);
                }

                txtIsoPath.Text = isoPath;

                FileInfo fi = new FileInfo(isoPath);
                bool isPs1 = IsoReader.IsPs1Game(isoPath);
                bool isTruncated = IsoReader.IsTruncated(isoPath);
                bool hasSystemCnf = IsoReader.HasSystemCnf(isoPath);

                string serial = IsoReader.DetectSerial(isoPath);
                txtSerial.Text = serial;
                txtTitleId.Text = IsoReader.GenerateCusaId(serial);

                // Buscar título oficial en la base de datos CSV
                string officialTitle = TitleDatabase.GetTitle(serial);
                bool isCheatOrUtility = FormatHelper.IsCheatOrUtility(fileNameWithoutExt, isoPath, serial, officialTitle);

                var discVal = FormatHelper.ValidateDisc(fi.Length, hasSystemCnf, false, isPs1, isTruncated, isCheatOrUtility);

                if (!discVal.IsValid)
                {
                    LogMessage(string.Format("[WARN] ⚠️ Verificación de disco: {0}", discVal.StatusText));
                    lblStatus.Text = discVal.StatusText;
                    lblStatus.ForeColor = discVal.StatusColor;
                    txtIsoPath.Text = "";
                    return;
                }

                LogMessage(string.Format("[INFO] Disco PS2 verificado: {0} ({1:0.00} GB)", discVal.Category, fi.Length / (1024.0 * 1024.0 * 1024.0)));
                lblStatus.Text = discVal.StatusText;
                lblStatus.ForeColor = discVal.StatusColor;

                string chosenTitle = "";
                if (isCheatOrUtility)
                {
                    // Limpiar etiquetas como (USA) (Unl)
                    string cleanTitle = Regex.Replace(fileNameWithoutExt, @"\s*\([^)]*\)", "").Trim();
                    chosenTitle = string.IsNullOrEmpty(cleanTitle) ? fileNameWithoutExt : cleanTitle;
                    txtGameTitle.Text = chosenTitle;

                    LogMessage(string.Format("[WARN] ⚠️ ARCHIVO IDENTIFICADO: Disco de Trucos / Utilidad no oficial (\"{0}\").", chosenTitle));
                    if (!string.IsNullOrEmpty(officialTitle) && !officialTitle.Equals(chosenTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        LogMessage(string.Format("[WARN] ⚠️ El serial detectado ({0}) suplanta la identidad de \"{1}\".", serial, officialTitle));
                    }
                }
                else
                {
                    chosenTitle = !string.IsNullOrEmpty(officialTitle) ? officialTitle : fileNameWithoutExt;
                    txtGameTitle.Text = chosenTitle;
                    if (!string.IsNullOrEmpty(officialTitle))
                    {
                        LogMessage(string.Format("[INFO] Reconocido en base de datos: \"{0}\" ({1})", officialTitle, serial));
                    }
                    else
                    {
                        LogMessage(string.Format("[INFO] ISO cargada. Serial: {0} -> Title ID: {1}", serial, txtTitleId.Text));

                        // Búsqueda en base de datos online si el serial no estaba en titles.csv local
                        if (!string.IsNullOrEmpty(serial))
                        {
                            Task.Run(async () =>
                            {
                                try
                                {
                                    string onlineTitle = await TitleDatabase.FetchTitleOnlineAsync(serial);
                                    if (!string.IsNullOrEmpty(onlineTitle))
                                    {
                                        this.BeginInvoke((MethodInvoker)delegate
                                        {
                                            if (string.IsNullOrEmpty(txtGameTitle.Text) ||
                                                txtGameTitle.Text.Equals(fileNameWithoutExt, StringComparison.OrdinalIgnoreCase) ||
                                                Regex.IsMatch(txtGameTitle.Text.Trim(), @"^[A-Za-z]{4}[-_]?\d{3,5}", RegexOptions.IgnoreCase))
                                            {
                                                txtGameTitle.Text = onlineTitle;
                                                LogMessage(string.Format("[SUCCESS] ¡Título oficial identificado vía online!: \"{0}\" ({1})", onlineTitle, serial));
                                            }
                                        });
                                    }
                                }
                                catch { }
                            });
                        }
                    }
                }

                // Comprobar parche comunitario optimizado
                if (ConfigDatabase.HasConfig(serial))
                {
                    string cfgText = ConfigDatabase.GetConfigText(serial);
                    string luaFile = ConfigDatabase.GetLuaPath(serial);
                    string patchType = (!string.IsNullOrEmpty(cfgText) && !string.IsNullOrEmpty(luaFile)) ? "CLI + LUA" :
                                       (!string.IsNullOrEmpty(luaFile) ? "Script LUA" : "Comandos CLI");

                    lblPatchInfo.Text = string.Format("🛡️ Parche Comunitario: ACTIVO ({0}) - Optimizaciones aplicadas", patchType);
                    lblPatchInfo.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113);

                    LogMessage(string.Format("[INFO] 🔥 ¡Configuración comunitaria detectada para {0}! (CLI: {1}, LUA: {2})",
                        serial,
                        !string.IsNullOrEmpty(cfgText) ? "SÍ" : "NO",
                        !string.IsNullOrEmpty(luaFile) ? "SÍ" : "NO"));
                }
                else
                {
                    lblPatchInfo.Text = "🛡️ Parche Comunitario: Estándar (El juego es compatible de fábrica sin parches extra)";
                    lblPatchInfo.ForeColor = System.Drawing.Color.DarkGray;
                }

                // Sugerir destino del PKG en la carpeta centralizada de salida con nombre limpio
                string defaultDir = GetDefaultOutputDir();
                string safeName = FormatHelper.SanitizeFileName(chosenTitle);
                txtOutputPath.Text = Path.Combine(defaultDir, string.Format("{0}.pkg", safeName));

                // Detectar si es CD o DVD
                if (CdFixer.IsPs2Cd(isoPath))
                {
                    LogMessage("[INFO] Formato detectado: Disco CD-ROM (Auto-parche LIMG habilitado).");
                }
                else
                {
                    LogMessage("[INFO] Formato detectado: Disco DVD-ROM estándar.");
                }

                // Auto-descargar carátula oficial y fondo solo si es un juego real
                if (isCheatOrUtility)
                {
                    LogMessage(string.Format("[WARN] 🛑 Se omitió la descarga automática de la portada de \"{0}\" para evitar asignar una carátula errónea.", officialTitle));
                    LogMessage("[TIP] Puedes usar el botón 'Crear Arte (Studio)...' o '...' para asignar una imagen propia si lo deseas.");
                }
                else
                {
                    TryAutoDownloadCover(serial, txtGameTitle.Text);
                }

                loadSuccess = true;
            }
            catch (Exception ex)
            {
                txtGameTitle.Text = fileNameWithoutExt;
                LogMessage("[WARN] Error al analizar serial de ISO: " + ex.Message);
            }
            finally
            {
                groupBoxIso.Enabled = true;
                groupBoxMedia.Enabled = true;
                groupBoxSettings.Enabled = true;
                _isBusy = false;

                if (loadSuccess)
                {
                    btnBuild.Enabled = true;
                    btnBuild.Text = "COMPILAR PKG DE PS4";
                    btnBuild.BackColor = Color.FromArgb(46, 204, 113);

                    btnOpenStudio.Enabled = true;
                    btnOpenStudio.BackColor = Color.FromArgb(142, 68, 173);
                    btnAutoCover.Enabled = true;
                }
                else
                {
                    btnBuild.Enabled = false;
                    btnBuild.Text = "SELECCIONA UNA ISO DE PS2 VÁLIDA";
                    btnBuild.BackColor = Color.FromArgb(60, 64, 70);

                    btnOpenStudio.Enabled = false;
                    btnOpenStudio.BackColor = Color.FromArgb(60, 64, 70);
                    btnAutoCover.Enabled = false;
                }
            }
        }

        private async void TryAutoDownloadCover(string serial, string gameTitle)
        {
            try
            {
                string cacheDir = Path.Combine(_appDir, "cache");
                LogMessage(string.Format("[INFO] Buscando arte para {0}...", serial));
                CoverResult result = await CoverDownloader.GetGameMediaAsync(serial, gameTitle, _localArtPath, cacheDir);
                if (result != null && !string.IsNullOrEmpty(result.SquareIconPath) && File.Exists(result.SquareIconPath))
                {
                    _currentCoverIcon = result.SquareIconPath;
                    _currentDiscIcon = result.DiscIconPath;

                    txtIconPath.Text = result.SquareIconPath;
                    pbIcon.ImageLocation = result.SquareIconPath;

                    string sourceLabel = result.IsFromLocalDataset ? "Dataset Local (Offline)" : "Servidor Online";
                    LogMessage(string.Format("[INFO] ¡Caratula frontal 512x512 cargada desde {0}!", sourceLabel));

                    if (!string.IsNullOrEmpty(result.DiscIconPath))
                    {
                        LogMessage("[TIP] Puedes hacer clic en la miniatura del icono para alternar entre la Portada y el Disco.");
                    }

                    // Asignar el fondo oficial si está disponible
                    if (!string.IsNullOrEmpty(result.BackgroundPath) && File.Exists(result.BackgroundPath))
                    {
                        txtPicPath.Text = result.BackgroundPath;
                        pbPic.ImageLocation = result.BackgroundPath;
                        LogMessage(string.Format("[INFO] ¡Fondo 1080p cargado desde {0}!", sourceLabel));
                    }
                }
                else
                {
                    txtIconPath.Text = "";
                    pbIcon.ImageLocation = null;
                    pbIcon.Image = null;
                    txtPicPath.Text = "";
                    pbPic.ImageLocation = null;
                    pbPic.Image = null;
                    LogMessage("[INFO] No se encontro arte automatico (normal en juegos Homebrew, Hacks o no oficiales).");
                    LogMessage("[TIP] Usa 'Crear Arte (Studio)' para pegar la caratula y fondo desde Google con Ctrl+V.");
                }
            }
            catch (Exception ex)
            {
                LogMessage("[WARN] Error al obtener multimedia: " + ex.Message);
            }
        }

        private void pbIcon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentDiscIcon) || string.IsNullOrEmpty(_currentCoverIcon)) return;

            if (txtIconPath.Text == _currentCoverIcon)
            {
                txtIconPath.Text = _currentDiscIcon;
                pbIcon.ImageLocation = _currentDiscIcon;
                LogMessage("[INFO] Cambiado icono a: Disco original (DVD).");
            }
            else
            {
                txtIconPath.Text = _currentCoverIcon;
                pbIcon.ImageLocation = _currentCoverIcon;
                LogMessage("[INFO] Cambiado icono a: Caratula frontal oficial con ambientacion.");
            }
        }

        private void btnAutoCover_Click(object sender, EventArgs e)
        {
            string serial = txtSerial.Text.Trim();
            if (string.IsNullOrEmpty(serial))
            {
                MessageBox.Show("Carga primero una imagen ISO de PS2.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txtIconPath.Text = "";
            txtPicPath.Text = "";
            pbIcon.Image = null;
            pbIcon.ImageLocation = null;
            pbPic.Image = null;
            pbPic.ImageLocation = null;
            _currentCoverIcon = null;
            _currentDiscIcon = null;
            TryAutoDownloadCover(serial, txtGameTitle.Text.Trim());
        }

        private void btnOpenStudio_Click(object sender, EventArgs e)
        {
            try
            {
                string serial = txtSerial.Text.Trim();
                if (string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(txtIsoPath.Text))
                {
                    MessageBox.Show("Por favor carga primero una imagen ISO de PS2 para identificar el juego.", "Carga un Juego Primero", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string targetFolder = Path.Combine(_appDir, "cache");
                if (!Directory.Exists(targetFolder))
                {
                    try { Directory.CreateDirectory(targetFolder); } catch { }
                }

                LogMessage("[INFO] Abriendo PS2 Artwork Studio para " + serial + "...");

                using (ArtStudioForm studio = new ArtStudioForm(targetFolder, serial))
                {
                    studio.ShowDialog(this);

                    if (studio.SavedArtwork)
                    {
                        if (File.Exists(studio.SavedIconPath))
                        {
                            txtIconPath.Text = studio.SavedIconPath;
                            pbIcon.ImageLocation = studio.SavedIconPath;
                            _currentCoverIcon = studio.SavedIconPath;
                        }

                        if (File.Exists(studio.SavedBgPath))
                        {
                            txtPicPath.Text = studio.SavedBgPath;
                            pbPic.ImageLocation = studio.SavedBgPath;
                        }

                        LogMessage("[SUCCESS] 🎨 ¡Arte personalizado desde Artwork Studio cargado al proyecto!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir PS2 Art Studio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar Icono del Juego";
                ofd.Filter = "Imagenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Todas las imagenes|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtIconPath.Text = ofd.FileName;
                    pbIcon.ImageLocation = ofd.FileName;
                }
            }
        }

        private void btnBrowsePic_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar Fondo del Juego";
                ofd.Filter = "Imagenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Todas las imagenes|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPicPath.Text = ofd.FileName;
                    pbPic.ImageLocation = ofd.FileName;
                }
            }
        }

        private void LoadImage(string imgPath)
        {
            try
            {
                using (Image bmp = Image.FromFile(imgPath))
                {
                    if (bmp.Width == bmp.Height)
                    {
                        txtIconPath.Text = imgPath;
                        pbIcon.ImageLocation = imgPath;
                        LogMessage("[INFO] Asignado como Icono (512x512).");
                    }
                    else
                    {
                        txtPicPath.Text = imgPath;
                        pbPic.ImageLocation = imgPath;
                        LogMessage("[INFO] Asignado como Fondo (1920x1080).");
                    }
                }
            }
            catch { }
        }

        private string GetDefaultOutputDir()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dir = Path.Combine(desktop, "PKGs_PS4");
            if (!Directory.Exists(dir))
            {
                try { Directory.CreateDirectory(dir); } catch { }
            }
            return dir;
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar PKG de PS4";
                sfd.Filter = "Paquete de PS4 (*.pkg)|*.pkg";
                sfd.InitialDirectory = GetDefaultOutputDir();
                sfd.FileName = string.Format("{0}.pkg", txtGameTitle.Text);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = sfd.FileName;
                }
            }
        }

        private void btnBrowseOrbis_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Ubicar orbis-pub-cmd.exe";
                ofd.Filter = "Ejecutable (*.exe)|*.exe";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtOrbisPath.Text = ofd.FileName;
                }
            }
        }

        private void btnSendToPs4_Click(object sender, EventArgs e)
        {
            string pkg = txtOutputPath.Text.Trim();
            using (SendToPs4Form sf = new SendToPs4Form(pkg))
            {
                sf.ShowDialog(this);
            }
        }

        private void btnTogglePaths_Click(object sender, EventArgs e)
        {
            pnlPaths.Visible = !pnlPaths.Visible;
            btnTogglePaths.Text = pnlPaths.Visible ? "▲ Ocultar" : "⚙️ Rutas";
            int shift = pnlPaths.Visible ? (pnlPaths.Height + 6) : -(pnlPaths.Height + 6);
            btnBuild.Top += shift;
            progressBar.Top += shift;
            lblProgress.Top += shift;
            lblStatus.Top += shift;
            groupBoxLog.Top += shift;
            groupBoxLog.Height -= shift;
        }

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLog.Text))
            {
                Clipboard.SetText(txtLog.Text);
                MessageBox.Show("Registro copiado al portapapeles.", "Copiado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private async void btnBuild_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                LogMessage("[WARN] ⚠️ Ya hay una tarea en ejecución.");
                return;
            }

            if (string.IsNullOrEmpty(txtIsoPath.Text) || !File.Exists(txtIsoPath.Text))
            {
                MessageBox.Show("Por favor selecciona una imagen ISO valida de PS2.", "Falta archivo ISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtOutputPath.Text))
            {
                MessageBox.Show("Por favor define la ruta de salida para el archivo PKG.", "Ruta no valida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string titleId = txtTitleId.Text.Trim().ToUpper();
            if (!titleId.StartsWith("CUSA") || titleId.Length != 9)
            {
                MessageBox.Show("El Title ID debe tener formato CUSAxxxxx (ej: CUSA12345).", "Title ID Invalido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar título del juego visible en la interfaz de PS4
            string currentGameTitle = txtGameTitle.Text.Trim();
            if (string.IsNullOrEmpty(currentGameTitle))
            {
                MessageBox.Show("Por favor escribe el título del juego para la PS4.", "Título Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGameTitle.Focus();
                return;
            }

            if (Regex.IsMatch(currentGameTitle, @"^[A-Za-z]{4}[-_]?\d{3,5}(\s*\(.*\))?$", RegexOptions.IgnoreCase))
            {
                DialogResult drSerial = MessageBox.Show(
                    string.Format("El nombre actual del juego es \"{0}\", el cual parece ser solo el código de serie del disco.\n\nEste es el nombre exacto que se mostrará en el menú principal de tu PlayStation 4.\n\n¿Deseas compilar con este código o prefieres escribir el nombre real del juego (ej: Pro Evolution Soccer 2013)?\n\nPresiona 'No' para cancelar y escribir el nombre real.", currentGameTitle),
                    "Confirmar Nombre del Juego",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (drSerial != DialogResult.Yes)
                {
                    txtGameTitle.Focus();
                    txtGameTitle.SelectAll();
                    return;
                }
            }

            // 1. Validar si la ISO está truncada / corrupta
            if (IsoReader.IsTruncated(txtIsoPath.Text))
            {
                MessageBox.Show("La imagen ISO parece estar incompleta o dañada (su tamaño en disco es menor a lo que declara el descriptor de volumen).\n\nVerifica que la descarga haya finalizado correctamente.", "Descarga Incompleta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Validar espacio en disco (mínimo 6 GB recomendados)
            string outDir = Path.GetDirectoryName(txtOutputPath.Text);
            long freeBytes;
            if (!FormatHelper.CheckDiskSpace(outDir, 6L * 1024L * 1024L * 1024L, out freeBytes))
            {
                double freeGb = freeBytes / (1024.0 * 1024.0 * 1024.0);
                DialogResult dr = MessageBox.Show(
                    string.Format("Advertencia de espacio en disco:\n\nLa unidad de destino tiene solo {0:0.0} GB libres.\nSe recomiendan al menos 6.0 GB para evitar errores al compilar el PKG.\n\n¿Deseas continuar de todos modos?", freeGb),
                    "Espacio Bajo en Disco",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes) return;
            }

            SetUiBusyState(true, "⚙️ COMPILANDO PKG...", Color.FromArgb(241, 196, 15), "Compilando PKG de PS4...");
            progressBar.Value = 0;

            string contentId = string.Format("UP9000-{0}_00-PS2TOPS400000001", titleId);

            // 3. Saneamiento de nombre de salida
            string outDirName = Path.GetDirectoryName(txtOutputPath.Text);
            string outFileName = FormatHelper.SanitizeFileName(Path.GetFileNameWithoutExtension(txtOutputPath.Text)) + ".pkg";
            string safePkgPath = Path.Combine(outDirName, outFileName);

            BuildOptions options = new BuildOptions();
            options.IsoPath = txtIsoPath.Text;
            options.GameTitle = txtGameTitle.Text.Trim();
            options.TitleId = titleId;
            options.Ps2Serial = txtSerial.Text.Trim();
            options.ContentId = contentId;
            options.IconPath = txtIconPath.Text;
            options.BackgroundPath = txtPicPath.Text;
            options.Widescreen = chkWidescreen.Checked;
            options.FastLoad = chkFastLoad.Checked;
            options.TemplatePath = GetSelectedTemplatePath();
            options.OrbisToolPath = txtOrbisPath.Text;
            options.OutputPkgPath = safePkgPath;

            string curSerial = txtSerial.Text.Trim();
            if (ConfigDatabase.HasConfig(curSerial))
            {
                options.CustomConfig = ConfigDatabase.GetConfigText(curSerial);
                options.LuaScriptPath = ConfigDatabase.GetLuaPath(curSerial);
                LogMessage(string.Format("[INFO] Aplicando parche comunitario optimizado para {0}.", curSerial));
            }

            try
            {
                bool ok = await _builder.BuildAsync(options);
                lblStatus.Text = ok ? "¡Finalizado con exito!" : "Error en el proceso.";

                if (ok)
                {
                    DialogResult dr = MessageBox.Show(string.Format("¡El archivo PKG fue creado exitosamente!\n\nUbicación:\n{0}\n\n¿Deseas abrir la carpeta con el archivo generado?", txtOutputPath.Text), "Proceso Exitoso", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", txtOutputPath.Text));
                        }
                        catch { }
                    }
                }
                else
                {
                    MessageBox.Show("Hubo un error al generar el PKG. Revisa el registro en la consola para mas detalles.", "Fallo en compilacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                SetUiBusyState(false, "COMPILAR PKG DE PS4", Color.FromArgb(46, 204, 113));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentExtractedTempDir) && Directory.Exists(_currentExtractedTempDir))
            {
                try { Directory.Delete(_currentExtractedTempDir, true); } catch { }
            }
            base.OnFormClosing(e);
        }

        private void LogMessage(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), msg);
                return;
            }

            // Separadores directos sin timestamp
            if (msg.StartsWith("──") || msg.StartsWith("=="))
            {
                txtLog.SelectionStart = txtLog.TextLength;
                txtLog.SelectionLength = 0;
                txtLog.SelectionColor = Color.FromArgb(70, 75, 85);
                txtLog.AppendText(msg + Environment.NewLine);
                txtLog.ScrollToCaret();
                return;
            }

            string time = string.Format("[{0:HH:mm:ss}] ", DateTime.Now);

            // Determinar color profesional según el tipo de registro
            Color msgColor = Color.FromArgb(220, 221, 225); // Gris claro / blanco suave por defecto
            if (msg.StartsWith("[ERROR]") || msg.Contains("[ERROR]"))
            {
                msgColor = Color.FromArgb(255, 99, 99); // Rojo vivo de error
            }
            else if (msg.StartsWith("[WARN]") || msg.Contains("[WARN]"))
            {
                msgColor = Color.FromArgb(243, 156, 18); // Naranja / Ámbar de advertencia
            }
            else if (msg.StartsWith("[SUCCESS]") || msg.Contains("[SUCCESS]"))
            {
                msgColor = Color.FromArgb(46, 204, 113); // Verde esmeralda de éxito
            }
            else if (msg.StartsWith("[TIP]") || msg.Contains("[TIP]"))
            {
                msgColor = Color.FromArgb(175, 122, 197); // Violeta suave para consejos
            }
            else if (msg.Contains("🔥") || msg.Contains("Parche Comunitario"))
            {
                msgColor = Color.FromArgb(84, 160, 255); // Azul cielo brillante
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;

            // Timestamp en gris tenue para que no compita visualmente con el texto
            txtLog.SelectionColor = Color.FromArgb(110, 115, 125);
            txtLog.AppendText(time);

            // Mensaje en su color correspondiente
            txtLog.SelectionColor = msgColor;
            txtLog.AppendText(msg + Environment.NewLine);

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void UpdateProgress(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgress), value);
                return;
            }

            int val = Math.Max(0, Math.Min(value, 100));
            progressBar.Value = val;
            lblProgress.Text = string.Format("{0}%", val);
        }
    }
}
