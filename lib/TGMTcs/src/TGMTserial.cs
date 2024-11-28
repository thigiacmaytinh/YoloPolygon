using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TGMTcs
{
    public class ArduinoEventArgs : EventArgs
    {
        public string message { get; private set; }

        public ArduinoEventArgs(string msg)
        {
            message = msg;
        }
    }

    class TGMTserial
    {
        public struct SerialInfo
        {
            public string portName;
            public int baudRate;
            public string description;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private SerialPort m_port;
        string m_portName = "";
        public delegate void OnMessageReceivedHandler(object sender, ArduinoEventArgs e);
        public OnMessageReceivedHandler onMessageReceived;

        public delegate void OnBoardDisconnectedHandler(object sender, ArduinoEventArgs e);
        public OnBoardDisconnectedHandler onBoardDisconnectedHandler;

        Thread m_threadCheckDisconnected;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        //if port is empty program will auto select arduino port
        public TGMTserial(string portName = "", int baudRate = 9600)
        {
            if (portName == "")
            {
                //SerialProperties serialProperties = AutoDetectPort();
                //portName = serialProperties.portName;
                //baudRate = serialProperties.baudRate;

                if (portName == null)
                {                    
                    return;
                }
            }
            try
            {
                m_port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
                // Attach a method to be called when there
                // is data waiting in the port's buffer
                m_port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

                // Begin communications
                m_port.Open();

                m_portName = portName;

                m_threadCheckDisconnected = new Thread(new ThreadStart(CheckDisconnected));
                m_threadCheckDisconnected.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        ~TGMTserial()
        {
            Disconnect();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void CheckDisconnected()
        {
            bool connected = true;
            while(connected)
            {
                Thread.Sleep(1000);
                if (m_port == null || !m_port.IsOpen)
                {
                    connected = false;
                    onBoardDisconnectedHandler?.Invoke(this, new ArduinoEventArgs("board disconnected"));
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!Connected)
                return;

            // Show all the incoming data in the port's buffer
            string message = m_port.ReadLine();
            if (message == "")
                return;

            if (message[message.Length - 1] == '\r')
            {
                message = message.Substring(0, message.Length - 1);
            }

            if(message != "")
            {
                onMessageReceived?.Invoke(this, new ArduinoEventArgs(message));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Send(string msg)
        {
            if (Connected)
                m_port.WriteLine(msg);
            //else
            //throw new Exception("Arduino not connected");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Send(char c)
        {
            if (Connected)
                m_port.Write(c.ToString());
            //else
            //throw new Exception("Arduino not connected");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<SerialInfo> ScanPorts()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            List<SerialInfo> result = new List<SerialInfo>();

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {                    

                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();
                    string pnp = item["PNPDeviceID"].ToString();

                    SerialInfo info = new SerialInfo();
                    info.portName = deviceId;
                    info.description = desc;

                    var config = item.GetRelated("Win32_SerialPortConfiguration").Cast<ManagementObject>().ToList().FirstOrDefault();
                    if (config == null)
                        continue;

                    info.baudRate = (config != null)
                                        ? int.Parse(config["BaudRate"].ToString())
                                        : int.Parse(item["MaxBaudRate"].ToString());

                    result.Add(info);
                }
            }
            catch (ManagementException e)
            {
                throw new Exception("Can not get Arduino port, please check connection");
            }

            if(result.Count == 0)
            {
                //find Arduino nano
                // Get all serial (COM)-ports you can see in the devicemanager
                searcher = new ManagementObjectSearcher("root\\cimv2",
                    "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");



                // Add all available (COM)-ports to the combobox
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    if (desc.Contains("USB-SERIAL CH340")) //arduino nano
                    {
                        string port = item["Caption"].ToString();
                        int indexOfCom = port.IndexOf("(COM");

                        SerialInfo info = new SerialInfo();
                        info.portName = port.Substring(indexOfCom + 1, port.Length - indexOfCom - 2);
                        info.baudRate = 9600; //TODO: edit later

                        result.Add(info);
                    }
                }
            }
            return result;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Connected
        {
            get
            {
                if (m_port == null)
                    return false;
                return m_port.IsOpen;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string ConnectedPort
        {
            get
            {
                return m_portName;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Disconnect()
        {
            if(Connected)
            {
                m_port.Close();
            }
        }
    }
}