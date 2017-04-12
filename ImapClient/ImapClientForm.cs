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
    internal delegate void DestructAllForms();

    public partial class ImapClientForm : Form
    {
        DestructAllForms DAF;

        MyImapClient imapClient = new MyImapClient();

        // Window for connecting to IMAP server
        #region connectWindowElements
        Label connectLabel = new Label() { Text = "IMAP CLIENT", Font = new Font("Arial", 32, FontStyle.Bold), Anchor = AnchorStyles.None, Width = 300, Height = 80, TextAlign = ContentAlignment.MiddleCenter };
        Button connectButton = new Button() { Text = "Connect", Width = 140, Height = 30, Visible = true, Anchor = AnchorStyles.None };
        TextBox connectAddressInput = new TextBox() { Anchor = AnchorStyles.None, Width = 140, Height = 60, MaxLength = 50};
        Label connectErrorLabel = new Label() { Anchor = AnchorStyles.None, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Bold), TextAlign = ContentAlignment.TopCenter, Width = 150, Height = 100 };
        #endregion connectWindowElements
        #region connectWindow
        private void ConstructConnectWindow(string errorMessage)
        {
            DAF();
            this.MinimumSize = new Size(350, 350);
            connectErrorLabel.Text = errorMessage;

            connectButton.Enabled = true;
            connectLabel.Enabled = true;
            connectAddressInput.Enabled = true;
            connectErrorLabel.Enabled = true;

            this.Controls.Add(connectButton);
            this.Controls.Add(connectLabel);
            this.Controls.Add(connectAddressInput);
            this.Controls.Add(connectErrorLabel);
            connectButton.Left = (this.ClientSize.Width - connectButton.Width) / 2;
            connectButton.Top = (this.ClientSize.Height - connectButton.Height + 60) / 2;
            connectLabel.Left = (this.ClientSize.Width - connectLabel.Width) / 2;
            connectLabel.Top = (this.ClientSize.Height - connectLabel.Height - 120) / 2;
            connectAddressInput.Left = (this.ClientSize.Width - connectAddressInput.Width) / 2;
            connectAddressInput.Top = (this.ClientSize.Height - connectAddressInput.Height) / 2;
            connectErrorLabel.Left = (this.ClientSize.Width - connectErrorLabel.Width) / 2;
            connectErrorLabel.Top = (this.ClientSize.Height - connectErrorLabel.Height + 200) / 2;
            connectButton.Click += ConnectButton_Click;
        }

        private void DestructConnectionWindow()
        {
            this.Controls.Remove(connectButton);
            this.Controls.Remove(connectLabel);
            this.Controls.Remove(connectAddressInput);
            this.Controls.Remove(connectErrorLabel);
            connectButton.Click -= ConnectButton_Click;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Enabled = false;
            }
            try
            {
                imapClient.Connect(connectAddressInput.Text);
            }
            catch(Exception)
            {
                ConstructConnectWindow("Failed to connect to the server");
            }
        }
        #endregion

        public ImapClientForm()
        {
            InitializeComponent();
            this.Text = "IMAP client";
            this.CenterToScreen();
            DAF += DestructConnectionWindow;
        }

        private void ImapClientForm_Shown(object sender, EventArgs e)
        {
            ConstructConnectWindow("");
        }

    }
}
