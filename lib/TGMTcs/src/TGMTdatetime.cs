using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TGMTcs
{
    public class TGMTdatetime
    {
        public static DateTime GetInternetTime()
        {
            TcpClient client = new TcpClient("time.nist.gov", 13);
            using (StreamReader streamReader = new StreamReader(client.GetStream()))
            {
                string response = streamReader.ReadToEnd();
                string utcDateTimeString = response.Substring(7, 17);
                DateTime localDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                return localDateTime;
            }
        }
    }
}

    

