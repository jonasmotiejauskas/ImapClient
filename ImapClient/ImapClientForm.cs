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
    internal delegate void DestructAllWindows();

    public partial class ImapClientForm : Form
    {
        DestructAllWindows DAW;

        MyImapClient imapClient = new MyImapClient();

        // Window for connecting to IMAP server
        #region connectWindowElements
        Label connectLabel = new Label() { Text = "IMAP CLIENT", Font = new Font("Arial", 32, FontStyle.Bold), Anchor = AnchorStyles.None, Width = 300, Height = 80, TextAlign = ContentAlignment.MiddleCenter };
        Button connectButton = new Button() { Enabled = false, Text = "Connect", Width = 140, Height = 30, Visible = true, Anchor = AnchorStyles.None };
        TextBox connectAddressInput = new TextBox() { Tag = "Server Address", Anchor = AnchorStyles.None, Width = 140, Height = 60, MaxLength = 50 };
        Label connectErrorLabel = new Label() { Anchor = AnchorStyles.None, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Bold), TextAlign = ContentAlignment.TopCenter, Width = 150, Height = 100 };
        //CheckBox    connectSecure = new CheckBox() { CheckState = CheckState.Checked, Anchor = AnchorStyles.None};
        //Label       connectSecureLabel = new Label() { Anchor = AnchorStyles.None, Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft, Text = "Secure Connection", Height = 50 };
        #endregion connectWindowElements
        #region connectWindow
        private void ConstructConnectWindow(string errorMessage)
        {
            DAW();
            this.MinimumSize = new Size(500, 400);
            connectErrorLabel.Text = errorMessage;
            connectAddressInput.Text = (string)connectAddressInput.Tag;

            connectLabel.Enabled = true;
            connectAddressInput.Enabled = true;
            connectErrorLabel.Enabled = true;
            //connectSecure.Enabled = true;
            //connectSecureLabel.Enabled = true;
            if (connectAddressInput.Text != "Server Address")
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
            //connectSecure.Left = this.ClientSize.Width / 2 + 90;
            //connectSecure.Top = this.ClientSize.Height / 2 + 25;
            //connectSecureLabel.Left = (this.ClientSize.Width - connectSecureLabel.Width + 270) / 2;
            //connectSecureLabel.Top = (this.ClientSize.Height - connectSecureLabel.Height + 15) / 2;

            this.Controls.Add(connectButton);
            this.Controls.Add(connectLabel);
            this.Controls.Add(connectAddressInput);
            this.Controls.Add(connectErrorLabel);
            //this.Controls.Add(connectSecure);
            //this.Controls.Add(connectSecureLabel);

            connectButton.Click += ConnectButton_Click;
            connectAddressInput.GotFocus += new EventHandler(OnTextChanged);
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

        private void DestructConnectionWindow()
        {
            this.Controls.Remove(connectButton);
            this.Controls.Remove(connectLabel);
            this.Controls.Remove(connectAddressInput);
            this.Controls.Remove(connectErrorLabel);
            //this.Controls.Remove(connectSecure);
            //this.Controls.Remove(connectSecureLabel);
            connectButton.Click -= ConnectButton_Click;
            connectAddressInput.GotFocus -= new EventHandler(OnTextChanged);
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
                imapClient.Connect(connectAddressInput.Text);
                ConstructLoginWindow("");
            }
            catch (Exception ex)
            {
                ConstructConnectWindow(ex.Message);
            }
        }
        #endregion

        // Window for logging in to IMAP server
        #region loginWindowElements
        Label loginTitleLabel = new Label() { Width = 150, Text = "Please login", Font = new Font("Arial", 15), Anchor = AnchorStyles.None, TextAlign = ContentAlignment.MiddleCenter };
        TextBox loginUsernameTextBox = new TextBox() { Tag = "Username", Anchor = AnchorStyles.None, Width = 140, Height = 60, MaxLength = 50 };
        TextBox loginPasswordTextBox = new TextBox() { Tag = "Password", Anchor = AnchorStyles.None, Width = 140, Height = 60, MaxLength = 50 };
        Button loginAuthButton = new Button() { Enabled = false, Text = "Login", Width = 65, Height = 30, Visible = true, Anchor = AnchorStyles.None };
        Label loginErrorLabel = new Label() { Anchor = AnchorStyles.None, ForeColor = Color.Red, Font = new Font("Arial", 8, FontStyle.Bold), TextAlign = ContentAlignment.TopCenter, Width = 150, Height = 100 };
        Button loginDisconnectButton = new Button() { Enabled = false, Text = "Quit", Width = 65, Height = 30, Visible = true, Anchor = AnchorStyles.None };
        #endregion loginWindowElements
        #region loginWindow

        private void ConstructLoginWindow(string errorMessage)
        {
            DAW();
            this.MinimumSize = new Size(500, 400);
            loginErrorLabel.Text = errorMessage;
            loginUsernameTextBox.Text = (string)loginUsernameTextBox.Tag;
            loginPasswordTextBox.Text = (string)loginPasswordTextBox.Tag;

            loginTitleLabel.Enabled = true;
            loginUsernameTextBox.Enabled = true;
            loginPasswordTextBox.Enabled = true;
            loginAuthButton.Enabled = true;
            loginErrorLabel.Enabled = true;
            loginDisconnectButton.Enabled = true;

            loginTitleLabel.Left = (this.ClientSize.Width - loginTitleLabel.Width) / 2;
            loginTitleLabel.Top = (this.ClientSize.Height - loginTitleLabel.Height - 100) / 2;
            loginUsernameTextBox.Left = (this.ClientSize.Width - loginUsernameTextBox.Width) / 2;
            loginUsernameTextBox.Top = (this.ClientSize.Height - loginUsernameTextBox.Height - 40) / 2;
            loginPasswordTextBox.Left = (this.ClientSize.Width - loginPasswordTextBox.Width) / 2;
            loginPasswordTextBox.Top = (this.ClientSize.Height - loginPasswordTextBox.Height + 15) / 2;
            loginErrorLabel.Left = (this.ClientSize.Width - loginErrorLabel.Width) / 2;
            loginErrorLabel.Top = (this.ClientSize.Height - loginErrorLabel.Height + 220) / 2;
            loginAuthButton.Left = (this.ClientSize.Width - loginUsernameTextBox.Width) / 2;
            loginAuthButton.Top = (this.ClientSize.Height - loginAuthButton.Height + 80) / 2;
            loginDisconnectButton.Left = (this.ClientSize.Width + 10) / 2;
            loginDisconnectButton.Top = (this.ClientSize.Height - loginDisconnectButton.Height + 80) / 2;

            this.Controls.Add(loginTitleLabel);
            this.Controls.Add(loginUsernameTextBox);
            this.Controls.Add(loginPasswordTextBox);
            this.Controls.Add(loginAuthButton);
            this.Controls.Add(loginErrorLabel);
            this.Controls.Add(loginDisconnectButton);

            loginUsernameTextBox.GotFocus += new EventHandler(OnTextChanged);
            loginPasswordTextBox.GotFocus += LoginPasswordTextBox_GotFocus;
            loginDisconnectButton.MouseClick += LoginDisconnectButton_MouseClick;
            loginAuthButton.MouseClick += LoginAuthButton_MouseClick;
        }

        private void LoginAuthButton_MouseClick(object sender, MouseEventArgs e)
        {
            int login = imapClient.Login(loginUsernameTextBox.Text, loginPasswordTextBox.Text);
            if (login == 0)
            {
                ConstructAuthenticatedWindow();
            }
            if (login == 1)
            {
                ConstructLoginWindow("Could not log in");
            }
            if (login == 2)
            {
                ConstructConnectWindow("Server Closed the Connection");
            }
        }

        private void LoginDisconnectButton_MouseClick(object sender, MouseEventArgs e)
        {
            imapClient.Disconnect();
            ConstructConnectWindow("Successfully disconnected");
        }

        private void DestructLoginWindow()
        {
            this.Controls.Remove(loginTitleLabel);
            this.Controls.Remove(loginUsernameTextBox);
            this.Controls.Remove(loginPasswordTextBox);
            this.Controls.Remove(loginAuthButton);
            this.Controls.Remove(loginErrorLabel);
            this.Controls.Remove(loginDisconnectButton);

            loginPasswordTextBox.PasswordChar = '\0';
            loginUsernameTextBox.GotFocus -= new EventHandler(OnTextChanged);
            loginPasswordTextBox.GotFocus -= LoginPasswordTextBox_GotFocus;
            loginDisconnectButton.MouseClick -= LoginDisconnectButton_MouseClick;
            loginAuthButton.MouseClick -= LoginAuthButton_MouseClick;
        }

        private void LoginPasswordTextBox_GotFocus(object sender, EventArgs e)
        {
            var textbox = (TextBox)sender;

            if (textbox.Text == (string)textbox.Tag)
            {
                textbox.Text = "";
            }

            textbox.PasswordChar = '*';
        }

        #endregion loginWindow

        // Window for working with mailboxes
        #region AuthenticatedWindowElements
        TextBox AuthenticatedCommandText = new TextBox();
        Button AuthenticatedCommandButton = new Button();
        ListBox AuthenticatedMailboxNames = new ListBox() { Enabled = false, Size = new Size(250, 400) };
        ListBox AuthenticatedMails = new ListBox() { Enabled = false, Size = new Size(400, 400) };
        Button AuthenticatedDeleteMailbox = new Button() { Enabled = false, Width = 65, Height = 30, Visible = true, Text = "Delete" };
        Button AuthenticatedCreateMailbox = new Button() { Enabled = false, Width = 65, Height = 30, Visible = true, Text = "Create"};
        Button AuthenticatedLogout = new Button() { Enabled = false, Width = 65, Height = 30, Visible = true, Text = "Logout" };
        #endregion AuthenticatedWIndowELements
        #region AuthenticatedWindow

        private void ConstructAuthenticatedWindow()
        {
            DAW();
            this.MinimumSize = new Size(680, 485);

            AuthenticatedCommandButton.Enabled = true;
            AuthenticatedCommandText.Enabled = true;
            AuthenticatedMailboxNames.Enabled = true;
            AuthenticatedCreateMailbox.Enabled = true;
            AuthenticatedDeleteMailbox.Enabled = true;
            AuthenticatedLogout.Enabled = true;
            AuthenticatedMails.Enabled = true;
            AuthenticatedCommandButton.Text = "Execute";

            AuthenticatedCommandButton.Top = 20;
            AuthenticatedMailboxNames.Top = 50;
            AuthenticatedCreateMailbox.Left = 5;
            AuthenticatedDeleteMailbox.Left = 80;
            AuthenticatedLogout.Left = 155;
            AuthenticatedCreateMailbox.Top = 10;
            AuthenticatedDeleteMailbox.Top = 10;
            AuthenticatedLogout.Top = 10;
            AuthenticatedMails.Top = 50;
            AuthenticatedMails.Left = 260;

            this.Controls.Add(AuthenticatedCommandText);
            this.Controls.Add(AuthenticatedCommandButton);
            this.Controls.Add(AuthenticatedMailboxNames);
            //this.Controls.Add(AuthenticatedCreateMailbox);
            //this.Controls.Add(AuthenticatedDeleteMailbox);
            this.Controls.Add(AuthenticatedLogout);
            this.Controls.Add(AuthenticatedMails);

            AuthenticatedMailboxNames.DataSource = imapClient.ListAllNodes();

            AuthenticatedCommandButton.MouseClick += AuthenticatedCommandButton_MouseClick;
            AuthenticatedLogout.MouseClick += LoginDisconnectButton_MouseClick;
            AuthenticatedCreateMailbox.MouseClick += AuthenticatedCreateMailbox_MouseClick;
            AuthenticatedDeleteMailbox.MouseClick += AuthenticatedDeleteMailbox_MouseClick;
            AuthenticatedMailboxNames.MouseDoubleClick += AuthenticatedMailboxNames_MouseDoubleClick;
        }

        private void AuthenticatedMailboxNames_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AuthenticatedMails.DataSource = imapClient.Select(AuthenticatedMailboxNames.SelectedItem.ToString());
        }

        private void AuthenticatedDeleteMailbox_MouseClick(object sender, MouseEventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete " + AuthenticatedMailboxNames.SelectedItem.ToString() + " mailbox?",
                                     "Confirm Delete",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                AuthenticatedMailboxNames.DataSource = imapClient.DeleteMailbox(AuthenticatedMailboxNames.SelectedItem.ToString());
            }
            
        }

        private void AuthenticatedCreateMailbox_MouseClick(object sender, MouseEventArgs e)
        {
            AuthenticatedMailboxNames.DataSource = imapClient.CreateMailbox(ShowDialog("Create a mailbox", "Mailbox name: "));
        }

        public static string ShowDialog(string caption, string text)
        {
            Form prompt = new Form();
            prompt.Width = 280;
            prompt.Height = 160;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 16, Top = 20, Width = 240, Text = text };
            TextBox textBox = new TextBox() { Left = 16, Top = 40, Width = 240, TabIndex = 0, TabStop = true };
            Button confirmation = new Button() { Text = "Create", Left = 16, Width = 80, Top = 88, TabIndex = 1, TabStop = true };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            return string.Format("{0}", textBox.Text);
        }

        private void DestructAuthenticatedWindow()
        {
            this.Controls.Remove(AuthenticatedMailboxNames);
            this.Controls.Remove(AuthenticatedCreateMailbox);
            this.Controls.Remove(AuthenticatedDeleteMailbox);
            this.Controls.Remove(AuthenticatedLogout);
            this.Controls.Remove(AuthenticatedMails);

            AuthenticatedCommandButton.MouseClick -= AuthenticatedCommandButton_MouseClick;
            AuthenticatedLogout.MouseClick -= LoginDisconnectButton_MouseClick;
            AuthenticatedCreateMailbox.MouseClick -= AuthenticatedCreateMailbox_MouseClick;
            AuthenticatedDeleteMailbox.MouseClick -= AuthenticatedDeleteMailbox_MouseClick;
            AuthenticatedMailboxNames.MouseDoubleClick -= AuthenticatedMailboxNames_MouseDoubleClick;
        }

        private void AuthenticatedCommandButton_MouseClick(object sender, MouseEventArgs e)
        {
            imapClient.Write(AuthenticatedCommandText.Text);
            imapClient.Read(false);
        }
        #endregion AuthenticatedWindow


        public ImapClientForm()
        {
            InitializeComponent();
            this.Text = "IMAP client";
            this.CenterToScreen();
            DAW += DestructConnectionWindow;
            DAW += DestructLoginWindow;
            DAW += DestructAuthenticatedWindow;
        }

        private void ImapClientForm_Shown(object sender, EventArgs e)
        {
            ConstructConnectWindow("");
        }

        void OnTextChanged(object sender, EventArgs e)
        {
            var textbox = (TextBox)sender;

            if (textbox.Text == (string)textbox.Tag)
            {
                textbox.Text = "";
            }
        }
    }
}
