namespace PS2toPS4Builder.UI
{
    partial class BatchForm
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
            this.components = new System.ComponentModel.Container();
            this.btnAddFiles = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.listViewGames = new System.Windows.Forms.ListView();
            this.colIndex = new System.Windows.Forms.ColumnHeader();
            this.colTitle = new System.Windows.Forms.ColumnHeader();
            this.colSerial = new System.Windows.Forms.ColumnHeader();
            this.colTitleId = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.colStatus = new System.Windows.Forms.ColumnHeader();
            this.contextMenuGames = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemRemove = new System.Windows.Forms.ToolStripMenuItem();

            this.groupBoxBatchSettings = new System.Windows.Forms.GroupBox();
            this.lblOutputDir = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.chkWidescreen = new System.Windows.Forms.CheckBox();
            this.chkFastLoad = new System.Windows.Forms.CheckBox();
            this.lblEmulator = new System.Windows.Forms.Label();
            this.cmbEmulator = new System.Windows.Forms.ComboBox();

            this.btnStart = new System.Windows.Forms.Button();
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();

            this.groupBoxBatchSettings.SuspendLayout();
            this.contextMenuGames.SuspendLayout();
            this.SuspendLayout();

            // 
            // BatchForm
            // 
            this.ClientSize = new System.Drawing.Size(760, 520);
            this.Text = "Modo por Lotes - Conversor Masivo de PS2 a PS4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 30, 33);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            // 
            // Top buttons
            // 
            this.btnAddFiles.Text = "+ Añadir ISOs...";
            this.btnAddFiles.Location = new System.Drawing.Point(12, 12);
            this.btnAddFiles.Size = new System.Drawing.Size(115, 28);
            this.btnAddFiles.BackColor = System.Drawing.Color.FromArgb(88, 101, 242);
            this.btnAddFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFiles.ForeColor = System.Drawing.Color.White;
            this.btnAddFiles.Click += new System.EventHandler(this.btnAddFiles_Click);

