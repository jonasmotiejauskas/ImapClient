using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImapClient
{
    class MyImapClient
    {
        System.Net.Sockets.TcpClient tcpc = null;
        System.Net.Security.SslStream ssl = null;
        int bytes = -1;
        byte[] buffer;
        StringBuilder sb = new StringBuilder();
        int port;

        public void Connect(string address, bool secure)
        {
            port = secure? 993 : 143;
            try
            {
                tcpc = new System.Net.Sockets.TcpClient();
                if (!tcpc.ConnectAsync(address, port).Wait(5000))
                {
                    throw new Exception("Connection timed out");
                }

                ssl = new System.Net.Security.SslStream(tcpc.GetStream());
                ssl.AuthenticateAsClient(address);
                ssl.Flush();

                sb = new StringBuilder();
                buffer = new byte[2048];
                bytes = ssl.Read(buffer, 0, 2048);
                sb.Append(Encoding.ASCII.GetString(buffer));
                MessageBox.Show(sb.ToString());
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
        }
    }
}
