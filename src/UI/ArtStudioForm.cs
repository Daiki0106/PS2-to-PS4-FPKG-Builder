using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PS2toPS4Builder.Core;

namespace PS2toPS4Builder.UI
{
    public partial class ArtStudioForm : Form
    {
        private Image _sourceImage;
        private Bitmap _iconBitmap;
        private Bitmap _bgBitmap;

        public bool SavedArtwork { get; private set; }
        public string SavedIconPath { get; private set; }
        public string SavedBgPath { get; private set; }

        public ArtStudioForm(string initialFolder = null, string initialSerial = null)
        {
            InitializeComponent();
            SetupCustomEvents();

            if (!string.IsNullOrEmpty(initialFolder) && Directory.Exists(initialFolder))
            {
                txtFolderPath.Text = initialFolder;
            }
            else
            {
                LoadDefaultDirectory();
            }

            if (!string.IsNullOrEmpty(initialSerial))
            {
                txtGameName.Text = initialSerial;
            }

            SavedArtwork = false;
            SavedIconPath = "";
            SavedBgPath = "";

            string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            if (File.Exists(icoPath))
            {
                try { this.Icon = new Icon(icoPath); } catch { }
            }
            if (this.Icon == null)
            {
                try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            }
        }

        private void SetupCustomEvents()
        {
            this.KeyPreview = true;
            this.KeyDown += ArtStudioForm_KeyDown;
            this.AllowDrop = true;
            this.DragEnter += ArtStudioForm_DragEnter;
            this.DragDrop += ArtStudioForm_DragDrop;

            // Deseleccionar campo al presionar Enter o Escape
            txtGameName.KeyDown += TextBox_EnterOrEscape_KeyDown;
            txtFolderPath.KeyDown += TextBox_EnterOrEscape_KeyDown;

            // Deseleccionar al hacer clic afuera en cualquier parte de la ventana
            AttachUnfocusClick(this);

            cmbIconStyle.SelectedIndexChanged += cmbIconStyle_SelectedIndexChanged;
        }

        private IconCropStyle GetCurrentIconStyle()
        {
            if (cmbIconStyle.SelectedIndex == 1) return IconCropStyle.FitBlack;
            if (cmbIconStyle.SelectedIndex == 2) return IconCropStyle.FitAmbient;
            return IconCropStyle.FillSquare;
        }

        private void cmbIconStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_sourceImage != null)
            {
                if (_iconBitmap != null) _iconBitmap.Dispose();
                _iconBitmap = StudioImageProcessor.CreateSquareIcon(_sourceImage, 512, GetCurrentIconStyle());
                picIcon.Image = _iconBitmap;
            }
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

