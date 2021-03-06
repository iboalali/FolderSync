﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FolderSync {
    public partial class connectForm : Form {
        string ip;
        string port;
        public connectForm () {
            InitializeComponent();

            string lastIP = ModifyRegistry.Read( "LastIP" );
            if ( lastIP != null ) {
                this.ip = lastIP;
            }

            string lastPort = ModifyRegistry.Read( "LastPort" );
            if ( lastPort != null ) {
                this.port = lastPort;
            }

            txtIP.Text = ip;
            txtPort.Text = port;
                
            //System.DirectoryServices.DirectoryEntry winNtDirectoryEntries = new System.DirectoryServices.DirectoryEntry( "WinNT:" );
            //List<String> computerNames = ( from DirectoryEntry availDomains in winNtDirectoryEntries.Children
            //                               from DirectoryEntry pcNameEntry in availDomains.Children
            //                               where pcNameEntry.SchemaClassName.ToLower().Contains( "computer" )
            //                               select pcNameEntry.Name ).ToList();
            //
            //DirectoryContext mycontext = new DirectoryContext( DirectoryContextType.Domain, "project.local" );
            //DomainController dc = DomainController.FindOne( mycontext );
            //IPAddress DCIPAdress = IPAddress.Parse( dc.IPAddress );
            //
            //textBox1.Text = DCIPAdress.ToString();
            //
            //foreach ( var item in computerNames ) {
            //    listView1.Items.Add( item );
            //}
        }

        private void btnCancel_Click ( object sender, EventArgs e ) {
            this.Close();
        }

        private void btnConnect_Click ( object sender, EventArgs e ) {
            IPAddress ipAddress;
            
            if ( IPAddress.TryParse( txtIP.Text, out ipAddress ) ) {
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
