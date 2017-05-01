using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ImapClient
{
    enum ImapClientState {NotConnected, NotAuthenticated, Authenticated, Selected, Logout}

    class MyImapClient
    {
        System.Net.Sockets.TcpClient tcpc = null;
        System.Net.Security.SslStream ssl = null;
        int bytes = -1;
        byte[] buffer;
        StringBuilder sb = new StringBuilder();
        int port = 993;
        bool secure = false;
        int tagNumber = 0;

        public ImapClientState ClientState { get; set; }

        public MyImapClient()
        {
            ClientState = ImapClientState.NotConnected;
        }

        private void Write(string command)
        {
            byte[] dummy = Encoding.ASCII.GetBytes(tagNumber + " " + command + " \r\n");
            if (secure)
            {
                ssl.Write(dummy, 0, dummy.Length);
                ssl.Flush();
            }
            else
            {
                tcpc.GetStream().Write(dummy, 0, dummy.Length);
            }
            tagNumber++;
        }

        private string Read()
        {
            if (secure)
            {
                sb = new StringBuilder();
                buffer = new byte[2048];
                bytes = ssl.Read(buffer, 0, 2048);
                sb.Append(Encoding.ASCII.GetString(buffer));
            }
            else
            {
                sb = new StringBuilder();
                buffer = new byte[2048];
                bytes = tcpc.GetStream().Read(buffer, 0, 2048);
                sb.Append(Encoding.ASCII.GetString(buffer));
            }
            return sb.ToString();
        }

        // 0 - Connected, 1 - Already connected
        public int Connect(string address, string portNumber, bool sc)
        {
            secure = sc;
            if(portNumber == "")
            {
                port = secure ? 993 : 143;
            }
            else
            {
                port = Int32.Parse(portNumber);
            }

            if(ClientState == ImapClientState.NotConnected)
            {
                try
                {
                    tcpc = new System.Net.Sockets.TcpClient();
                    if (!tcpc.ConnectAsync(address, port).Wait(5000))
                    {
                        throw new Exception("Connection timed out");
                    }

                    if (secure)
                    {
                        ssl = new System.Net.Security.SslStream(tcpc.GetStream());
                        ssl.AuthenticateAsClient(address);
                        ssl.Flush();
                    }
                    Read();
                    if (!secure)
                    {
                        Write("CAPABILITY");
                        if (Read().IndexOf("STARTTLS") != 0)
                        {
                            Write("STARTTLS");
                            Read();
                            ssl = new System.Net.Security.SslStream(tcpc.GetStream());
                            ssl.AuthenticateAsClient(address);
                            ssl.Flush();
                            secure = true;
                        }
                    }
                    Write("CAPABILITY");
                    MessageBox.Show(Read());


                    ClientState = ImapClientState.NotAuthenticated;
                    return 0;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {   
                    throw ex;
                }
            }
            else
            {
                return 1;
            }
        }

        public void Disconnect()
        {
            if (ssl != null)
            {
                ssl.Close();
                ssl.Dispose();
            }
            if (tcpc != null)
            {
                tcpc.Close();
            }

            ClientState = ImapClientState.NotConnected;
        }
    }
}
