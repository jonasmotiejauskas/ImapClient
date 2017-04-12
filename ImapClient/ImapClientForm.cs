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
        // Connect Window UI elements
        Label connectLabel = new Label() { Text = "Please enter server address", Font = new Font("Arial", 10, FontStyle.Bold), Anchor = AnchorStyles.None, Width = 200, TextAlign = ContentAlignment.MiddleCenter };
        Button connectButton = new Button() { Text = "Connect", Width = 140, Height = 30, Visible = true, Anchor = AnchorStyles.None };
        TextBox connectAddressInput = new TextBox() { Anchor = AnchorStyles.None, Width = 140, Height = 60, MaxLength = 50};
        Label connectErrorLabel = new Label() { Anchor = AnchorStyles.None, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Width = 150, Height = 50 };

        public ImapClientForm()
        {
            InitializeComponent();
            Text = "Generic IMAP client";
            this.CenterToScreen();
        }

        private void ImapClientForm_Shown(object sender, EventArgs e)
        {
            constructConnectWindow("");
        }

        #region connectWindow
        private void constructConnectWindow(String errorMessage)
        {
            connectErrorLabel.Text = errorMessage;

            this.Controls.Add(connectButton);
            this.Controls.Add(connectLabel);
            this.Controls.Add(connectAddressInput);
            this.Controls.Add(connectErrorLabel);
            connectButton.Left = (this.ClientSize.Width - connectButton.Width) / 2;
            connectButton.Top = (this.ClientSize.Height - connectButton.Height + 60) / 2;
            connectLabel.Left = (this.ClientSize.Width - connectLabel.Width) / 2;
            connectLabel.Top = (this.ClientSize.Height - connectLabel.Height - 60) / 2;
            connectAddressInput.Left = (this.ClientSize.Width - connectAddressInput.Width) / 2;
            connectAddressInput.Top = (this.ClientSize.Height - connectAddressInput.Height) / 2;
            connectErrorLabel.Left = (this.ClientSize.Width - connectErrorLabel.Width) / 2;
            connectErrorLabel.Top = (this.ClientSize.Height - connectErrorLabel.Height + 120) / 2;
            connectButton.Click += ConnectButton_Click;
        }

        private void destructConnectionWindow()
        {
            this.Controls.Remove(connectButton);
            this.Controls.Remove(connectLabel);
            this.Controls.Remove(connectAddressInput);
            this.Controls.Remove(connectErrorLabel);
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            destructConnectionWindow();
        }
        #endregion


    }
}