        private void LoadDefaultDirectory()
        {
            string cfgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last_path.txt");
            string target = null;

            if (File.Exists(cfgFile))
            {
                try
                {
                    string saved = File.ReadAllText(cfgFile).Trim();
                    if (Directory.Exists(saved)) target = saved;
                }
                catch { }
            }

            if (string.IsNullOrEmpty(target) || !Directory.Exists(target))
            {
                string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
                target = Directory.Exists(cacheDir) ? cacheDir : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            txtFolderPath.Text = target;
        }

        private void ArtStudioForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Si el portapapeles contiene una imagen, SIEMPRE pegar la imagen
                if (Clipboard.ContainsImage())
                {
                    PasteFromClipboard();
                    this.ActiveControl = null;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }

                if (this.ActiveControl is TextBox)
                {
                    return;
                }

                PasteFromClipboard();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ArtStudioForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void ArtStudioForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    string first = files[0];
                    if (Directory.Exists(first))
                    {
                        SetTargetFolder(first);
                    }
                    else if (File.Exists(first))
                    {
                        string ext = Path.GetExtension(first).ToLower();
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".webp")
                        {
                            LoadImageFromFile(first);
                        }
                        else
                        {
                            string serial = IsoReader.DetectSerial(first);
                            string name = Path.GetFileNameWithoutExtension(first);

                            if (string.IsNullOrEmpty(serial))
                            {
                                serial = StudioImageProcessor.ExtractSerialFromText(name);
                            }

                            string effectiveId = !string.IsNullOrEmpty(serial) ? serial : name;
                            txtGameName.Text = effectiveId;
                            string dir = Path.GetDirectoryName(first);
                            SetTargetFolder(dir);

                            lblStatus.Text = "Juego detectado: " + effectiveId;
                            lblStatus.ForeColor = Color.LightGreen;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al arrastrar: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void SetTargetFolder(string folderPath)
        {
            txtFolderPath.Text = folderPath;
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last_path.txt"), folderPath);
            }
            catch { }

            DirectoryInfo di = new DirectoryInfo(folderPath);
            string folderName = di.Name;

            string serial = StudioImageProcessor.ExtractSerialFromText(folderName);
            if (!string.IsNullOrEmpty(serial))
            {
                txtGameName.Text = serial;
            }
            else if (!folderName.Equals("PS2", StringComparison.OrdinalIgnoreCase))
            {
                txtGameName.Text = folderName;
            }

            lblStatus.Text = "Carpeta asignada: " + folderName;
            lblStatus.ForeColor = Color.LightSkyBlue;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        private void PasteFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    Image img = Clipboard.GetImage();
                    if (img != null)
                    {
                        SetSourceImage(img);
                        lblStatus.Text = string.Format("Imagen pegada desde el portapapeles ({0}x{1})", img.Width, img.Height);
                        lblStatus.ForeColor = Color.LightGreen;
                        return;
                    }
                }

                if (Clipboard.ContainsFileDropList())
                {
                    var fileList = Clipboard.GetFileDropList();
                    if (fileList.Count > 0)
                    {
                        string firstFile = fileList[0];
                        if (File.Exists(firstFile))
                        {
                            LoadImageFromFile(firstFile);
                            return;
                        }
                    }
                }

                lblStatus.Text = "No hay ninguna imagen en el portapapeles. Copia una con clic derecho.";
                lblStatus.ForeColor = Color.FromArgb(241, 196, 15);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al pegar portapapeles: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar Imagen de Portada o Fondo";
                ofd.Filter = "Imagenes Compatibles (*.png;*.jpg;*.jpeg;*.bmp;*.webp)|*.png;*.jpg;*.jpeg;*.bmp;*.webp|Todos los Archivos|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadImageFromFile(ofd.FileName);
                }
            }
        }

        private void LoadImageFromFile(string filePath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image img = Image.FromStream(ms);
                    SetSourceImage((Image)img.Clone());
                }
                lblStatus.Text = string.Format("Cargada: {0} ({1}x{2})", Path.GetFileName(filePath), _sourceImage.Width, _sourceImage.Height);
                lblStatus.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al cargar imagen: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void SetSourceImage(Image img)
        {
            if (_sourceImage != null) _sourceImage.Dispose();
            if (_iconBitmap != null) _iconBitmap.Dispose();
            if (_bgBitmap != null) _bgBitmap.Dispose();

            _sourceImage = img;

            // Procesar icono 512x512 según el estilo elegido por el usuario
            _iconBitmap = StudioImageProcessor.CreateSquareIcon(_sourceImage, 512, GetCurrentIconStyle());
            picIcon.Image = _iconBitmap;

            // Procesar fondo panorámico 1920x1080
            _bgBitmap = StudioImageProcessor.CreateWidescreenBackground(_sourceImage, 1920, 1080);
            picBg.Image = _bgBitmap;

            btnSaveIcon.Enabled = true;
            btnSaveBg.Enabled = true;
            btnSaveBoth.Enabled = true;
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Selecciona la carpeta donde guardar icon0.png y pic1.png";
                if (Directory.Exists(txtFolderPath.Text)) fbd.SelectedPath = txtFolderPath.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SetTargetFolder(fbd.SelectedPath);
                }
            }
        }

        private string EnsureTargetFolder()
        {
            string folder = txtFolderPath.Text.Trim();
            if (string.IsNullOrEmpty(folder))
            {
                folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            }

            if (!Directory.Exists(folder))
            {
                try { Directory.CreateDirectory(folder); } catch { }
            }

            return folder;
        }

        private void btnSaveIcon_Click(object sender, EventArgs e)
        {
            if (_iconBitmap == null)
            {
                MessageBox.Show("Primero pega o carga una imagen.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string folder = EnsureTargetFolder();
            string serial = !string.IsNullOrEmpty(txtGameName.Text.Trim()) ? txtGameName.Text.Trim() : "custom";
            string fileName = string.Format("{0}_studio_icon0.png", FormatHelper.SanitizeFileName(serial));
            string targetPath = Path.Combine(folder, fileName);

            try
            {
                StudioImageProcessor.SavePng24(_iconBitmap, targetPath);
                SavedIconPath = targetPath;
                SavedArtwork = true;
                lblStatus.Text = "¡Guardado exitoso: " + fileName + " (512x512)!";
                lblStatus.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al guardar icono: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void btnSaveBg_Click(object sender, EventArgs e)
        {
            if (_bgBitmap == null)
            {
                MessageBox.Show("Primero pega o carga una imagen.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string folder = EnsureTargetFolder();
            string serial = !string.IsNullOrEmpty(txtGameName.Text.Trim()) ? txtGameName.Text.Trim() : "custom";
            string fileName = string.Format("{0}_studio_pic1.png", FormatHelper.SanitizeFileName(serial));
            string targetPath = Path.Combine(folder, fileName);

            try
            {
                StudioImageProcessor.SavePng24(_bgBitmap, targetPath);
                SavedBgPath = targetPath;
                SavedArtwork = true;
                lblStatus.Text = "¡Guardado exitoso: " + fileName + " (1920x1080)!";
                lblStatus.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al guardar fondo: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void btnSaveBoth_Click(object sender, EventArgs e)
        {
            if (_iconBitmap == null || _bgBitmap == null)
            {
                MessageBox.Show("Primero pega o carga una imagen.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string folder = EnsureTargetFolder();
            string serial = !string.IsNullOrEmpty(txtGameName.Text.Trim()) ? txtGameName.Text.Trim() : "custom";
            string cleanSerial = FormatHelper.SanitizeFileName(serial);
            string iconName = string.Format("{0}_studio_icon0.png", cleanSerial);
            string bgName = string.Format("{0}_studio_pic1.png", cleanSerial);

            try
            {
                string iconPath = Path.Combine(folder, iconName);
                string bgPath = Path.Combine(folder, bgName);

                StudioImageProcessor.SavePng24(_iconBitmap, iconPath);
                StudioImageProcessor.SavePng24(_bgBitmap, bgPath);

                SavedIconPath = iconPath;
                SavedBgPath = bgPath;
                SavedArtwork = true;

                lblStatus.Text = "¡Arte guardado exitosamente!";
                lblStatus.ForeColor = Color.LightGreen;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al guardar archivos: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string folder = txtFolderPath.Text.Trim();
            if (Directory.Exists(folder))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", folder);
                }
                catch { }
            }
            else
            {
                MessageBox.Show("La carpeta especificada no existe.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
