using System;
using System.IO;
using System.Windows.Forms;

namespace EWOS
{
    public partial class FrmSettings : Form
    {
        private string _path;

        public FrmSettings(string path)
        {
            // ReSharper disable RedundantThisQualifier
            this._path = path;
            // ReSharper restore RedundantThisQualifier
            InitializeComponent();
            textBox1.Text = path;
        }

        private void BtnApplyClick(object sender, EventArgs e)
        {
            string dir = Environment.GetEnvironmentVariable("USERPROFILE");
            // ReSharper disable RedundantThisQualifier
            if (this._path != @"C:\" && this._path != dir + @"wurm")
            // ReSharper restore RedundantThisQualifier
            {
                TextWriter save = new StreamWriter("config.dat");
                // ReSharper disable RedundantThisQualifier
                this._path = textBox1.Text;
                // ReSharper restore RedundantThisQualifier
                save.WriteLine(this._path);
                save.Flush();
                save.Close();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(@"I detected that you are still using default values for the folder your logs are stored in, this needs to be changed so we can detect your log files.");
            }
        }

        private void BtnOpenFolderClick(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.SelectedPath = this._path;

            if (d.ShowDialog() == DialogResult.OK)
            {
                this._path = d.SelectedPath;
                textBox1.Text = d.SelectedPath;
            }
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}