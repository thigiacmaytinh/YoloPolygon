using System;
using System.IO;
using System.Net;
#if !NET_35
using System.Net.Http;
#endif
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace TGMTcs
{
    public class TGMTonline
    {
        bool m_isWaitingMessage = true;
        public static event EventHandler<TGMTonlineArgs> onListenMessage;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        ~TGMTonline()
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool IsInternetAvailable(string url = "http://google.com")
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead(url))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string SendGETrequestAsync(string request)
        {
            return "";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string SendGETrequest(string request)
        {

#if NET_35
            // Create a request for the URL. 		
            WebRequest httpRequest = WebRequest.Create(request);
            // If required by the server, set the credentials.
            httpRequest.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
            // Display the status.
            string StatusDescription = response.StatusDescription;
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;

#else
            try
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    WebRequest wrGETURL;
                    wrGETURL = WebRequest.Create(request);

                    WebProxy myProxy = new WebProxy("myproxy", 80);
                    myProxy.BypassProxyOnLocal = true;

                    wrGETURL.Proxy = WebProxy.GetDefaultProxy();

                    Stream objStream = wrGETURL.GetResponse().GetResponseStream();
                    StreamReader objReader = new StreamReader(objStream);

                    string respond = objReader.ReadToEnd();
                    return respond;
                }
            }
            catch(Exception ex)
            {
                return "";
            }            
#endif
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

#if NET_45 || NET_46 || NET_47
        public static async void SendPOSTrequest(string url, Dictionary<string, string> values, Action<int, string> callback)
        {
            try
            {
                var items = values.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
                var content = new StringContent(String.Join("&", items), null, "application/x-www-form-urlencoded");


                HttpClient client = new HttpClient();
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var result = await client.PostAsync(url, content);

                var responseString = await result.Content.ReadAsStringAsync();
                if (callback != null)
                {
                    callback((int)result.StatusCode, responseString);
                }
            }
            catch(Exception ex)
            {
                if (callback != null)
                {
                    callback(399, "{\"Error\" : \"Không kết nối được server: " + ex.Message + "\"}");
                }
            }
            
        }

#endif

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        Socket m_socket;
        bool m_connectSuccess = false;
        public bool SetTargetSocket(string ip, int port)
        {
            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAdd = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAdd, port);
                m_socket.Connect(remoteEP);

                m_connectSuccess = true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                m_connectSuccess = false;
            }

            return m_connectSuccess;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string SendMessageToSocket(string msg, bool waitRespondMessage = false)
        {
            if (!m_connectSuccess)
                return "Error: Connection failed";

            try
            {
                //Start sending stuf..
                byte[] byData = Encoding.ASCII.GetBytes(msg);
                m_socket.Send(byData);
                if (waitRespondMessage)
                {
                    byte[] buffer = new byte[1024];
                    int iRx = m_socket.Receive(buffer);
                    char[] chars = new char[iRx];

                    Decoder d = Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                    string receive = new string(chars);

                    return receive;
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void StartListening()
        {
            if (!m_connectSuccess)
                return;// "Error: Connection failed";
            Decoder decoder = Encoding.ASCII.GetDecoder();
            try
            {
                //begin connection
                byte[] byData = Encoding.ASCII.GetBytes("O");
                m_socket.Send(byData);
                while (m_isWaitingMessage)
                {
                    byte[] buffer = new byte[1024];
                    int iRx = m_socket.Receive(buffer);
                    char[] chars = new char[iRx];


                    int charLen = decoder.GetChars(buffer, 0, iRx, chars, 0);
                    string receive = new string(chars);

                    onListenMessage?.Invoke(null, new TGMTonlineArgs(receive));
                    Console.WriteLine(receive);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void StopListening()
        {
            m_isWaitingMessage = false;
            if (m_socket.IsBound)
            {
                m_socket.Disconnect(false);
            }
            m_socket.Close();
        }


        public class TGMTonlineArgs : EventArgs
        {
            public string message;

            public TGMTonlineArgs(string msg)
            {
                message = msg;
            }
        }
    }
}