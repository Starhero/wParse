﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WParse;
using EWOS;
using Hero;

namespace EWOS
{
    public partial class FormMain : Form
    {
        private string Path
        {
            get;
            set;
        }
        private string User
        {
            get;
            set;
        }
        private string FullPath
        {
            get;
            set;
        }

        public FormMain()
        {
            Hero.SplashScreen.SetStatus("Setting up Main forum", true);
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Welp...sorry this isnt implemented...So..playing with this is just gonna annoy you with this message box. Yeah I know, sucks right...",
                "Dam it to Magranon!", MessageBoxButtons.OK);
            //    Cursor.Current = Cursors.WaitCursor;
            //    log.Parse();
            //    long tmp = log.GetCount("Start Repairing");
            //    treeView1.BeginUpdate();
            //    treeView1.Nodes.Clear();
            //    foreach (KeyValuePair<string, long> kv in log.GetAllCounts())
            //    {
            //        if (kv.Key == "Fasting")
            //        {
            //            TreeNode node = new TreeNode("Fasting");
            //            node.Nodes.Add(new TreeNode("You fasted " + kv.Value + "times."));
            //            treeView1.Nodes.Add(node);
            //        }
            //        else
            //        {
            //            TreeNode node = new TreeNode(kv.Key);
            //            node.Nodes.Add(new TreeNode("Count: " + kv.Value));
            //            treeView1.Nodes.Add(node);
            //        }
            //    }

            //    treeView1.EndUpdate();
            //    Cursor.Current = Cursors.Default;
            //    }
            //    else
            //    {
            //    }
            //}
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            initialload();
        }

        private void btnSaveClick(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Implemented yet sorry!",@"I CAN'T EVEN SAVE? !@#$%^");
        }

        private void BtnSettingsClick(object sender, EventArgs e)
        {
            FrmSettings set = new FrmSettings(this.Path);
            set.StartPosition = FormStartPosition.CenterParent;
            set.ShowDialog();
        }

        private void dateTPStart_ValueChanged(object sender, EventArgs e)
        {
            //TODO: Need to actually do this someday....Need to show results between a certain date range.
            MessageBox.Show(
                "Welp...sorry this isnt implemented...So..playing with this is just gonna annoy you with this message box. Yeah I know, sucks right...",
                "Dam it to Vynora!", MessageBoxButtons.OK);
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void dateTPEnd_ValueChanged(object sender, EventArgs e)
        {
            MessageBox.Show(
               "Welp...sorry this isnt implemented...So..playing with this is just gonna annoy you with this message box. Yeah I know, sucks right...",
               "GOD DAM IT YOU SUCK!", MessageBoxButtons.OK);

        }

        private void cbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

           

            

            

            
            this.treeView1.Nodes.Clear();
            this.FullPath =
            this.User = this.cbAccounts.SelectedItem.ToString();
            this.FullPath = null;
            

            sb.Append(this.Path);
            sb.Append(@"\players\");
            sb.Append(this.User);
            this.FullPath = sb.ToString();

            LoadAndParse();

        }
    }
}