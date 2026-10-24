namespace PS2toPS4Builder.UI
{
    partial class SendToPs4Form
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
            this.groupBoxPs4 = new System.Windows.Forms.GroupBox();
            this.lblPs4Ip = new System.Windows.Forms.Label();
            this.txtPs4Ip = new System.Windows.Forms.TextBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.lblLocalIp = new System.Windows.Forms.Label();

            this.groupBoxPkg = new System.Windows.Forms.GroupBox();
            this.lblPkg = new System.Windows.Forms.Label();
            this.txtPkgPath = new System.Windows.Forms.TextBox();
            this.btnBrowsePkg = new System.Windows.Forms.Button();

            this.btnSend = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();

            this.groupBoxPs4.SuspendLayout();
            this.groupBoxPkg.SuspendLayout();
            this.SuspendLayout();

            // 
            // SendToPs4Form
            // 
            this.ClientSize = new System.Drawing.Size(660, 430);
            this.Text = "Instalador Remoto de PKG por Red (Remote Package Installer)";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(28, 30, 33);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            // 
            // groupBoxPs4
            // 
            this.groupBoxPs4.Location = new System.Drawing.Point(12, 10);
            this.groupBoxPs4.Size = new System.Drawing.Size(636, 65);
            this.groupBoxPs4.Text = "1. Conexión de Red con la PS4 (GoldHEN)";
            this.groupBoxPs4.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.lblPs4Ip.Text = "IP de tu PS4:";
            this.lblPs4Ip.Location = new System.Drawing.Point(10, 26);
            this.lblPs4Ip.Size = new System.Drawing.Size(75, 20);
            this.lblPs4Ip.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtPs4Ip.Location = new System.Drawing.Point(90, 23);
            this.txtPs4Ip.Size = new System.Drawing.Size(140, 23);
            this.txtPs4Ip.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtPs4Ip.ForeColor = System.Drawing.Color.White;

            this.btnTestConnection.Text = "🔍 Probar Conexión";
            this.btnTestConnection.Location = new System.Drawing.Point(240, 22);
            this.btnTestConnection.Size = new System.Drawing.Size(130, 25);
            this.btnTestConnection.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);

            this.lblLocalIp.Text = "Tu IP local (PC): ...";
            this.lblLocalIp.Location = new System.Drawing.Point(385, 26);
            this.lblLocalIp.Size = new System.Drawing.Size(240, 20);
            this.lblLocalIp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblLocalIp.ForeColor = System.Drawing.Color.Silver;

            this.groupBoxPs4.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblPs4Ip, this.txtPs4Ip, this.btnTestConnection, this.lblLocalIp
            });

            // 
            // groupBoxPkg
            // 
            this.groupBoxPkg.Location = new System.Drawing.Point(12, 82);
            this.groupBoxPkg.Size = new System.Drawing.Size(636, 60);
            this.groupBoxPkg.Text = "2. Paquete PKG a Instalar";
            this.groupBoxPkg.ForeColor = System.Drawing.Color.FromArgb(92, 179, 255);

            this.lblPkg.Text = "Archivo:";
            this.lblPkg.Location = new System.Drawing.Point(10, 24);
            this.lblPkg.Size = new System.Drawing.Size(55, 20);
            this.lblPkg.ForeColor = System.Drawing.Color.Gainsboro;

            this.txtPkgPath.Location = new System.Drawing.Point(70, 21);
            this.txtPkgPath.Size = new System.Drawing.Size(515, 23);
            this.txtPkgPath.BackColor = System.Drawing.Color.FromArgb(42, 44, 49);
            this.txtPkgPath.ForeColor = System.Drawing.Color.White;

            this.btnBrowsePkg.Text = "...";
            this.btnBrowsePkg.Location = new System.Drawing.Point(592, 20);
            this.btnBrowsePkg.Size = new System.Drawing.Size(35, 25);
            this.btnBrowsePkg.BackColor = System.Drawing.Color.FromArgb(58, 62, 69);
            this.btnBrowsePkg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowsePkg.Click += new System.EventHandler(this.btnBrowsePkg_Click);

            this.groupBoxPkg.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblPkg, this.txtPkgPath, this.btnBrowsePkg
            });

            // 
            // btnSend
            // 
            this.btnSend.Text = "🚀 ENVIAR E INSTALAR EN PS4";
            this.btnSend.Location = new System.Drawing.Point(12, 150);
            this.btnSend.Size = new System.Drawing.Size(636, 40);
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            // 
            // progressBar & status
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 198);
            this.progressBar.Size = new System.Drawing.Size(500, 16);

            this.lblProgress.Text = "0%";
            this.lblProgress.Location = new System.Drawing.Point(518, 196);
            this.lblProgress.Size = new System.Drawing.Size(130, 20);
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblProgress.ForeColor = System.Drawing.Color.Gold;

            this.lblStatus.Text = "Listo para transmitir.";
            this.lblStatus.Location = new System.Drawing.Point(12, 218);
            this.lblStatus.Size = new System.Drawing.Size(636, 20);
            this.lblStatus.ForeColor = System.Drawing.Color.LightSkyBlue;

            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 242);
            this.txtLog.Size = new System.Drawing.Size(636, 175);
            this.txtLog.Multiline = true;
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(16, 17, 19);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(80, 250, 123);
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.groupBoxPs4,
                this.groupBoxPkg,
                this.btnSend,
                this.progressBar,
                this.lblProgress,
                this.lblStatus,
                this.txtLog
            });

            this.groupBoxPs4.ResumeLayout(false);
            this.groupBoxPs4.PerformLayout();
            this.groupBoxPkg.ResumeLayout(false);
            this.groupBoxPkg.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.GroupBox groupBoxPs4;
        private System.Windows.Forms.Label lblPs4Ip;
        private System.Windows.Forms.TextBox txtPs4Ip;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblLocalIp;

        private System.Windows.Forms.GroupBox groupBoxPkg;
        private System.Windows.Forms.Label lblPkg;
        private System.Windows.Forms.TextBox txtPkgPath;
        private System.Windows.Forms.Button btnBrowsePkg;

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtLog;
    }
}
