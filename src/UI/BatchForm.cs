using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS2toPS4Builder.Core;

namespace PS2toPS4Builder.UI
{
    public partial class BatchForm : Form
    {
        private class BatchItem
        {
            public string FilePath;
            public string Title;
            public string Serial;
            public string TitleId;
            public string FileSize;
            public string OutputPkg;
            public string CleanUpDir;
            public bool IsValid;
            public bool IsCheatOrUtility;
            public ListViewItem ListItem;
        }

        private List<BatchItem> _queue = new List<BatchItem>();
        private bool _isProcessing = false;
        private bool _cancelRequested = false;
        private string _appDir;
        private string _localArtPath;
        private string _orbisPath;

        public BatchForm(string appDir, string localArtPath, string orbisPath)
        {
            _appDir = appDir;
            _localArtPath = localArtPath;
            _orbisPath = orbisPath;

            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.AllowDrop = true;
            this.DragEnter += BatchForm_DragEnter;
            this.DragDrop += BatchForm_DragDrop;

            // Carpeta de salida por defecto (Escritorio o Descargas)
            txtOutputDir.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PKGs_PS4");

            // Cargar emuladores
            LoadEmulators();

            string icoPath = Path.Combine(_appDir, "app.ico");
            if (File.Exists(icoPath))
            {
                try { this.Icon = new Icon(icoPath); } catch { }
            }
            if (this.Icon == null)
            {
                try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            }
        }

        private void LoadEmulators()
        {
            cmbEmulator.Items.Clear();
            string emusDir = Path.Combine(_appDir, "emus");
            if (Directory.Exists(emusDir))
            {
                foreach (string dir in Directory.GetDirectories(emusDir))
                {
                    cmbEmulator.Items.Add(Path.GetFileName(dir));
                }
            }

            if (!cmbEmulator.Items.Contains("Plantilla por Defecto (Jak v2)"))
            {
                cmbEmulator.Items.Insert(0, "Plantilla por Defecto (Jak v2)");
            }

            cmbEmulator.SelectedIndex = 0;
        }

        private string GetSelectedTemplatePath()
        {
            if (cmbEmulator.SelectedItem == null)
                return Path.Combine(_appDir, "template");

            string selected = cmbEmulator.SelectedItem.ToString();
            if (selected.StartsWith("Plantilla por Defecto"))
                return Path.Combine(_appDir, "template");

            string emuPath = Path.Combine(_appDir, "emus", selected);
            return Directory.Exists(emuPath) ? emuPath : Path.Combine(_appDir, "template");
        }

