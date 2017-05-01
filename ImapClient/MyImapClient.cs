using System;
using System.Text;
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

        public ImapClientState ClientState { get; set; }

        public MyImapClient()
        {
            ClientState = ImapClientState.NotConnected;
        }

        // 0 - success
        public int Noop()
        {
            //if (secure)
            //{
            //    sb = new StringBuilder();
            //    buffer = new byte[2048];
            //    bytes = ssl.Read(buffer, 0, 2048);
            //    sb.Append(Encoding.ASCII.GetString(buffer));
            //    MessageBox.Show(sb.ToString());
            //}
            //else
            //{
            //    sb = new StringBuilder();
            //    buffer = new byte[2048];
            //    bytes = tcpc.GetStream().Read(buffer, 0, 2048);
            //    sb.Append(Encoding.ASCII.GetString(buffer));
            //    MessageBox.Show(sb.ToString());
            //}

            return 0;
        }

        // 0 - Connected, 1 - Already connected
        public int Connect(string address, bool sc)
        {
            secure = sc;
            port = secure? 993 : 143;
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
                finally
                {
                    ClientState = ImapClientState.NotAuthenticated;
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
