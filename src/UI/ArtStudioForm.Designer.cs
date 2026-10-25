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

            // Hidden target fields to maintain compatibility without UI clutter
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.txtGameName = new System.Windows.Forms.TextBox();

            this.grpPreviews = new System.Windows.Forms.GroupBox();
            this.pnlIconBox = new System.Windows.Forms.Panel();
            this.lblIconTitle = new System.Windows.Forms.Label();
            this.cmbIconStyle = new System.Windows.Forms.ComboBox();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.btnSaveIcon = new System.Windows.Forms.Button();

            this.pnlBgBox = new System.Windows.Forms.Panel();
            this.lblBgTitle = new System.Windows.Forms.Label();
            this.picBg = new System.Windows.Forms.PictureBox();
            this.btnSaveBg = new System.Windows.Forms.Button();

            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSaveBoth = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.pnlTop.SuspendLayout();
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
            this.pnlTop.Size = new System.Drawing.Size(740, 50);

            // btnPaste
            this.btnPaste.Location = new System.Drawing.Point(12, 8);
            this.btnPaste.Size = new System.Drawing.Size(190, 34);
            this.btnPaste.Text = "📋 Pegar Imagen (Ctrl+V)";
            this.btnPaste.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnPaste.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnPaste.ForeColor = System.Drawing.Color.White;
            this.btnPaste.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);

            // btnBrowseImage
            this.btnBrowseImage.Location = new System.Drawing.Point(208, 8);
            this.btnBrowseImage.Size = new System.Drawing.Size(130, 34);
            this.btnBrowseImage.Text = "📁 Cargar Archivo...";
            this.btnBrowseImage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowseImage.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnBrowseImage.ForeColor = System.Drawing.Color.White;
            this.btnBrowseImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseImage.Click += new System.EventHandler(this.btnBrowseImage_Click);

            // lblImageInfo
            this.lblImageInfo.Location = new System.Drawing.Point(345, 8);
            this.lblImageInfo.Size = new System.Drawing.Size(385, 34);
            this.lblImageInfo.Text = "Copia cualquier imagen (Google) y pulsa Ctrl+V o arrástrala aquí.";
            this.lblImageInfo.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.lblImageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            // Hidden textboxes (kept for internal folder/serial tracking)
            //
            this.txtFolderPath.Visible = false;
            this.txtGameName.Visible = false;

            //
            // grpPreviews
            //
            this.grpPreviews.Controls.Add(this.pnlIconBox);
            this.grpPreviews.Controls.Add(this.pnlBgBox);
            this.grpPreviews.Location = new System.Drawing.Point(12, 54);
            this.grpPreviews.Size = new System.Drawing.Size(716, 316);
            this.grpPreviews.Text = " Vista Previa (Adaptación Automática Oficial PS4) ";
            this.grpPreviews.ForeColor = System.Drawing.Color.Gainsboro;

            // pnlIconBox (Left: 512x512 preview scaled)
            this.pnlIconBox.Location = new System.Drawing.Point(12, 22);
            this.pnlIconBox.Size = new System.Drawing.Size(235, 280);
            this.pnlIconBox.Controls.Add(this.lblIconTitle);
            this.pnlIconBox.Controls.Add(this.cmbIconStyle);
            this.pnlIconBox.Controls.Add(this.picIcon);
            this.pnlIconBox.Controls.Add(this.btnSaveIcon);

            // lblIconTitle
            this.lblIconTitle.Text = "Cuadrado 512x512 (Icono PS4)";
            this.lblIconTitle.Location = new System.Drawing.Point(0, 0);
            this.lblIconTitle.Size = new System.Drawing.Size(235, 18);
            this.lblIconTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIconTitle.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblIconTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // cmbIconStyle
            this.cmbIconStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIconStyle.Location = new System.Drawing.Point(17, 20);
            this.cmbIconStyle.Size = new System.Drawing.Size(200, 23);
            this.cmbIconStyle.BackColor = System.Drawing.Color.FromArgb(32, 34, 38);
            this.cmbIconStyle.ForeColor = System.Drawing.Color.White;
            this.cmbIconStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cmbIconStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbIconStyle.Items.AddRange(new object[] {
                "✨ Llenar Cuadrado (Estilo PS4)",
                "⬛ Fondo Negro Puro (Elegante)",
                "🌫️ Fondo Difuminado (Clásico)"
            });
            this.cmbIconStyle.SelectedIndex = 0;

            // picIcon (200 x 185)
            this.picIcon.Location = new System.Drawing.Point(17, 47);
            this.picIcon.Size = new System.Drawing.Size(200, 182);
            this.picIcon.BackColor = System.Drawing.Color.FromArgb(20, 20, 22);
            this.picIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // btnSaveIcon
            this.btnSaveIcon.Location = new System.Drawing.Point(17, 235);
            this.btnSaveIcon.Size = new System.Drawing.Size(200, 34);
            this.btnSaveIcon.Text = "💾 Guardar Solo Icono";
            this.btnSaveIcon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveIcon.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnSaveIcon.ForeColor = System.Drawing.Color.White;
            this.btnSaveIcon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveIcon.Click += new System.EventHandler(this.btnSaveIcon_Click);

            // pnlBgBox (Right: 1920x1080 preview scaled to 16:9)
            this.pnlBgBox.Location = new System.Drawing.Point(260, 22);
            this.pnlBgBox.Size = new System.Drawing.Size(445, 280);
            this.pnlBgBox.Controls.Add(this.lblBgTitle);
            this.pnlBgBox.Controls.Add(this.picBg);
            this.pnlBgBox.Controls.Add(this.btnSaveBg);

            // lblBgTitle
            this.lblBgTitle.Text = "Panorámico 1920x1080 (Fondo Cinemático)";
            this.lblBgTitle.Location = new System.Drawing.Point(0, 0);
            this.lblBgTitle.Size = new System.Drawing.Size(445, 20);
            this.lblBgTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBgTitle.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblBgTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // picBg (356 x 200 -> Aspect Ratio 16:9 exacto)
            this.picBg.Location = new System.Drawing.Point(44, 25);
            this.picBg.Size = new System.Drawing.Size(356, 200);
            this.picBg.BackColor = System.Drawing.Color.FromArgb(20, 20, 22);
            this.picBg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

            // btnSaveBg
            this.btnSaveBg.Location = new System.Drawing.Point(44, 235);
            this.btnSaveBg.Size = new System.Drawing.Size(356, 34);
            this.btnSaveBg.Text = "🖼️ Guardar Solo Fondo";
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
            this.pnlBottom.Location = new System.Drawing.Point(0, 384);
            this.pnlBottom.Size = new System.Drawing.Size(740, 52);

            // btnSaveBoth
            this.btnSaveBoth.Location = new System.Drawing.Point(12, 8);
            this.btnSaveBoth.Size = new System.Drawing.Size(280, 36);
            this.btnSaveBoth.Text = "✔ APLICAR ARTE AL PROYECTO";
            this.btnSaveBoth.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSaveBoth.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnSaveBoth.ForeColor = System.Drawing.Color.White;
            this.btnSaveBoth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveBoth.Click += new System.EventHandler(this.btnSaveBoth_Click);

            // btnOpenFolder
            this.btnOpenFolder.Location = new System.Drawing.Point(300, 8);
            this.btnOpenFolder.Size = new System.Drawing.Size(120, 36);
            this.btnOpenFolder.Text = "📁 Abrir Carpeta";
            this.btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenFolder.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            this.btnOpenFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(426, 10);
            this.lblStatus.Size = new System.Drawing.Size(302, 32);
            this.lblStatus.Text = "Listo. Pega una imagen con Ctrl+V.";
            this.lblStatus.ForeColor = System.Drawing.Color.LightGreen;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);

            //
            // ArtStudioForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(47, 49, 54);
            this.ClientSize = new System.Drawing.Size(740, 436);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.grpPreviews);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.txtGameName);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArtStudioForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PS2 / PS4 Artwork Studio - Copiar y Pegar Arte";

            this.pnlTop.ResumeLayout(false);
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

        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.TextBox txtGameName;

        private System.Windows.Forms.GroupBox grpPreviews;
        private System.Windows.Forms.Panel pnlIconBox;
        private System.Windows.Forms.Label lblIconTitle;
        private System.Windows.Forms.ComboBox cmbIconStyle;
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.Button btnSaveIcon;

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
