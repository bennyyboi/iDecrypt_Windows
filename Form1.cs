using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;


namespace QuickDecrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Launches a exe (executable) and sends it some arguments (simple!)
        /// </summary>
    public void loadexe(string launchFile , string launchArgs)
    {
         Process proc = new Process(); //define new process
         proc.StartInfo.FileName = launchFile;
         proc.StartInfo.Arguments = " " + launchArgs;
         proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
         Application.DoEvents();
         proc.Start();
         while (proc.HasExited)
         {
             Application.DoEvents();
             int i = 50000;
             Application.DoEvents();
         }
         proc.WaitForExit();

    }

        private void Form1_Load(object sender, EventArgs e)
        {
            idver.Text = "v" + Application.ProductVersion.ToString();
           

            //setup options for the openfile dialog
            opendmg.Filter = "*.dmg (Mac OS Disk Image)|*.dmg";
            opendmg.FileName = ""; //leave this blank
            opendmg.Title = "Browse for a DMG (Mac OS Disk Image)"; //change this to whatever you like :)

            //setup the main textbox
            dmgfile.Text = "No DMG selected yet?";
            dmgsize.Text = "0 bytes";

            //setup options for outputFld dialog
            outputFld.ShowNewFolderButton = true;
            outputFld.Description = "Select a destination to save the decrypted dmg to...";

         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //show the open dialog and pre-define the output...
            outputFld.ShowDialog();
            outputdmg1.Text = outputFld.SelectedPath.ToString() + "\\" + Path.GetFileNameWithoutExtension(opendmg.FileName.ToString()) + "_decrypted.dmg";

        }

        private void buttonx_Click(object sender, EventArgs e)
        {
            opendmg.ShowDialog();
            //show the dmg selected!
            
            //show the file size of the DMG selected...
            if (opendmg.FileName.ToString() == "")
            {
                dmgsize.Text = "0 bytes";
                dmgfile.Text = "No DMG selected yet?";
            }
            else
            {

                dmgfile.Text = opendmg.FileName.ToString();
                MessageBox.Show("This should be the rootfs DMG, and not the restore/update ramdisks. You can tell the difference by the file size of the DMG's the biggest one is the rootfs and other two are the ramdisks, ramdisks can only be decrypted using xPwn (for Mac)/xPwnTool (for Windows).");
                FileInfo fi = new FileInfo(opendmg.FileName.ToString());
                long len = fi.Length;
                dmgsize.Text = len + " bytes";

                //read the key from the xml file and check it against the rootfs dmg filename.
                XmlDocument doc = new XmlDocument();
                doc.Load("iDeviceKeys.xml");
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("DeviceKey");

                foreach (XmlNode node in nodes)
                {
                    string rootfs = node["rootfs"].InnerText;
                    string dmg_key = node["key"].InnerText;
                    
                    //get the key now...
                    if (Path.GetFileName(opendmg.FileName.ToString()) == rootfs)
                    {
                        dmgkey.Text = dmg_key;
                    }
                }
            }
        }

        private void decryptnow_Click(object sender, EventArgs e)
        {
            //we must check to see if all fields are empty or not if so then display a alert box.
            if((!string.IsNullOrEmpty(dmgfile.Text)) && (!string.IsNullOrEmpty(outputdmg1.Text)) && (!string.IsNullOrEmpty(dmgkey.Text)))
            {
                decryptnow.Text = "Decrypting...";
                decryptnow.Enabled = false;
                buttonx.Enabled = false;
                button1.Enabled = false;
                dmgkey.ReadOnly = true;
                outputdmg1.ReadOnly = true;
                decryptStatus.Visible = true;
                decryptStatus.Text = "Decrypting '" + Path.GetFileName(opendmg.FileName.ToString()) + "'...";
                this.Cursor = Cursors.WaitCursor;
                loadexe("vfdecrypt.exe", " -i " + dmgfile.Text.ToString() + " -o " + outputFld.SelectedPath.ToString() + "\\" + Path.GetFileNameWithoutExtension(opendmg.FileName.ToString()) + "_decrypted.dmg" + " -k " + dmgkey.Text.ToString());
                MessageBox.Show("Decryption is done, you can find the decrypted rootfs dmg at '" + outputdmg1.Text.ToString() + "' enjoy!");
                decryptStatus.Visible = false;
                dmgkey.ReadOnly = false;
                outputdmg1.ReadOnly = false;
                decryptnow.Enabled = true;
                buttonx.Enabled = true;
                button1.Enabled = true;
                this.Cursor = Cursors.Default;
                decryptnow.Text = "Decrypt DMG";
                
            }
            else
            {
                MessageBox.Show("You have have left everything empty, so you can't decrypt a empty file can you?!?"); 
            }

        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

      
        private void updateBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void checkForUpdateToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Start the updater application, this will check against a file on my server for updates.
            Process.Start("updater.exe");   
        }

        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //exit the application
            this.Close();
        }

      

    }
}
