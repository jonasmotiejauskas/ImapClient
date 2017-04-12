using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImapClient
{
    public partial class ImapClientForm : Form
    {
        Label connectLabel = new Label() { Text = "Enter server address" };
        Button connectButton = new Button() { Text = "Connect", Width = 125, Height = 30, Visible = true };

        public ImapClientForm()
        {
            InitializeComponent();
            Text = "Generic IMAP client";
            this.CenterToScreen();
        }

        private void ImapClientForm_Shown(object sender, EventArgs e)
        {
            this.Controls.Add(connectButton);
            connectButton.Left = (this.ClientSize.Width - connectButton.Width) / 2;
            connectButton.Top = (this.ClientSize.Height - connectButton.Height) / 2;
        }
    }
}
