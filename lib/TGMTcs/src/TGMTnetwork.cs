using System;
using System.IO;
using System.Net;
#if !NET_35
using System.Net.Http;
#endif
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace TGMTcs
{
    public class TGMTnetwork
    {

        static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string LocalIPAddress()
        {
            string localComputerName = Dns.GetHostName();
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress item in localIPs)
            {
                string ip = item.ToString();
                if (!ip.Contains("192.168"))
                    continue;
                if (IsLocalIpAddress(ip))
                {
                    return item.ToString();
                }
            }
            return "";
        }
    }
}