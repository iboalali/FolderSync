using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FolderSync {
    public partial class connectForm : Form {
        public connectForm () {
            InitializeComponent();

            txtIP.Focus();
        }

        private void btnCancel_Click ( object sender, EventArgs e ) {
            this.Close();
        }

        private void btnConnect_Click ( object sender, EventArgs e ) {
            IPAddress ipAddress;
            if ( IPAddress.TryParse( txtIP.Text, out ipAddress ) ) {
                //CastUnit = new ScreenCast();
                //CastUnit.Connect( textBox1.Text );

                this.DialogResult = DialogResult.OK;

            } else {
                MessageBox.Show( "Invalid IP address, Please input a valid IP address",
                    "Error Parsing IP Address", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void txtIP_KeyPress ( object sender, KeyPressEventArgs e ) {
            if ( !char.IsDigit( e.KeyChar ) && e.KeyChar != ','
               && e.KeyChar != '.' && !char.IsControl( e.KeyChar ) ) {
                e.Handled = true;

            } else {
                if ( e.KeyChar == ',' ) {
                    e.KeyChar = '.';
                }

                if ( ( sender as TextBox ).Text.Length > 15 ) {
                    e.Handled = true;
                } else {





                }
            }
        }
    }
}
