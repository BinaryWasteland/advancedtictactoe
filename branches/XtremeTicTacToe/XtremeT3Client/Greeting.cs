using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XtremeT3Client
{
    public partial class Greeting : Form
    {
        public XtremeT3Board parentForm;

        public string UserName
        {
            get { return txtName.Text; }
        }

        public string Server
        {
            get { return txtServer.Text; }
        }

        public Greeting()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Close(); 
        }

        private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                DialogResult = DialogResult.OK;
                btnGo_Click(sender, e);
            }
        }
    }
}