            this.btnAddFolder.Text = "📁 Añadir Carpeta...";
            this.btnAddFolder.Location = new System.Drawing.Point(132, 12);
            this.btnAddFolder.Size = new System.Drawing.Size(125, 28);
            this.btnAddFolder.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnAddFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFolder.ForeColor = System.Drawing.Color.White;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);

            this.btnRemoveSelected.Text = "❌ Quitar Juego";
            this.btnRemoveSelected.Location = new System.Drawing.Point(262, 12);
            this.btnRemoveSelected.Size = new System.Drawing.Size(118, 28);
            this.btnRemoveSelected.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnRemoveSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveSelected.ForeColor = System.Drawing.Color.White;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);

            this.btnClear.Text = "🗑️ Limpiar Todo";
            this.btnClear.Location = new System.Drawing.Point(385, 12);
            this.btnClear.Size = new System.Drawing.Size(105, 28);
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            this.lblTotal.Text = "Total en cola: 0 juegos";
            this.lblTotal.Location = new System.Drawing.Point(495, 17);
            this.lblTotal.Size = new System.Drawing.Size(253, 20);
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTotal.ForeColor = System.Drawing.Color.Gold;

            // 
            // Context Menu
            // 
            this.menuItemRemove.Text = "❌ Quitar de la lista (Supr)";
            this.menuItemRemove.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            this.contextMenuGames.Items.Add(this.menuItemRemove);

            // 
            // listViewGames
            // 
            this.listViewGames.Location = new System.Drawing.Point(12, 48);
            this.listViewGames.Size = new System.Drawing.Size(736, 260);
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.FullRowSelect = true;
            this.listViewGames.GridLines = true;
            this.listViewGames.BackColor = System.Drawing.Color.FromArgb(18, 19, 21);
            this.listViewGames.ForeColor = System.Drawing.Color.White;
            this.listViewGames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewGames.ContextMenuStrip = this.contextMenuGames;
            this.listViewGames.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewGames_KeyDown);

            this.colIndex.Text = "#";
            this.colIndex.Width = 35;

            this.colTitle.Text = "Título Oficial";
            this.colTitle.Width = 270;

            this.colSerial.Text = "Serial";
            this.colSerial.Width = 95;

            this.colTitleId.Text = "PS4 ID";
            this.colTitleId.Width = 85;

            this.colSize.Text = "Tamaño";
            this.colSize.Width = 75;

            this.colStatus.Text = "Estado";
            this.colStatus.Width = 175;

            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colIndex,
                this.colTitle,
                this.colSerial,
                this.colTitleId,
                this.colSize,
                this.colStatus
            });

            // 
            // groupBoxBatchSettings
            // 
            this.groupBoxBatchSettings.Location = new System.Drawing.Point(12, 316);
            this.groupBoxBatchSettings.Size = new System.Drawing.Size(736, 85);
            this.groupBoxBatchSettings.Text = "Opciones para todo el Lote";
            this.groupBoxBatchSettings.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.lblOutputDir.Text = "Carpeta Salida PKGs:";
            this.lblOutputDir.Location = new System.Drawing.Point(10, 24);
            this.lblOutputDir.Size = new System.Drawing.Size(125, 20);
            this.lblOutputDir.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtOutputDir.Location = new System.Drawing.Point(140, 21);
            this.txtOutputDir.Size = new System.Drawing.Size(500, 23);
            this.txtOutputDir.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtOutputDir.ForeColor = System.Drawing.Color.White;

            this.btnBrowseOutput.Text = "...";
            this.btnBrowseOutput.Location = new System.Drawing.Point(646, 20);
            this.btnBrowseOutput.Size = new System.Drawing.Size(35, 25);
            this.btnBrowseOutput.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);

            this.chkWidescreen.Text = "16:9 Widescreen";
            this.chkWidescreen.Location = new System.Drawing.Point(10, 54);
            this.chkWidescreen.Size = new System.Drawing.Size(120, 22);
            this.chkWidescreen.ForeColor = System.Drawing.Color.White;

            this.chkFastLoad.Text = "Lectura Rápida CDVD";
            this.chkFastLoad.Location = new System.Drawing.Point(140, 54);
            this.chkFastLoad.Size = new System.Drawing.Size(155, 22);
            this.chkFastLoad.Checked = true;
            this.chkFastLoad.ForeColor = System.Drawing.Color.White;

            this.lblEmulator.Text = "Emulador:";
            this.lblEmulator.Location = new System.Drawing.Point(310, 55);
            this.lblEmulator.Size = new System.Drawing.Size(65, 20);
            this.lblEmulator.ForeColor = System.Drawing.Color.Gainsboro;

            this.cmbEmulator.Location = new System.Drawing.Point(380, 52);
            this.cmbEmulator.Size = new System.Drawing.Size(200, 23);
            this.cmbEmulator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmulator.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.cmbEmulator.ForeColor = System.Drawing.Color.White;

            this.groupBoxBatchSettings.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblOutputDir, this.txtOutputDir, this.btnBrowseOutput,
                this.chkWidescreen, this.chkFastLoad,
                this.lblEmulator, this.cmbEmulator
            });

            // 
            // btnStart
            // 
            this.btnStart.Text = "INICIAR PROCESAMIENTO EN LOTE";
            this.btnStart.Location = new System.Drawing.Point(12, 410);
            this.btnStart.Size = new System.Drawing.Size(736, 40);
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // 
            // progressBar & status
            // 
            this.progressBarTotal.Location = new System.Drawing.Point(12, 458);
            this.progressBarTotal.Size = new System.Drawing.Size(736, 16);

            this.lblStatus.Text = "Listo para iniciar.";
            this.lblStatus.Location = new System.Drawing.Point(12, 480);
            this.lblStatus.Size = new System.Drawing.Size(736, 22);
            this.lblStatus.ForeColor = System.Drawing.Color.Silver;

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnAddFiles,
                this.btnAddFolder,
                this.btnRemoveSelected,
                this.btnClear,
                this.lblTotal,
                this.listViewGames,
                this.groupBoxBatchSettings,
                this.btnStart,
                this.progressBarTotal,
                this.lblStatus
            });

            this.groupBoxBatchSettings.ResumeLayout(false);
            this.groupBoxBatchSettings.PerformLayout();
            this.contextMenuGames.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.ColumnHeader colSerial;
        private System.Windows.Forms.ColumnHeader colTitleId;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.ContextMenuStrip contextMenuGames;
        private System.Windows.Forms.ToolStripMenuItem menuItemRemove;

        private System.Windows.Forms.GroupBox groupBoxBatchSettings;
        private System.Windows.Forms.Label lblOutputDir;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.CheckBox chkWidescreen;
        private System.Windows.Forms.CheckBox chkFastLoad;
        private System.Windows.Forms.Label lblEmulator;
        private System.Windows.Forms.ComboBox cmbEmulator;

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.Label lblStatus;
    }
}
