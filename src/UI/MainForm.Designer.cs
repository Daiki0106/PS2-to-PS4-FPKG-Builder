namespace PS2toPS4Builder.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.groupBoxIso = new System.Windows.Forms.GroupBox();
            this.lblIso = new System.Windows.Forms.Label();
            this.txtIsoPath = new System.Windows.Forms.TextBox();
            this.btnBrowseIso = new System.Windows.Forms.Button();
            this.btnOpenBatch = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtGameTitle = new System.Windows.Forms.TextBox();
            this.lblSerial = new System.Windows.Forms.Label();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.lblTitleId = new System.Windows.Forms.Label();
            this.txtTitleId = new System.Windows.Forms.TextBox();

            this.groupBoxMedia = new System.Windows.Forms.GroupBox();
            this.lblIcon = new System.Windows.Forms.Label();
            this.txtIconPath = new System.Windows.Forms.TextBox();
            this.btnBrowseIcon = new System.Windows.Forms.Button();
            this.btnAutoCover = new System.Windows.Forms.Button();
            this.btnOpenStudio = new System.Windows.Forms.Button();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.lblPic = new System.Windows.Forms.Label();
            this.txtPicPath = new System.Windows.Forms.TextBox();
            this.btnBrowsePic = new System.Windows.Forms.Button();
            this.pbPic = new System.Windows.Forms.PictureBox();

            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.chkWidescreen = new System.Windows.Forms.CheckBox();
            this.chkFastLoad = new System.Windows.Forms.CheckBox();
            this.lblEmulator = new System.Windows.Forms.Label();
            this.cmbEmulator = new System.Windows.Forms.ComboBox();
            this.btnSendToPs4 = new System.Windows.Forms.Button();
            this.btnTogglePaths = new System.Windows.Forms.Button();
            this.lblPatchInfo = new System.Windows.Forms.Label();

            this.pnlPaths = new System.Windows.Forms.Panel();
            this.lblOrbis = new System.Windows.Forms.Label();
            this.txtOrbisPath = new System.Windows.Forms.TextBox();
            this.btnBrowseOrbis = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();

            this.btnBuild = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();

            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.btnCopyLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();

            this.groupBoxIso.SuspendLayout();
            this.groupBoxMedia.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPic)).BeginInit();
            this.groupBoxSettings.SuspendLayout();
            this.pnlPaths.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();

            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 620);
            this.Text = "PS2 to PS4 FPKG Builder - Beta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 30, 33);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            // 
            // groupBoxIso
            // 
            this.groupBoxIso.Location = new System.Drawing.Point(12, 8);
            this.groupBoxIso.Size = new System.Drawing.Size(776, 88);
            this.groupBoxIso.Text = "1. Archivo del Juego (PS2 ISO)";
            this.groupBoxIso.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.lblIso.Text = "ISO:";
            this.lblIso.Location = new System.Drawing.Point(12, 23);
            this.lblIso.Size = new System.Drawing.Size(35, 20);
            this.lblIso.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtIsoPath.Location = new System.Drawing.Point(50, 20);
            this.txtIsoPath.Size = new System.Drawing.Size(515, 23);
            this.txtIsoPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtIsoPath.ForeColor = System.Drawing.Color.White;

            this.btnBrowseIso.Text = "Examinar...";
            this.btnBrowseIso.Location = new System.Drawing.Point(572, 19);
            this.btnBrowseIso.Size = new System.Drawing.Size(95, 26);
            this.btnBrowseIso.BackColor = System.Drawing.Color.FromArgb(88, 101, 242);
            this.btnBrowseIso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseIso.ForeColor = System.Drawing.Color.White;
            this.btnBrowseIso.Click += new System.EventHandler(this.btnBrowseIso_Click);

            this.btnOpenBatch.Text = "📦 Lotes...";
            this.btnOpenBatch.Location = new System.Drawing.Point(673, 19);
            this.btnOpenBatch.Size = new System.Drawing.Size(95, 26);
            this.btnOpenBatch.BackColor = System.Drawing.Color.FromArgb(241, 196, 15);
            this.btnOpenBatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenBatch.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnOpenBatch.ForeColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.btnOpenBatch.Click += new System.EventHandler(this.btnOpenBatch_Click);

            this.lblTitle.Text = "Título:";
            this.lblTitle.Location = new System.Drawing.Point(12, 54);
            this.lblTitle.Size = new System.Drawing.Size(40, 20);
            this.lblTitle.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtGameTitle.Location = new System.Drawing.Point(50, 52);
            this.txtGameTitle.Size = new System.Drawing.Size(310, 23);
            this.txtGameTitle.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtGameTitle.ForeColor = System.Drawing.Color.White;

            this.lblSerial.Text = "PS2 Serial:";
            this.lblSerial.Location = new System.Drawing.Point(372, 54);
            this.lblSerial.Size = new System.Drawing.Size(65, 20);
            this.lblSerial.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtSerial.Location = new System.Drawing.Point(440, 52);
            this.txtSerial.Size = new System.Drawing.Size(115, 23);
            this.txtSerial.ReadOnly = true;
            this.txtSerial.BackColor = System.Drawing.Color.FromArgb(36, 38, 42);
            this.txtSerial.ForeColor = System.Drawing.Color.LightGreen;

            this.lblTitleId.Text = "PS4 Title ID:";
            this.lblTitleId.Location = new System.Drawing.Point(568, 54);
            this.lblTitleId.Size = new System.Drawing.Size(75, 20);
            this.lblTitleId.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtTitleId.Location = new System.Drawing.Point(648, 52);
            this.txtTitleId.Size = new System.Drawing.Size(120, 23);
            this.txtTitleId.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtTitleId.ForeColor = System.Drawing.Color.Gold;

            this.groupBoxIso.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblIso, this.txtIsoPath, this.btnBrowseIso, this.btnOpenBatch,
                this.lblTitle, this.txtGameTitle,
                this.lblSerial, this.txtSerial,
                this.lblTitleId, this.txtTitleId
            });

            // 
            // groupBoxMedia
            // 
            this.groupBoxMedia.Location = new System.Drawing.Point(12, 100);
            this.groupBoxMedia.Size = new System.Drawing.Size(776, 96);
            this.groupBoxMedia.Text = "2. Multimedia (Icono 512x512 y Fondo 1920x1080 - Auto-adaptable)";
            this.groupBoxMedia.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.lblIcon.Text = "Icono:";
            this.lblIcon.Location = new System.Drawing.Point(12, 25);
            this.lblIcon.Size = new System.Drawing.Size(42, 20);
            this.lblIcon.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtIconPath.Location = new System.Drawing.Point(55, 22);
            this.txtIconPath.Size = new System.Drawing.Size(175, 23);
            this.txtIconPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtIconPath.ForeColor = System.Drawing.Color.White;

            this.btnBrowseIcon.Text = "...";
            this.btnBrowseIcon.Location = new System.Drawing.Point(235, 21);
            this.btnBrowseIcon.Size = new System.Drawing.Size(32, 25);
            this.btnBrowseIcon.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowseIcon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseIcon.Click += new System.EventHandler(this.btnBrowseIcon_Click);

            this.btnAutoCover.Text = "🔄 Auto";
            this.btnAutoCover.Location = new System.Drawing.Point(271, 21);
            this.btnAutoCover.Size = new System.Drawing.Size(60, 25);
            this.btnAutoCover.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnAutoCover.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAutoCover.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular);
            this.btnAutoCover.ForeColor = System.Drawing.Color.White;
            this.btnAutoCover.Click += new System.EventHandler(this.btnAutoCover_Click);

            this.pbIcon.Location = new System.Drawing.Point(338, 18);
            this.pbIcon.Size = new System.Drawing.Size(68, 68);
            this.pbIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbIcon.BackColor = System.Drawing.Color.FromArgb(20, 21, 23);
            this.pbIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbIcon.Click += new System.EventHandler(this.pbIcon_Click);

            this.btnOpenStudio.Text = "🎨 Crear Arte (Studio)...";
            this.btnOpenStudio.Location = new System.Drawing.Point(55, 54);
            this.btnOpenStudio.Size = new System.Drawing.Size(276, 26);
            this.btnOpenStudio.BackColor = System.Drawing.Color.FromArgb(142, 68, 173);
            this.btnOpenStudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenStudio.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnOpenStudio.ForeColor = System.Drawing.Color.White;
            this.btnOpenStudio.Click += new System.EventHandler(this.btnOpenStudio_Click);

            this.lblPic.Text = "Fondo:";
            this.lblPic.Location = new System.Drawing.Point(420, 25);
            this.lblPic.Size = new System.Drawing.Size(45, 20);
            this.lblPic.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtPicPath.Location = new System.Drawing.Point(470, 22);
            this.txtPicPath.Size = new System.Drawing.Size(200, 23);
            this.txtPicPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtPicPath.ForeColor = System.Drawing.Color.White;

            this.btnBrowsePic.Text = "...";
            this.btnBrowsePic.Location = new System.Drawing.Point(674, 21);
            this.btnBrowsePic.Size = new System.Drawing.Size(32, 25);
            this.btnBrowsePic.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowsePic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowsePic.Click += new System.EventHandler(this.btnBrowsePic_Click);

            this.pbPic.Location = new System.Drawing.Point(712, 18);
            this.pbPic.Size = new System.Drawing.Size(56, 68);
            this.pbPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPic.BackColor = System.Drawing.Color.FromArgb(20, 21, 23);

            this.groupBoxMedia.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblIcon, this.txtIconPath, this.btnBrowseIcon, this.btnAutoCover, this.pbIcon,
                this.lblPic, this.txtPicPath, this.btnBrowsePic, this.pbPic, this.btnOpenStudio
            });

            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 202);
            this.groupBoxSettings.Size = new System.Drawing.Size(776, 74);
            this.groupBoxSettings.Text = "3. Opciones y Emulador";
            this.groupBoxSettings.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.chkWidescreen.Text = "16:9 Widescreen";
            this.chkWidescreen.Location = new System.Drawing.Point(12, 20);
            this.chkWidescreen.Size = new System.Drawing.Size(120, 22);
            this.chkWidescreen.ForeColor = System.Drawing.Color.White;

            this.chkFastLoad.Text = "Lectura Rápida CDVD";
            this.chkFastLoad.Location = new System.Drawing.Point(138, 20);
            this.chkFastLoad.Size = new System.Drawing.Size(150, 22);
            this.chkFastLoad.Checked = true;
            this.chkFastLoad.ForeColor = System.Drawing.Color.White;

            this.lblEmulator.Text = "Emulador:";
            this.lblEmulator.Location = new System.Drawing.Point(298, 21);
            this.lblEmulator.Size = new System.Drawing.Size(65, 20);
            this.lblEmulator.ForeColor = System.Drawing.Color.Gainsboro;

            this.cmbEmulator.Location = new System.Drawing.Point(365, 18);
            this.cmbEmulator.Size = new System.Drawing.Size(200, 23);
            this.cmbEmulator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmulator.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.cmbEmulator.ForeColor = System.Drawing.Color.White;

            this.btnSendToPs4.Text = "🚀 A PS4...";
            this.btnSendToPs4.Location = new System.Drawing.Point(575, 17);
            this.btnSendToPs4.Size = new System.Drawing.Size(95, 26);
            this.btnSendToPs4.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnSendToPs4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendToPs4.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnSendToPs4.ForeColor = System.Drawing.Color.White;
            this.btnSendToPs4.Click += new System.EventHandler(this.btnSendToPs4_Click);

            this.btnTogglePaths.Text = "⚙️ Rutas";
            this.btnTogglePaths.Location = new System.Drawing.Point(676, 17);
            this.btnTogglePaths.Size = new System.Drawing.Size(92, 26);
            this.btnTogglePaths.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnTogglePaths.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTogglePaths.Click += new System.EventHandler(this.btnTogglePaths_Click);

            // Badge / Línea informativa del parche comunitario
            this.lblPatchInfo.Text = "🛡️ Parche Comunitario: Esperando juego...";
            this.lblPatchInfo.Location = new System.Drawing.Point(12, 48);
            this.lblPatchInfo.Size = new System.Drawing.Size(756, 18);
            this.lblPatchInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPatchInfo.ForeColor = System.Drawing.Color.Gray;

            this.groupBoxSettings.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.chkWidescreen, this.chkFastLoad,
                this.lblEmulator, this.cmbEmulator,
                this.btnSendToPs4, this.btnTogglePaths,
                this.lblPatchInfo
            });

            // 
            // pnlPaths (Collapsible)
            // 
            this.pnlPaths.Location = new System.Drawing.Point(12, 280);
            this.pnlPaths.Size = new System.Drawing.Size(776, 54);
            this.pnlPaths.Visible = false;
            this.pnlPaths.BackColor = System.Drawing.Color.FromArgb(34, 36, 40);

            this.lblOrbis.Text = "orbis-pub-cmd:";
            this.lblOrbis.Location = new System.Drawing.Point(8, 6);
            this.lblOrbis.Size = new System.Drawing.Size(90, 18);
            this.lblOrbis.ForeColor = System.Drawing.Color.Silver;

            this.txtOrbisPath.Location = new System.Drawing.Point(100, 4);
            this.txtOrbisPath.Size = new System.Drawing.Size(625, 21);
            this.txtOrbisPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtOrbisPath.ForeColor = System.Drawing.Color.White;

            this.btnBrowseOrbis.Text = "...";
            this.btnBrowseOrbis.Location = new System.Drawing.Point(730, 3);
            this.btnBrowseOrbis.Size = new System.Drawing.Size(38, 23);
            this.btnBrowseOrbis.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowseOrbis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseOrbis.Click += new System.EventHandler(this.btnBrowseOrbis_Click);

            this.lblOutput.Text = "Destino PKG:";
            this.lblOutput.Location = new System.Drawing.Point(8, 30);
            this.lblOutput.Size = new System.Drawing.Size(90, 18);
            this.lblOutput.ForeColor = System.Drawing.Color.Silver;

            this.txtOutputPath.Location = new System.Drawing.Point(100, 28);
            this.txtOutputPath.Size = new System.Drawing.Size(625, 21);
            this.txtOutputPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtOutputPath.ForeColor = System.Drawing.Color.White;

            this.btnBrowseOutput.Text = "...";
            this.btnBrowseOutput.Location = new System.Drawing.Point(730, 27);
            this.btnBrowseOutput.Size = new System.Drawing.Size(38, 23);
            this.btnBrowseOutput.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);

            this.pnlPaths.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblOrbis, this.txtOrbisPath, this.btnBrowseOrbis,
                this.lblOutput, this.txtOutputPath, this.btnBrowseOutput
            });

            // 
            // btnBuild
            // 
            this.btnBuild.Text = "COMPILAR PKG DE PS4";
            this.btnBuild.Location = new System.Drawing.Point(12, 282);
            this.btnBuild.Size = new System.Drawing.Size(776, 36);
            this.btnBuild.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnBuild.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuild.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnBuild.ForeColor = System.Drawing.Color.White;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);

            // 
            // progressBar & status
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 324);
            this.progressBar.Size = new System.Drawing.Size(725, 16);

            this.lblProgress.Text = "0%";
            this.lblProgress.Location = new System.Drawing.Point(742, 322);
            this.lblProgress.Size = new System.Drawing.Size(46, 18);
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProgress.ForeColor = System.Drawing.Color.Gold;

            this.lblStatus.Text = "Listo.";
            this.lblStatus.Location = new System.Drawing.Point(12, 344);
            this.lblStatus.Size = new System.Drawing.Size(776, 18);
            this.lblStatus.ForeColor = System.Drawing.Color.Silver;

            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Location = new System.Drawing.Point(12, 365);
            this.groupBoxLog.Size = new System.Drawing.Size(776, 245);
            this.groupBoxLog.Text = "4. Registro de Compilación y Salida";
            this.groupBoxLog.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.btnCopyLog.Text = "Copiar";
            this.btnCopyLog.Location = new System.Drawing.Point(635, 14);
            this.btnCopyLog.Size = new System.Drawing.Size(65, 22);
            this.btnCopyLog.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnCopyLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyLog.ForeColor = System.Drawing.Color.White;
            this.btnCopyLog.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnCopyLog.Click += new System.EventHandler(this.btnCopyLog_Click);

            this.btnClearLog.Text = "Limpiar";
            this.btnClearLog.Location = new System.Drawing.Point(705, 14);
            this.btnClearLog.Size = new System.Drawing.Size(60, 22);
            this.btnClearLog.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.ForeColor = System.Drawing.Color.White;
            this.btnClearLog.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);

            this.txtLog.Location = new System.Drawing.Point(10, 40);
            this.txtLog.Size = new System.Drawing.Size(756, 195);
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.txtLog.WordWrap = false;
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(16, 17, 19);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(220, 221, 225);
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this.groupBoxLog.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnCopyLog,
                this.btnClearLog,
                this.txtLog
            });

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.groupBoxIso,
                this.groupBoxMedia,
                this.groupBoxSettings,
                this.pnlPaths,
                this.btnBuild,
                this.progressBar,
                this.lblProgress,
                this.lblStatus,
                this.groupBoxLog
            });

            this.groupBoxIso.ResumeLayout(false);
            this.groupBoxIso.PerformLayout();
            this.groupBoxMedia.ResumeLayout(false);
            this.groupBoxMedia.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPic)).EndInit();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.pnlPaths.ResumeLayout(false);
            this.pnlPaths.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox groupBoxIso;
        private System.Windows.Forms.Label lblIso;
        private System.Windows.Forms.TextBox txtIsoPath;
        private System.Windows.Forms.Button btnBrowseIso;
        private System.Windows.Forms.Button btnOpenBatch;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtGameTitle;
        private System.Windows.Forms.Label lblSerial;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Label lblTitleId;
        private System.Windows.Forms.TextBox txtTitleId;

        private System.Windows.Forms.GroupBox groupBoxMedia;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.TextBox txtIconPath;
        private System.Windows.Forms.Button btnBrowseIcon;
        private System.Windows.Forms.Button btnAutoCover;
        private System.Windows.Forms.Button btnOpenStudio;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.TextBox txtPicPath;
        private System.Windows.Forms.Button btnBrowsePic;
        private System.Windows.Forms.PictureBox pbPic;

        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.CheckBox chkWidescreen;
        private System.Windows.Forms.CheckBox chkFastLoad;
        private System.Windows.Forms.Label lblEmulator;
        private System.Windows.Forms.ComboBox cmbEmulator;
        private System.Windows.Forms.Button btnSendToPs4;
        private System.Windows.Forms.Button btnTogglePaths;
        private System.Windows.Forms.Label lblPatchInfo;

        private System.Windows.Forms.Panel pnlPaths;
        private System.Windows.Forms.Label lblOrbis;
        private System.Windows.Forms.TextBox txtOrbisPath;
        private System.Windows.Forms.Button btnBrowseOrbis;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnBrowseOutput;

        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.Button btnCopyLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}
