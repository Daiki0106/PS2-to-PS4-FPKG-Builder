using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PS2toPS4Builder.Core;

namespace PS2toPS4Builder.UI
{
    public partial class SendToPs4Form : Form
    {
        private RemoteInstaller _installer = new RemoteInstaller();
        private string _pkgPath;

        public SendToPs4Form(string pkgPath)
        {
            _pkgPath = pkgPath;
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            if (File.Exists(_pkgPath))
            {
                txtPkgPath.Text = _pkgPath;
            }
            else
            {
                // Buscar el PKG más reciente en la carpeta de salida predeterminada
                try
                {
                    string defaultDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PKGs_PS4");
                    if (Directory.Exists(defaultDir))
                    {
                        var pkgs = new DirectoryInfo(defaultDir).GetFiles("*.pkg");
                        if (pkgs.Length > 0)
                        {
                            Array.Sort(pkgs, (a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));
                            txtPkgPath.Text = pkgs[0].FullName;
                            _pkgPath = pkgs[0].FullName;
                        }
                    }
                }
                catch { }
            }

            lblLocalIp.Text = string.Format("Tu IP local (PC): {0}", RemoteInstaller.GetLocalIpAddress());

            _installer.OnLog += LogMessage;
            _installer.OnTransferProgress += UpdateTransferProgress;

            // Cargar última IP de PS4 si existe en archivo local
            string ipFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "ps4_ip.txt");
            if (File.Exists(ipFile))
            {
                try { txtPs4Ip.Text = File.ReadAllText(ipFile).Trim(); } catch { }
            }
            else
            {
                txtPs4Ip.Text = "192.168.1.";
            }

            this.FormClosing += (s, e) => _installer.StopServer();
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            string ip = txtPs4Ip.Text.Trim();
            if (string.IsNullOrEmpty(ip) || ip.Length < 7)
            {
                MessageBox.Show("Ingresa una dirección IP válida para tu PS4.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblStatus.Text = "Comprobando conexión con PS4 en " + ip + "...";
            btnTestConnection.Enabled = false;

            bool online = await _installer.CheckPs4ConnectionAsync(ip);

            btnTestConnection.Enabled = true;
            if (online)
            {
                lblStatus.Text = "¡Conexión exitosa! PS4 detectada en el puerto 12800.";
                lblStatus.ForeColor = Color.LightGreen;
            }
            else
            {
                lblStatus.Text = "No se pudo conectar. Verifica que GoldHEN esté activo.";
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string ip = txtPs4Ip.Text.Trim();
            if (string.IsNullOrEmpty(ip) || ip.Length < 7)
            {
                MessageBox.Show("Ingresa la IP de tu PS4.", "Falta IP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pkg = txtPkgPath.Text.Trim();
            if (!File.Exists(pkg))
            {
                MessageBox.Show("El archivo PKG seleccionado no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Guardar IP para la próxima vez
            try
            {
                string ipDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                if (!Directory.Exists(ipDir)) Directory.CreateDirectory(ipDir);
                File.WriteAllText(Path.Combine(ipDir, "ps4_ip.txt"), ip);
            }
            catch { }

            btnSend.Enabled = false;
            progressBar.Value = 0;
            lblStatus.Text = "Enviando comando de instalación a la PS4...";
            lblStatus.ForeColor = Color.LightSkyBlue;

            bool ok = await _installer.SendInstallCommandAsync(ip, pkg);

            btnSend.Enabled = true;
            if (ok)
            {
                lblStatus.Text = "¡Instalación enviada! Observa las notificaciones en tu PS4.";
                lblStatus.ForeColor = Color.LightGreen;
                MessageBox.Show("¡Instalación enviada con éxito a la PS4!\n\nVerás la notificación de descarga en la esquina superior de tu pantalla de televisión.", "Envío Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Error al enviar la instalación. Revisa el registro.";
                lblStatus.ForeColor = Color.FromArgb(255, 120, 120);
            }
        }

        private void btnBrowsePkg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar archivo PKG para enviar a PS4";
                ofd.Filter = "Paquetes de PS4 (*.pkg)|*.pkg|Todos los archivos (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPkgPath.Text = ofd.FileName;
                    _pkgPath = ofd.FileName;
                }
            }
        }

        private void LogMessage(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), msg);
                return;
            }
            txtLog.AppendText(string.Format("[{0:HH:mm:ss}] {1}{2}", DateTime.Now, msg, Environment.NewLine));
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void UpdateTransferProgress(long sent, long total)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<long, long>(UpdateTransferProgress), sent, total);
                return;
            }

            if (total > 0)
            {
                int pct = (int)((sent * 100) / total);
                progressBar.Value = Math.Max(0, Math.Min(pct, 100));

                double sentMb = sent / (1024.0 * 1024.0);
                double totalMb = total / (1024.0 * 1024.0);
                lblProgress.Text = string.Format("{0}% ({1:0.0} / {2:0.0} MB)", pct, sentMb, totalMb);
            }
        }
    }
}