        private void BatchForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void BatchForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dropped = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (dropped != null)
                {
                    foreach (string item in dropped)
                    {
                        if (Directory.Exists(item))
                        {
                            AddFolder(item);
                        }
                        else if (File.Exists(item))
                        {
                            string ext = Path.GetExtension(item).ToLower();
                            if (FormatHelper.IsSupportedExtension(ext))
                            {
                                AddIso(item);
                            }
                        }
                    }
                }
            }
        }

        public void AddFolder(string folderPath)
        {
            try
            {
                string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
                foreach (string f in files)
                {
                    string ext = Path.GetExtension(f).ToLower();
                    if (FormatHelper.IsSupportedExtension(ext))
                    {
                        AddIso(f);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al escanear carpeta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddIso(string filePath)
        {
            // Evitar duplicados
            foreach (var existing in _queue)
            {
                if (existing.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                FileInfo fi = new FileInfo(filePath);

                BatchItem item = new BatchItem();
                item.FilePath = filePath;
                item.OutputPkg = "";
                item.CleanUpDir = null;

                string displayTitle = fileName;
                string displaySerial = "";
                string displayTitleId = "CUSA Auto";
                long sizeInBytes = fi.Length;
                bool isCompressed = FormatHelper.IsCompressedArchive(ext);
                bool isChd = FormatHelper.IsChd(ext);
                bool hasSystemCnf = true;

                if (isCompressed)
                {
                    string toolsDir = Path.Combine(_appDir, "tools");
                    var info = FormatHelper.InspectArchive(filePath, toolsDir);

                    if (info.UncompressedSize > 0)
                    {
                        sizeInBytes = info.UncompressedSize;
                    }
                    hasSystemCnf = info.HasSystemCnf;

                    string s = info.DetectedSerial;
                    if (string.IsNullOrEmpty(s)) s = IsoReader.DetectSerialFromName(fileName);
                    if (!string.IsNullOrEmpty(s))
                    {
                        displaySerial = s;
                        displayTitleId = IsoReader.GenerateCusaId(s);
                        string t = TitleDatabase.GetTitle(s);
                        if (!string.IsNullOrEmpty(t)) displayTitle = t;
                    }
                    else if (!string.IsNullOrEmpty(info.InternalName))
                    {
                        displayTitle = Path.GetFileNameWithoutExtension(info.InternalName);
                    }
                }
                else if (isChd)
                {
                    string s = IsoReader.DetectSerialFromName(fileName);
                    if (!string.IsNullOrEmpty(s))
                    {
                        displaySerial = s;
                        displayTitleId = IsoReader.GenerateCusaId(s);
                        string t = TitleDatabase.GetTitle(s);
                        if (!string.IsNullOrEmpty(t)) displayTitle = t;
                    }
                }
                else
                {
                    // Archivo directo: .iso, .bin, .img, .mdf
                    hasSystemCnf = IsoReader.HasSystemCnf(filePath);

                    string s = IsoReader.DetectSerial(filePath);
                    if (string.IsNullOrEmpty(s)) s = IsoReader.DetectSerialFromName(fileName);

                    if (!string.IsNullOrEmpty(s))
                    {
                        displaySerial = s;
                        displayTitleId = IsoReader.GenerateCusaId(s);
                        string t = TitleDatabase.GetTitle(s);
                        if (!string.IsNullOrEmpty(t)) displayTitle = t;
                    }
                }

                bool isPs1 = !isCompressed && !isChd && IsoReader.IsPs1Game(filePath);
                bool isTruncated = !isCompressed && !isChd && IsoReader.IsTruncated(filePath);

                string dbTitle = !string.IsNullOrEmpty(displaySerial) ? TitleDatabase.GetTitle(displaySerial) : null;
                bool isCheatOrUtility = FormatHelper.IsCheatOrUtility(fileName, filePath, displaySerial, dbTitle);
                if (isCheatOrUtility)
                {
                    string cleanTitle = System.Text.RegularExpressions.Regex.Replace(fileName, @"\s*\([^)]*\)", "").Trim();
                    displayTitle = string.IsNullOrEmpty(cleanTitle) ? fileName : cleanTitle;
                }
                item.IsCheatOrUtility = isCheatOrUtility;

                // APLICAR REGLAS EXACTAS DE VALIDACIÓN DE DISCOS PS2
                var val = FormatHelper.ValidateDisc(sizeInBytes, hasSystemCnf, isCompressed || isChd, isPs1, isTruncated, isCheatOrUtility);
                item.IsValid = val.IsValid;

                double displayGb = sizeInBytes / (1024.0 * 1024.0 * 1024.0);
                string displaySize = string.Format("{0:0.00} GB", displayGb);

                item.Title = displayTitle;
                item.Serial = displaySerial;
                item.TitleId = displayTitleId;
                item.FileSize = displaySize;

                ListViewItem lvi = new ListViewItem((_queue.Count + 1).ToString());
                lvi.SubItems.Add(displayTitle);
                lvi.SubItems.Add(string.IsNullOrEmpty(displaySerial) ? "..." : displaySerial);
                lvi.SubItems.Add(displayTitleId);
                lvi.SubItems.Add(displaySize);
                lvi.SubItems.Add(val.StatusText);
                lvi.ForeColor = Color.White;
                lvi.SubItems[5].ForeColor = val.StatusColor;

                item.ListItem = lvi;
                _queue.Add(item);
                listViewGames.Items.Add(lvi);
                UpdateTotalLabel();
                EnsureUniqueTitleIds();
            }
            catch { }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar Imágenes de PS2 (ISO, BIN, IMG, MDF, GZ, 7Z, CHD)";
                ofd.Filter = FormatHelper.FileFilter;
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in ofd.FileNames)
                    {
                        AddIso(file);
                    }
                }
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Selecciona la carpeta donde tienes tus ISOs de PS2";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    AddFolder(fbd.SelectedPath);
                }
            }
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            RemoveSelectedGames();
        }

        private void listViewGames_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedGames();
            }
        }

        private void RemoveSelectedGames()
        {
            if (_isProcessing) return;
            if (listViewGames.SelectedItems.Count == 0) return;

            for (int i = listViewGames.SelectedItems.Count - 1; i >= 0; i--)
            {
                ListViewItem lvi = listViewGames.SelectedItems[i];
                BatchItem toRemove = _queue.Find(b => b.ListItem == lvi);
                if (toRemove != null)
                {
                    if (!string.IsNullOrEmpty(toRemove.CleanUpDir) && Directory.Exists(toRemove.CleanUpDir))
                    {
                        try { Directory.Delete(toRemove.CleanUpDir, true); } catch { }
                    }
                    _queue.Remove(toRemove);
                }
                listViewGames.Items.Remove(lvi);
            }

            for (int i = 0; i < listViewGames.Items.Count; i++)
            {
                listViewGames.Items[i].SubItems[0].Text = (i + 1).ToString();
            }

            UpdateTotalLabel();
            EnsureUniqueTitleIds();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;
            foreach (var item in _queue)
            {
                if (!string.IsNullOrEmpty(item.CleanUpDir) && Directory.Exists(item.CleanUpDir))
                {
                    try { Directory.Delete(item.CleanUpDir, true); } catch { }
                }
            }
            _queue.Clear();
            listViewGames.Items.Clear();
            UpdateTotalLabel();
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Selecciona la carpeta donde se guardarán todos los archivos .pkg";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtOutputDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void UpdateTotalLabel()
        {
            lblTotal.Text = string.Format("Total en cola: {0} juegos", _queue.Count);
        }

        private void EnsureUniqueTitleIds()
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in _queue)
            {
                if (!item.IsValid) continue;
                string baseId = item.TitleId;
                if (string.IsNullOrEmpty(baseId) || baseId == "CUSA Auto" || baseId == "CUSA (Auto)")
                {
                    baseId = IsoReader.GenerateCusaId(item.Serial);
                }

                string candidate = baseId;
                int suffix = 1;
                while (seen.Contains(candidate))
                {
                    int num;
                    string numPart = System.Text.RegularExpressions.Regex.Match(candidate, @"\d+").Value;
                    if (int.TryParse(numPart, out num))
                    {
                        candidate = string.Format("CUSA{0:D5}", (num + suffix) % 100000);
                    }
                    else
                    {
                        candidate = string.Format("CUSA{0}", new Random().Next(10000, 99999));
                    }
                    suffix++;
                }

                seen.Add(candidate);
                item.TitleId = candidate;
                item.ListItem.SubItems[3].Text = candidate;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_isProcessing)
            {
                _cancelRequested = true;
                btnStart.Enabled = false;
                lblStatus.Text = "Cancelando procesamiento...";
                return;
            }

            if (_queue.Count == 0)
            {
                MessageBox.Show("Agrega al menos una imagen ISO a la lista para comenzar.", "Lista Vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string outDir = txtOutputDir.Text.Trim();
            if (string.IsNullOrEmpty(outDir))
            {
                MessageBox.Show("Por favor define la carpeta de salida.", "Falta Carpeta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            // Comprobar espacio libre en disco (mínimo 6 GB recomendados)
            long freeBytes;
            if (!FormatHelper.CheckDiskSpace(outDir, 6L * 1024L * 1024L * 1024L, out freeBytes))
            {
                double freeGb = freeBytes / (1024.0 * 1024.0 * 1024.0);
                DialogResult dr = MessageBox.Show(
                    string.Format("Advertencia de espacio en disco:\n\nLa unidad de destino tiene solo {0:0.0} GB libres.\nSe recomiendan al menos 6.0 GB para evitar errores al compilar el lote.\n\n¿Deseas continuar de todos modos?", freeGb),
                    "Espacio Bajo en Disco",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes) return;
            }

            EnsureUniqueTitleIds();

            _isProcessing = true;
            _cancelRequested = false;
            btnStart.Text = "⏹ CANCELAR LOTE";
            btnStart.BackColor = Color.FromArgb(231, 76, 60);
            btnAddFiles.Enabled = false;
            btnAddFolder.Enabled = false;
            btnClear.Enabled = false;

            int completed = 0;
            int errors = 0;

            progressBarTotal.Maximum = _queue.Count;
            progressBarTotal.Value = 0;

            for (int i = 0; i < _queue.Count; i++)
            {
                if (_cancelRequested) break;

                BatchItem item = _queue[i];

                if (!item.IsValid)
                {
                    item.ListItem.SubItems[5].Text = "Omitido";
                    item.ListItem.SubItems[5].ForeColor = Color.FromArgb(243, 156, 18);
                    progressBarTotal.Value = i + 1;
                    continue;
                }

                item.ListItem.SubItems[5].Text = "Iniciando...";
                item.ListItem.SubItems[5].ForeColor = Color.Gold;
                item.ListItem.EnsureVisible();

                lblStatus.Text = string.Format("Procesando ({0}/{1}): {2}...", i + 1, _queue.Count, item.Title);

                bool success = await ProcessSingleGameAsync(item, outDir);

                if (success)
                {
                    completed++;
                    item.ListItem.SubItems[5].Text = "✓ Completado";
                    item.ListItem.SubItems[5].ForeColor = Color.LightGreen;
                }
                else
                {
                    errors++;
                    item.ListItem.SubItems[5].Text = "❌ Error";
                    item.ListItem.SubItems[5].ForeColor = Color.FromArgb(255, 100, 100);
                }

                progressBarTotal.Value = i + 1;
            }

            _isProcessing = false;
            btnStart.Text = "▶ INICIAR PROCESAMIENTO EN LOTE";
            btnStart.BackColor = Color.FromArgb(46, 204, 113);
            btnStart.Enabled = true;
            btnAddFiles.Enabled = true;
            btnAddFolder.Enabled = true;
            btnClear.Enabled = true;

            lblStatus.Text = string.Format("Lote terminado: {0} completados, {1} errores.", completed, errors);

            if (!_cancelRequested)
            {
                MessageBox.Show(string.Format("¡Procesamiento en lote completado!\n\nJuegos creados: {0}\nErrores: {1}\n\nUbicación:\n{2}", completed, errors, outDir), "Lote Finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task<bool> ProcessSingleGameAsync(BatchItem item, string outDir)
        {
            string actualGameFile = item.FilePath;
            string tempCleanUpDir = null;

            try
            {
                // Auto-descomprimir si es .gz, .7z, .zip o .chd
                string ext = Path.GetExtension(item.FilePath).ToLower();
                if (FormatHelper.IsCompressedArchive(ext) || FormatHelper.IsChd(ext))
                {
                    item.ListItem.SubItems[5].Text = "📦 Descomprimiendo...";
                    item.ListItem.SubItems[5].ForeColor = Color.FromArgb(52, 152, 219);
                    lblStatus.Text = string.Format("Descomprimiendo archivo: {0}...", item.Title);

                    string toolsDir = Path.Combine(_appDir, "tools");
                    actualGameFile = await FormatHelper.PrepareImageAsync(item.FilePath, toolsDir, null);
                    if (string.IsNullOrEmpty(actualGameFile) || !File.Exists(actualGameFile))
                    {
                        return false;
                    }
                    tempCleanUpDir = Path.GetDirectoryName(actualGameFile);

                    // VALIDAR SI LA IMAGEN EXTRAÍDA ES PS1 O CARECE DE SYSTEM.CNF DE PS2
                    if (IsoReader.IsPs1Game(actualGameFile))
                    {
                        item.ListItem.SubItems[5].Text = "❌ Es juego PS1";
                        item.ListItem.SubItems[5].ForeColor = Color.FromArgb(231, 76, 60);
                        return false;
                    }

                    if (!IsoReader.HasSystemCnf(actualGameFile))
                    {
                        item.ListItem.SubItems[5].Text = "❌ No es juego PS2";
                        item.ListItem.SubItems[5].ForeColor = Color.FromArgb(231, 76, 60);
                        return false;
                    }

                    // VALIDAR SI LA IMAGEN EXTRAÍDA ESTÁ TRUNCADA / DAÑADA
                    if (IsoReader.IsTruncated(actualGameFile))
                    {
                        item.ListItem.SubItems[5].Text = "❌ Descarga incompleta";
                        item.ListItem.SubItems[5].ForeColor = Color.FromArgb(231, 76, 60);
                        return false;
                    }
                }

                // Si el serial no se conocía antes de descomprimir, detectarlo ahora en la imagen real
                if (string.IsNullOrEmpty(item.Serial))
                {
                    string realSerial = IsoReader.DetectSerial(actualGameFile);
                    if (!string.IsNullOrEmpty(realSerial))
                    {
                        item.Serial = realSerial;
                        item.TitleId = IsoReader.GenerateCusaId(realSerial);
                        string realTitle = TitleDatabase.GetTitle(realSerial);
                        if (!string.IsNullOrEmpty(realTitle)) item.Title = realTitle;

                        item.ListItem.SubItems[1].Text = item.Title;
                        item.ListItem.SubItems[2].Text = item.Serial;
                        item.ListItem.SubItems[3].Text = item.TitleId;
                    }
                }

                // 1. Obtener arte (Local o Online) solo si no es disco de trucos / utilidad
                CoverResult art = null;
                if (!item.IsCheatOrUtility)
                {
                    string cacheDir = Path.Combine(_appDir, "cache");
                    art = await CoverDownloader.GetGameMediaAsync(item.Serial, item.Title, _localArtPath, cacheDir);
                }

                string iconPath = (art != null) ? art.SquareIconPath : "";
                string bgPath = (art != null) ? art.BackgroundPath : "";

                // 2. Ruta de salida PKG limpia y saneada
                string cleanTitle = FormatHelper.SanitizeFileName(item.Title);
                string pkgOutput = Path.Combine(outDir, string.Format("{0}.pkg", cleanTitle));

                // 3. Opciones de compilación
                string contentId = string.Format("UP9000-{0}_00-PS2TOPS400000001", item.TitleId);

                BuildOptions opts = new BuildOptions();
                opts.IsoPath = actualGameFile;
                opts.GameTitle = item.Title;
                opts.TitleId = item.TitleId;
                opts.Ps2Serial = item.Serial;
                opts.ContentId = contentId;
                opts.IconPath = iconPath;
                opts.BackgroundPath = bgPath;
                opts.Widescreen = chkWidescreen.Checked;
                opts.FastLoad = chkFastLoad.Checked;
                opts.TemplatePath = GetSelectedTemplatePath();
                opts.OrbisToolPath = _orbisPath;
                opts.OutputPkgPath = pkgOutput;

                if (ConfigDatabase.HasConfig(item.Serial))
                {
                    opts.CustomConfig = ConfigDatabase.GetConfigText(item.Serial);
                    opts.LuaScriptPath = ConfigDatabase.GetLuaPath(item.Serial);
                }

                item.ListItem.SubItems[5].Text = "⚙️ Creando PKG (0%)...";
                item.ListItem.SubItems[5].ForeColor = Color.Gold;

                PkgBuilder builder = new PkgBuilder();
                builder.OnProgress += (percent) =>
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            item.ListItem.SubItems[5].Text = string.Format("⚙️ Creando PKG ({0}%)...", percent);
                        }));
                    }
                    else
                    {
                        item.ListItem.SubItems[5].Text = string.Format("⚙️ Creando PKG ({0}%)...", percent);
                    }
                };
                return await builder.BuildAsync(opts);
            }
            catch
            {
                return false;
            }
            finally
            {
                // Limpiar archivo temporal si fue extraído
                if (!string.IsNullOrEmpty(tempCleanUpDir) && Directory.Exists(tempCleanUpDir))
                {
                    try { Directory.Delete(tempCleanUpDir, true); } catch { }
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_isProcessing)
            {
                foreach (var item in _queue)
                {
                    if (!string.IsNullOrEmpty(item.CleanUpDir) && Directory.Exists(item.CleanUpDir))
                    {
                        try { Directory.Delete(item.CleanUpDir, true); } catch { }
                    }
                }
            }
            base.OnFormClosing(e);
        }
    }
}
