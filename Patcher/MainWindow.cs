using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patcher
{
    public partial class MainWindow : Form
    {
        private string executable { get; set; }

        private bool getExecutableLocation()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "PlantsVsZombies.exe|*.exe";

                DialogResult res = dialog.ShowDialog();

                if (res == DialogResult.Cancel || dialog.FileName == null)
                    return false;

                executable = dialog.FileName;

                return true;
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            foreach(var patch in Patches.PatchList)
            {
                PatchesListBox.Items.Add(patch.Key);
            }

            if (!getExecutableLocation())
                Application.Exit();


            // toggle minimize -> normal
            // because OpenFileDialog minimizes our window
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }



        private void ApplyButton_Click(object sender, EventArgs e)
        {
            foreach(var patchName in PatchesListBox.CheckedItems)
            {
                foreach(var patch in Patches.PatchList)
                {
                    if(patch.Key == patchName.ToString())
                    {
                        if(!Patcher.ApplyPatch(executable, patch.Value))
                        {
                            MessageBox.Show("Patching Failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            MessageBox.Show("Patching Success!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
