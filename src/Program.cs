using System;
using System.Windows.Forms;
using PS2toPS4Builder.UI;

namespace PS2toPS4Builder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error al iniciar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
