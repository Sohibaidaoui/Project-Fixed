using Reborn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Client;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Client
{

    public partial class Form1 : Form
    {
        IntPtr mainHandle;
        public static api KeyAuthApp = new api(
    name: "Aidaouisedik28's Application",
    ownerid: "5p4oCVTHHX",
    secret: "64951a8e38872adf4acc7bddfd1adefd1beed755095aca91932772b180509e3e",
    version: "1.0");

        public Form1(IntPtr handle)
        {
            mainHandle = handle;
            InitializeComponent();

        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();
            //400, 124
        }

      
        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            KeyAuthApp.login(usr.Text, pass.Text);
            if (KeyAuthApp.response.success)
            {
                hack abc = new hack(mainHandle);
                abc.Show();
                this.Hide();

            }
            else
            {
                kxc.Text = "Cheking.... for KXC Server";
                Thread.Sleep(2000);
                kxc.Text = "Wrong User or Pass!!";
            }
        }

       

    }
}
