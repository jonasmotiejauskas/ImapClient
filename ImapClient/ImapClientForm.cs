﻿using System;
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
    internal delegate void DestructAllWindows();

    public partial class ImapClientForm : Form
    {
        DestructAllWindows DAW;

        MyImapClient imapClient = new MyImapClient();

        // Window for connecting to IMAP server
        #region connectWindowElements
        Label       connectLabel = new Label() { Text = "IMAP CLIENT", Font = new Font("Arial", 32, FontStyle.Bold), Anchor = AnchorStyles.None, Width = 300, Height = 80, TextAlign = ContentAlignment.MiddleCenter };
        Button      connectButton = new Button() { Enabled = false, Text = "Connect", Width = 140, Height = 30, Visible = true, Anchor = AnchorStyles.None };
        TextBox     connectAddressInput = new TextBox() { Text = "Server Address", Anchor = AnchorStyles.None, Width = 140, Height = 60, MaxLength = 50};
        Label       connectErrorLabel = new Label() { Anchor = AnchorStyles.None, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Bold), TextAlign = ContentAlignment.TopCenter, Width = 150, Height = 100 };
        CheckBox    connectSecure = new CheckBox() { CheckState = CheckState.Checked, Anchor = AnchorStyles.None};
        Label       connectSecureLabel = new Label() { Anchor = AnchorStyles.None, Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft, Text = "Secure Connection", Height = 50 };
        #endregion connectWindowElements
        #region connectWindow
        private void ConstructConnectWindow(string errorMessage)
        {
            DAW();
            this.MinimumSize = new Size(500, 400);
            connectErrorLabel.Text = errorMessage;

            connectLabel.Enabled = true;
            connectAddressInput.Enabled = true;
            connectErrorLabel.Enabled = true;
            connectSecure.Enabled = true;
            connectSecureLabel.Enabled = true;
            if(connectAddressInput.Text != "Server Address")
            {
                connectButton.Enabled = true;
            }

            connectButton.Left = (this.ClientSize.Width - connectButton.Width) / 2;
            connectButton.Top = (this.ClientSize.Height - connectButton.Height + 60) / 2;
            connectLabel.Left = (this.ClientSize.Width - connectLabel.Width) / 2;
            connectLabel.Top = (this.ClientSize.Height - connectLabel.Height - 120) / 2;
            connectAddressInput.Left = (this.ClientSize.Width - connectAddressInput.Width) / 2;
            connectAddressInput.Top = (this.ClientSize.Height - connectAddressInput.Height) / 2;
            connectErrorLabel.Left = (this.ClientSize.Width - connectErrorLabel.Width) / 2;
            connectErrorLabel.Top = (this.ClientSize.Height - connectErrorLabel.Height + 200) / 2;
            connectSecure.Left = this.ClientSize.Width / 2 + 90;
            connectSecure.Top = this.ClientSize.Height / 2 + 25;
            connectSecureLabel.Left = (this.ClientSize.Width - connectSecureLabel.Width + 270) / 2;
            connectSecureLabel.Top = (this.ClientSize.Height - connectSecureLabel.Height + 15) / 2;

            this.Controls.Add(connectButton);
            this.Controls.Add(connectLabel);
            this.Controls.Add(connectAddressInput);
            this.Controls.Add(connectErrorLabel);
            this.Controls.Add(connectSecure);
            this.Controls.Add(connectSecureLabel);

            connectButton.Click += ConnectButton_Click;
            connectAddressInput.GotFocus += ConnectAddressInput_GotFocus;
            connectAddressInput.LostFocus += ConnectAddressInput_LostFocus;
            connectAddressInput.TextChanged += ConnectAddressInput_TextChanged;
        }

        private void ConnectAddressInput_TextChanged(object sender, EventArgs e)
        {
            if (connectAddressInput.Text == "" || connectAddressInput.Text == "Server Address")
            {
                connectButton.Enabled = false;
            }
            else
            {
                connectButton.Enabled = true;
            }
        }

        private void ConnectAddressInput_LostFocus(object sender, EventArgs e)
        {
            if (connectAddressInput.Text == "")
            {
                connectAddressInput.Text = "Server Address";
            }
        }

        private void ConnectAddressInput_GotFocus(object sender, EventArgs e)
        {
            if(connectAddressInput.Text == "Server Address")
            {
                connectAddressInput.Text = "";
            }
        }

        private void DestructConnectionWindow()
        {
            this.Controls.Remove(connectButton);
            this.Controls.Remove(connectLabel);
            this.Controls.Remove(connectAddressInput);
            this.Controls.Remove(connectErrorLabel);
            this.Controls.Remove(connectSecure);
            this.Controls.Remove(connectSecureLabel);
            connectButton.Click -= ConnectButton_Click;
            connectAddressInput.GotFocus -= ConnectAddressInput_GotFocus;
            connectAddressInput.LostFocus -= ConnectAddressInput_LostFocus;
            connectAddressInput.TextChanged -= ConnectAddressInput_TextChanged;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Enabled = false;
            }
            try
            {
                string[] addr;
                addr = connectAddressInput.Text.Split(':');
                if(addr.Length != 1)
                {
                    imapClient.Connect(addr[0], addr[1], connectSecure.Checked);
                }
                else
                {
                    imapClient.Connect(addr[0], "", connectSecure.Checked);
                } 
            }
            catch(Exception)
            {
                ConstructConnectWindow("Failed to connect to the server");
            }
        }
        #endregion

        // Window for logging in to IMAP server
        #region loginWindowElements
        Label       loginTitleLabel = new Label() { Text = "Please login", Font = new Font("Arial", 10), Anchor = AnchorStyles.None, TextAlign = ContentAlignment.MiddleLeft };
        TextBox     loginUsernameTextBox;
        TextBox     loginPasswordTextBox;
        Button      loginAuthButton;
        Label       loginErrorLabel;
        Button      loginDisconnectButton;
        #endregion loginWindowElements
        #region loginWindow

        private void constructLoginWindow(string errorMessage)
        {
            DAW();
            this.MinimumSize = new Size(500, 400);
            //loginErrorLabel.Text = errorMessage;

            loginTitleLabel.Enabled = true;
            loginUsernameTextBox.Enabled = true;
            loginPasswordTextBox.Enabled = true;
            loginAuthButton.Enabled = true;
            loginErrorLabel.Enabled = true;
            loginDisconnectButton.Enabled = true;

            //connectButton.Left = (this.ClientSize.Width - connectButton.Width) / 2;
            //connectButton.Top = (this.ClientSize.Height - connectButton.Height + 60) / 2;

            this.Controls.Add(loginTitleLabel);
            this.Controls.Add(loginUsernameTextBox);
            this.Controls.Add(loginPasswordTextBox);
            this.Controls.Add(loginAuthButton);
            this.Controls.Add(loginErrorLabel);
            this.Controls.Add(loginDisconnectButton);
        }

        #endregion loginWindow

        public ImapClientForm()
        {
            InitializeComponent();
            this.Text = "IMAP client";
            this.CenterToScreen();
            DAW += DestructConnectionWindow;
        }

        private void ImapClientForm_Shown(object sender, EventArgs e)
        {
            ConstructConnectWindow("");
        }

    }
}
