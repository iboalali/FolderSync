using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderSync {
    public partial class Form1 : Form {
        FolderBrowserDialog fbd;
        string selectedPath;
        FileSystemWatcher watcher;

        public Form1 () {
            InitializeComponent();
            this.Icon = global::FolderSync.Properties.Resources.Icontoaster_Leox_Graphite_Sync;
            watcher = new FileSystemWatcher();
        }

        private void btnExit_Click ( object sender, EventArgs e ) {
            Environment.Exit( Environment.ExitCode );
        }

        private void btnBrowse_Click ( object sender, EventArgs e ) {
            fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.Description = "Select a folder to sync to another PC";

            if ( DialogResult.OK == fbd.ShowDialog() ) {
                selectedPath = fbd.SelectedPath;
                txtPath.Text = selectedPath;
            }



        }

    }
}
