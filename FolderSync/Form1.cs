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
using System.Net;
using System.Net.Sockets;

namespace FolderSync {
    public partial class Form1 : Form {
        FolderBrowserDialog fbd;
        string selectedPath;
        FileSystemWatcher watcher;
        //StreamWriter sw;
        //StreamReader sr;
        FileStream fs;
        TcpClient clientSocket;
        connectForm cForm;
        string ip = null;
        string port = null;
        byte [] byteArray;

        public Form1 () {
            InitializeComponent();
            this.Icon = global::FolderSync.Properties.Resources.Icontoaster_Leox_Graphite_Sync;

            watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Created += new FileSystemEventHandler( OnFileCreated );
            watcher.Error += new ErrorEventHandler( OnError );
            watcher.IncludeSubdirectories = true;

            // To read the pending file 
            //sr = new StreamReader( "pendingFiles.bin" );

            string path = ModifyRegistry.Read( "LastPath" );
            if ( path != null ) {
                txtPath.Text = path;
                watcher.Path = path;
            }

            
            clientSocket = new System.Net.Sockets.TcpClient();
            
        }

        public void Connect ( string IP = "192.168.43.3", int Port = 9055 ) {
            clientSocket.Connect( IP, Port );
        }

        private void OnFileCreated ( object sender, FileSystemEventArgs e ) {
            while ( true ) {
                try {
                    FileInfo fi = new FileInfo( e.FullPath );
                    fs = new FileStream( e.FullPath, FileMode.Open, FileAccess.Read );

                    byte[] name = GetBytes( e.Name );
                    byte[] nameLength = BitConverter.GetBytes( name.Length );
                    byte[] fileContent = File.ReadAllBytes( e.FullPath );

                    IEnumerable<byte> rv = nameLength.Concat( name ).Concat( fileContent );

                    byteArray = rv.ToArray();

                    try {
                        SendData( byteArray );

                    } catch ( Exception ) {


                        throw;
                    }
                    break;

                } catch ( IOException ) {
                    continue;
                }
            }
        }

        /// <summary>
        /// Gets the byte array of the string
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>The byte array</returns>
        static byte[] GetBytes ( string str ) {
            byte[] bytes = new byte[str.Length * sizeof( char )];
            System.Buffer.BlockCopy( str.ToCharArray(), 0, bytes, 0, bytes.Length );
            return bytes;
        }

        /// <summary>
        /// This method is called when the FileSystemWatcher detects an error.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnError ( object source, ErrorEventArgs e ) {
            //  Show that an error has been detected.
            Console.WriteLine( "The FileSystemWatcher has detected an error" );
            //  Give more information if the error is due to an internal buffer overflow. 
            if ( e.GetException().GetType() == typeof( InternalBufferOverflowException ) ) {
                //  This can happen if Windows is reporting many file system events quickly  
                //  and internal buffer of the  FileSystemWatcher is not large enough to handle this 
                //  rate of events. The InternalBufferOverflowException error informs the application 
                //  that some of the file system events are being lost.
                Console.WriteLine( ( "The file system watcher experienced an internal buffer overflow: " + e.GetException().Message ) );
            }
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
                watcher.Path = selectedPath;
                ModifyRegistry.Write( "LastPath", selectedPath );
            }



        }

        private void btnConnect_Click ( object sender, EventArgs e ) {
            if ( btnConnect.Text == "Connect" ) {

                cForm = new connectForm();

                if ( cForm.ShowDialog( this ) == DialogResult.OK ) {
                    

                    ip = cForm.txtIP.Text;
                    port = cForm.txtPort.Text;

                    while ( true ) {
                        try {
                            // Connect to the client
                            Connect( ip );

                            //  Begin watching.
                            watcher.EnableRaisingEvents = true;

                            btnConnect.Text = "Disconnect";
                            break;

                        } catch ( SocketException ) {
                            DialogResult r = MessageBox.Show( "Could not connect to " + ip + ":" + port, "Connection Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error );
                            if ( r == DialogResult.Retry ) {
                                continue;

                            }
                            break;
                            
                        }
                    }

                    ModifyRegistry.Write( "LastIP", ip );
                    ModifyRegistry.Write( "LastPort", port );

                    
                } 
            } else {
                btnConnect.Text = "Connect";
                watcher.EnableRaisingEvents = false;
                //clientSocket.Close();

            }
        }

        public void SendData ( byte[] d ) {
            NetworkStream serverStream = clientSocket.GetStream();

            //Type
            byte[] buffer = new byte[4];
            buffer = BitConverter.GetBytes( 25 );//bitmap type
            serverStream.Write( buffer, 0, 4 );
            // this.networkStream.Flush();


            //CommandID
            byte[] buffer2 = new byte[8];
            buffer2 = BitConverter.GetBytes( ( long ) 1 );
            serverStream.Write( buffer2, 0, 8 );
            //this.networkStream.Flush();


            //from id
            //byte [] senderIPBuffer = Encoding.ASCII.GetBytes(cmd.SenderIP.ToString());
            byte[] buffer3 = new byte[8];
            buffer3 = BitConverter.GetBytes( ( long ) 100 );
            serverStream.Write( buffer3, 0, 8 );


            //target
            byte[] buffer4 = new byte[8];
            buffer4 = BitConverter.GetBytes( ( long ) 1 );//server
            serverStream.Write( buffer4, 0, 8 );


            //data length
            byte[] buffer5 = new byte[4];
            buffer5 = BitConverter.GetBytes( d.Length );
            serverStream.Write( buffer5, 0, 4 );

            //actual data
            serverStream.Write( d, 0, d.Length );
            serverStream.Flush();

        }


    }
}
