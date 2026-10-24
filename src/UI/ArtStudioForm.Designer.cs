namespace PS2toPS4Builder.UI
{
    partial class ArtStudioForm
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnBrowseImage = new System.Windows.Forms.Button();
            this.lblImageInfo = new System.Windows.Forms.Label();

            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.lblGameName = new System.Windows.Forms.Label();
            this.txtGameName = new System.Windows.Forms.TextBox();
            this.lblPs4FormatBadge = new System.Windows.Forms.Label();

            this.grpPreviews = new System.Windows.Forms.GroupBox();
            this.pnlIconBox = new System.Windows.Forms.Panel();
            this.lblIconTitle = new System.Windows.Forms.Label();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.btnSaveIcon = new System.Windows.Forms.Button();
            this.btnSaveDisc = new System.Windows.Forms.Button();

            this.pnlBgBox = new System.Windows.Forms.Panel();
            this.lblBgTitle = new System.Windows.Forms.Label();
            this.picBg = new System.Windows.Forms.PictureBox();
            this.btnSaveBg = new System.Windows.Forms.Button();

            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSaveBoth = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.pnlTop.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.grpPreviews.SuspendLayout();
            this.pnlIconBox.SuspendLayout();
            this.pnlBgBox.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBg)).BeginInit();
            this.SuspendLayout();

            //
            // pnlTop
            //
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(32, 34, 37);
            this.pnlTop.Controls.Add(this.btnPaste);
            this.pnlTop.Controls.Add(this.btnBrowseImage);
            this.pnlTop.Controls.Add(this.lblImageInfo);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Size = new System.Drawing.Size(860, 56);

            // btnPaste
            this.btnPaste.Location = new System.Drawing.Point(14, 10);
            this.btnPaste.Size = new System.Drawing.Size(210, 36);
            this.btnPaste.Text = "📋 Pegar Imagen (Ctrl+V)";
            this.btnPaste.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnPaste.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnPaste.ForeColor = System.Drawing.Color.White;
            this.btnPaste.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);

            // btnBrowseImage
            this.btnBrowseImage.Location = new System.Drawing.Point(232, 10);
            this.btnBrowseImage.Size = new System.Drawing.Size(160, 36);
            this.btnBrowseImage.Text = "📁 Cargar Archivo...";
            this.btnBrowseImage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowseImage.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnBrowseImage.ForeColor = System.Drawing.Color.White;
            this.btnBrowseImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseImage.Click += new System.EventHandler(this.btnBrowseImage_Click);

            // lblImageInfo
            this.lblImageInfo.Location = new System.Drawing.Point(404, 12);
            this.lblImageInfo.Size = new System.Drawing.Size(440, 32);
            this.lblImageInfo.Text = "Copia cualquier imagen (Google, navegador) y pulsa Ctrl+V o arrastra una imagen aquí.";
            this.lblImageInfo.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.lblImageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            // grpTarget
            //
            this.grpTarget.Controls.Add(this.lblFolder);
            this.grpTarget.Controls.Add(this.txtFolderPath);
            this.grpTarget.Controls.Add(this.btnBrowseFolder);
            this.grpTarget.Controls.Add(this.lblGameName);
            this.grpTarget.Controls.Add(this.txtGameName);
            this.grpTarget.Controls.Add(this.lblPs4FormatBadge);
            this.grpTarget.Location = new System.Drawing.Point(14, 64);
            this.grpTarget.Size = new System.Drawing.Size(832, 98);
            this.grpTarget.Text = " Destino y Nomenclatura Automática para PS4 ";
            this.grpTarget.ForeColor = System.Drawing.Color.Gainsboro;

            // lblFolder
            this.lblFolder.Text = "Carpeta:";
            this.lblFolder.Location = new System.Drawing.Point(14, 25);
            this.lblFolder.Size = new System.Drawing.Size(60, 20);

            // txtFolderPath
            this.txtFolderPath.Location = new System.Drawing.Point(76, 22);
            this.txtFolderPath.Size = new System.Drawing.Size(620, 23);
            this.txtFolderPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtFolderPath.ForeColor = System.Drawing.Color.White;

            // btnBrowseFolder
            this.btnBrowseFolder.Text = "Examinar...";
            this.btnBrowseFolder.Location = new System.Drawing.Point(704, 21);
            this.btnBrowseFolder.Size = new System.Drawing.Size(114, 25);
            this.btnBrowseFolder.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowseFolder.ForeColor = System.Drawing.Color.White;
            this.btnBrowseFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);

            // lblGameName
            this.lblGameName.Text = "Nombre / Serial:";
            this.lblGameName.Location = new System.Drawing.Point(14, 60);
            this.lblGameName.Size = new System.Drawing.Size(100, 20);

            // txtGameName
            this.txtGameName.Location = new System.Drawing.Point(118, 57);
            this.txtGameName.Size = new System.Drawing.Size(260, 23);
            this.txtGameName.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtGameName.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.txtGameName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblPs4FormatBadge
            this.lblPs4FormatBadge.Text = "🎮 Formato Oficial PS4: icon0.png (512x512) y pic1.png (1920x1080)";
            this.lblPs4FormatBadge.Location = new System.Drawing.Point(390, 57);
            this.lblPs4FormatBadge.Size = new System.Drawing.Size(430, 23);
            this.lblPs4FormatBadge.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.lblPs4FormatBadge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPs4FormatBadge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            //
            // grpPreviews
            //
            this.grpPreviews.Controls.Add(this.pnlIconBox);
            this.grpPreviews.Controls.Add(this.pnlBgBox);
            this.grpPreviews.Location = new System.Drawing.Point(14, 168);
            this.grpPreviews.Size = new System.Drawing.Size(832, 400);
            this.grpPreviews.Text = " Vista Previa y Conversión Automática ";
            this.grpPreviews.ForeColor = System.Drawing.Color.Gainsboro;

            // pnlIconBox (Left: 512x512 preview scaled)
            this.pnlIconBox.Location = new System.Drawing.Point(14, 25);
            this.pnlIconBox.Size = new System.Drawing.Size(300, 360);
            this.pnlIconBox.Controls.Add(this.lblIconTitle);
            this.pnlIconBox.Controls.Add(this.picIcon);
            this.pnlIconBox.Controls.Add(this.btnSaveIcon);
            this.pnlIconBox.Controls.Add(this.btnSaveDisc);

            // lblIconTitle
            this.lblIconTitle.Text = "Cuadrado 512x512 (Icono / Portada)";
            this.lblIconTitle.Location = new System.Drawing.Point(0, 0);
            this.lblIconTitle.Size = new System.Drawing.Size(300, 20);
            this.lblIconTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIconTitle.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblIconTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // picIcon
            this.picIcon.Location = new System.Drawing.Point(20, 25);
            this.picIcon.Size = new System.Drawing.Size(260, 260);
            this.picIcon.BackColor = System.Drawing.Color.FromArgb(20, 20, 22);
            this.picIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // btnSaveIcon
            this.btnSaveIcon.Location = new System.Drawing.Point(0, 292);
            this.btnSaveIcon.Size = new System.Drawing.Size(300, 30);
            this.btnSaveIcon.Text = "💾 Guardar Icono (icon0.png)";
            this.btnSaveIcon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveIcon.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnSaveIcon.ForeColor = System.Drawing.Color.White;
            this.btnSaveIcon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveIcon.Click += new System.EventHandler(this.btnSaveIcon_Click);

            // btnSaveDisc
            this.btnSaveDisc.Location = new System.Drawing.Point(0, 326);
            this.btnSaveDisc.Size = new System.Drawing.Size(300, 26);
            this.btnSaveDisc.Text = "💿 Guardar Disco (icon0.png)";
            this.btnSaveDisc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnSaveDisc.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnSaveDisc.ForeColor = System.Drawing.Color.White;
            this.btnSaveDisc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDisc.Click += new System.EventHandler(this.btnSaveDisc_Click);

            // pnlBgBox (Right: 1920x1080 preview scaled)
            this.pnlBgBox.Location = new System.Drawing.Point(328, 25);
            this.pnlBgBox.Size = new System.Drawing.Size(490, 360);
            this.pnlBgBox.Controls.Add(this.lblBgTitle);
            this.pnlBgBox.Controls.Add(this.picBg);
            this.pnlBgBox.Controls.Add(this.btnSaveBg);

            // lblBgTitle
            this.lblBgTitle.Text = "Panorámico 1920x1080 (Fondo Cinemático)";
            this.lblBgTitle.Location = new System.Drawing.Point(0, 0);
            this.lblBgTitle.Size = new System.Drawing.Size(490, 20);
            this.lblBgTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBgTitle.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblBgTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // picBg (Aspect ratio 16:9 -> 480 x 270 approx)
            this.picBg.Location = new System.Drawing.Point(5, 25);
            this.picBg.Size = new System.Drawing.Size(480, 260);
            this.picBg.BackColor = System.Drawing.Color.FromArgb(20, 20, 22);
            this.picBg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // btnSaveBg
            this.btnSaveBg.Location = new System.Drawing.Point(5, 292);
            this.btnSaveBg.Size = new System.Drawing.Size(480, 30);
            this.btnSaveBg.Text = "🖼️ Guardar Fondo (pic1.png)";
            this.btnSaveBg.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveBg.BackColor = System.Drawing.Color.FromArgb(142, 68, 173);
            this.btnSaveBg.ForeColor = System.Drawing.Color.White;
            this.btnSaveBg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveBg.Click += new System.EventHandler(this.btnSaveBg_Click);

            //
            // pnlBottom
            //
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(32, 34, 37);
            this.pnlBottom.Controls.Add(this.btnSaveBoth);
            this.pnlBottom.Controls.Add(this.btnOpenFolder);
            this.pnlBottom.Controls.Add(this.lblStatus);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 580);
            this.pnlBottom.Size = new System.Drawing.Size(860, 52);

            // btnSaveBoth
            this.btnSaveBoth.Location = new System.Drawing.Point(14, 8);
            this.btnSaveBoth.Size = new System.Drawing.Size(260, 36);
            this.btnSaveBoth.Text = "⚡ GUARDAR AMBOS (icon0 + pic1)";
            this.btnSaveBoth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveBoth.BackColor = System.Drawing.Color.FromArgb(241, 196, 15);
            this.btnSaveBoth.ForeColor = System.Drawing.Color.Black;
            this.btnSaveBoth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveBoth.Click += new System.EventHandler(this.btnSaveBoth_Click);

            // btnOpenFolder
            this.btnOpenFolder.Location = new System.Drawing.Point(282, 8);
            this.btnOpenFolder.Size = new System.Drawing.Size(140, 36);
            this.btnOpenFolder.Text = "📁 Abrir Carpeta";
            this.btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenFolder.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnOpenFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(432, 10);
            this.lblStatus.Size = new System.Drawing.Size(412, 32);
            this.lblStatus.Text = "Listo. Pega una imagen con Ctrl+V.";
            this.lblStatus.ForeColor = System.Drawing.Color.LightGreen;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);

            //
            // ArtStudioForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(47, 49, 54);
            this.ClientSize = new System.Drawing.Size(860, 632);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.grpTarget);
            this.Controls.Add(this.grpPreviews);
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArtStudioForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PS2 / PS4 Artwork Studio - Copiar, Pegar y Redimensionar";

            this.pnlTop.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.grpPreviews.ResumeLayout(false);
            this.pnlIconBox.ResumeLayout(false);
            this.pnlBgBox.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBg)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnBrowseImage;
        private System.Windows.Forms.Label lblImageInfo;

        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.Label lblGameName;
        private System.Windows.Forms.TextBox txtGameName;
        private System.Windows.Forms.Label lblPs4FormatBadge;

        private System.Windows.Forms.GroupBox grpPreviews;
        private System.Windows.Forms.Panel pnlIconBox;
        private System.Windows.Forms.Label lblIconTitle;
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.Button btnSaveIcon;
        private System.Windows.Forms.Button btnSaveDisc;

        private System.Windows.Forms.Panel pnlBgBox;
        private System.Windows.Forms.Label lblBgTitle;
        private System.Windows.Forms.PictureBox picBg;
        private System.Windows.Forms.Button btnSaveBg;

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnSaveBoth;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Label lblStatus;
    }
}
