using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ImapClient
{
    //enum ImapClientState {NotConnected, NotAuthenticated, Authenticated, Selected}

    class MyImapClient
    {
        System.Net.Sockets.TcpClient tcpc = null;
        System.Net.Security.SslStream ssl = null;
        int bytes = -1;
        byte[] buffer;
        StringBuilder sb = new StringBuilder();      
        int tagNumber = 0;
        bool secure;

        //public ImapClientState ClientState { get; set; }

        public MyImapClient()
        {
            secure = false;
        }

        public void Write(string command)
        {
            tagNumber++;
            byte[] dummy = UTF8Encoding.UTF8.GetBytes("TAGGED" + tagNumber + " " + command + "\r\n");
            if (secure)
            {
                ssl.Write(dummy, 0, dummy.Length);
                ssl.Flush();
            }
            else
            {
                tcpc.GetStream().Write(dummy, 0, dummy.Length);
            }
        }

        public string Read(bool connect)
        {
            if (secure)
            {
                if (connect)
                {
                    sb = new StringBuilder();
                    buffer = new byte[2048];
                    ssl.Read(buffer, 0, 2048);
                    sb.Append(UTF8Encoding.UTF8.GetString(buffer));
                }
                else
                {
                    sb = new StringBuilder();
                    buffer = new byte[2048];
                    int bytes = -1;
                    do
                    {
                        bytes = ssl.Read(buffer, 0, buffer.Length);

                        // Use Decoder class to convert from bytes to UTF8
                        // in case a character spans two buffers.
                        Decoder decoder = Encoding.UTF8.GetDecoder();
                        char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                        decoder.GetChars(buffer, 0, bytes, chars, 0);
                        sb.Append(chars);
                        // Check for EOF.
                        if (sb.ToString().IndexOf("TAGGED"+tagNumber.ToString()) != -1)
                        {
                            break;
                        }
                    } while (bytes != 0);
                }
                        Thread.Sleep(3);
            }
            else
            {
                sb = new StringBuilder();
                buffer = new byte[4000];
                bytes = tcpc.GetStream().Read(buffer, 0, 4000);
                sb.Append(UTF8Encoding.UTF8.GetString(buffer));
            }

            string res = sb.ToString();

            //MessageBox.Show(res);

            return res;
        }

        public void Connect(string address)
        {
            try
            {
                System.Net.Sockets.TcpClient tcp143 = new System.Net.Sockets.TcpClient();
                System.Net.Sockets.TcpClient tcp993 = new System.Net.Sockets.TcpClient();

                if (!tcp143.ConnectAsync(address, 143).Wait(1000))
                {
                    if (!tcp993.ConnectAsync(address, 993).Wait(1000))
                    {
                        throw new Exception("Connection timeout");
                    }
                    else
                    {
                        tcpc = tcp993;
                    }
                }
                else
                {
                    if (tcp143 != null)
                    {
                        tcpc = tcp143;
                        Read(true);
                        Write("CAPABILITIES");
                        if (Read(false).Contains("STARTTLS"))
                        {
                            Write("STARTTLS");
                        }
                        else
                        {
                            if (!tcp993.ConnectAsync(address, 993).Wait(1000))
                            {
                                throw new Exception("Connection not secure");
                            }
                            else
                            {
                                tcpc = tcp993;
                            }
                        }
                    }
                }

                ssl = new System.Net.Security.SslStream(tcpc.GetStream());
                ssl.AuthenticateAsClient(address);
                ssl.Flush();
                secure = true;
                Read(true);
            }
            catch (System.Net.Sockets.SocketException)
            {
                throw new Exception("Connection failed");
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

        public void Logout()
        {
            Write("LOGOUT");
            MessageBox.Show(Read(false));
            this.Disconnect();
        }

        // 0 - Logged in, 1 - Not logged in, 2 - Disconnected
        public int Login(string username, string password)
        {
            Write("CAPABILITY");
            if (Read(false).Contains("LOGINDISABLED"))
            {
                return 1;
            }

            string result;
            int tagNum = tagNumber;
            
            Write("LOGIN " + username + " " + password);
            result = Read(false);
            if (result.Contains("* BYE"))
            {
                Disconnect();
                return 2;
            }

            if (result.Contains("TAGGED"+tagNumber+" OK"))
            {
                Write("ENABLE UTF8=ACCEPT");
                Read(true);
                return 0;
            }
            //result = result.Substring(result.IndexOf(tagNum.ToString()), result.IndexOf("\r\n"));
            //resultSplit = result.Split(' ');
            //if (resultSplit[1] == "OK")
            //{
            //    //ClientState = ImapClientState.Authenticated;
            //    return 0;
            //}
            return 1;
        }

        public List<string> ListAllNodes()
        {
            List<string> names = new List<string>();
            Write("list \"\" *");
            string ret = Read(false);

            using (StringReader reader = new StringReader(ret))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if(line.Contains("* LIST") && line.Contains("\"/\"") && line.Contains("\\HasNoChildren"))
                    {
                        line = line.Substring(line.IndexOf("\"/\"")+5);
                        line = line.Remove(line.Length - 1);
                        names.Add(line);
                    }
                }
            }

            return names;
        }

        public List<string> CreateMailbox(string name)
        {
            Write("create "+name);
            Read(false);
            return ListAllNodes();
        }

        public List<string> DeleteMailbox(string name)
        {
            Write("delete " + name);
            Read(false);
            return ListAllNodes();
        }

        public List<string> Select(string name)
        {
            List<string> res = new List<string>();

            Write("select "+name);
            if(Read(false).Contains("OK"))
            {
                Write("UID SEARCH ALL");
                string response = Read(false).Substring(9);
                response = response.Remove(response.IndexOf("TAGGED"));

                string[] messageUIDS = response.Split(' ');
                foreach(var uid in messageUIDS)
                {
                    Write("fetch "+uid+ " body.peek[header.fields (to from subject date)]");
                    string ret = Read(false);

                    using (StringReader reader = new StringReader(ret))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("Subject"))
                            {
                                if (line.Contains("utf-8?b?") || line.Contains("UTF-8?B?"))
                                {
                                    line = line.Substring(line.IndexOf("?") + 9);
                                    line = line.Remove(line.IndexOf("?")); 
                                    byte[] data = Convert.FromBase64String(line);
                                    line = Encoding.UTF8.GetString(data);
                                    res.Add(line);
                                }
                                else
                                {
                                    if(line.Contains("utf-8?q?") || line.Contains("UTF-8?Q?"))
                                    {
                                        line = line.Substring(line.IndexOf("?") + 9);
                                        line = line.Remove(line.IndexOf("?"));
                                        Attachment attachment = Attachment.CreateAttachmentFromString("utf-8", line);
                                        res.Add(attachment.Name);
                                    }
                                    else
                                    {
                                        res.Add(line.Substring(8));
                                    }
                                }         
                            }
                        }
                    }
                }
            }
            return res;
        }

        private static string DecodeQuotedPrintables(string input, string charSet)
        {


            System.Text.Encoding enc = System.Text.Encoding.UTF7;

            try
            {
                enc = Encoding.GetEncoding(charSet);
            }
            catch
            {
                enc = new UTF8Encoding();
            }



            ////parse looking for =XX where XX is hexadecimal
            //var occurences = new Regex(@"(=[0-9A-Z]{2}){1,}", RegexOptions.Multiline);
            var occurences = new Regex("(\\=([0-9A-F][0-9A-F]))", RegexOptions.Multiline);
            var matches = occurences.Matches(input);

            foreach (Match match in matches)
            {
                try
                {
                    byte[] b = new byte[match.Groups[0].Value.Length / 3];
                    for (int i = 0; i < match.Groups[0].Value.Length / 3; i++)
                    {
                        b[i] = byte.Parse(match.Groups[0].Value.Substring(i * 3 + 1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    char[] hexChar = enc.GetChars(b);
                    input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
                }
                catch
                {; }
            }
            input = input.Replace("?=", "").Replace("=\r\n", "");

            return input;
        }

        //public string EntityToUnicode(string html)
        //{
        //    var replacements = new Dictionary<string, string>();
        //    var regex = new Regex("&[a-zA-Z1-9]{2,5}-");
        //    foreach (Match match in regex.Matches(html))
        //    {
        //        string rep = match.Value.Replace('-', ';');
        //        if (!replacements.ContainsKey(rep))
        //        {
        //            var unicode = System.Net.WebUtility.HtmlDecode(rep);
        //            if (unicode.Length == 1)
        //            {
        //                replacements.Add(match.Value, string.Concat("&#", Convert.ToInt32(unicode[0]), ";"));
        //            }
        //        }
        //    }
        //    foreach (var replacement in replacements)
        //    {
        //        html = html.Replace(replacement.Key, replacement.Value);
        //    }
        //    return html;
        //}
    }
}
