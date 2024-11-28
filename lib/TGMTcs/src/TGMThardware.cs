using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Management;
using System.Net.NetworkInformation;

namespace TGMTcs
{
    public class TGMThardware
    {
        public static string GetCpuId()
        {
            //must add reference to System.Management
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
            mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                if (mo["ProcessorID"] != null)
                    id = mo["ProcessorID"].ToString();
            }
            return id;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetMainboardId()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string serial = "";
            foreach (ManagementObject mo in moc)
            {
                serial = (string)mo["SerialNumber"];
            }
            if (serial == "To be filled by O.E.M.")
                return "";
            return serial;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetDiskId()
        {
            TGMTregistry reg = new TGMTregistry();
            reg.Init("Microsoft");

            string saveModel = reg.ReadString("model");
            if (saveModel != "")
                return saveModel;

            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            if (searcher == null)
                return "";

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                //string model = wmi_HD["Model"].ToString();
                //string interfaceType = wmi_HD["InterfaceType"].ToString();
                //string caption = wmi_HD["Caption"].ToString();
                string serialNo = wmi_HD.GetPropertyValue("SerialNumber").ToString();//get the serailNumber of diskdrive
                if (serialNo != null && serialNo != "")
                {
                    reg.SaveValue("model", serialNo);
                    return serialNo;
                }
            }
            return "";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetPartitionId(string partition)
        {
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + partition + "\"");
            dsk.Get();
            string diskid = dsk["VolumeSerialNumber"].ToString();
            return diskid;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetMacAddress()
        {
            TGMTregistry reg = new TGMTregistry();
            reg.Init("Microsoft");

            string saveModel = reg.ReadString("model");
            if (saveModel != "")
                return saveModel;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet)
                {
                    string mac = nic.GetPhysicalAddress().ToString();
                    reg.SaveValue("model", mac);
                    return mac;
                }
            }

            return "";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetUDID()
        {
            string udid = GetCpuId();
            string diskID = GetDiskId();
            if(diskID != "")
                udid += diskID;
            else
            {
                string mac = GetMacAddress();
                udid += mac;
            }



            udid.Replace(" ", "");

            udid = TGMTutil.ConvertToAlphanumeric(udid);
            udid = udid.ToLower();
            return udid;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string GetPCname()
        {
            return Environment.MachineName;
        }
    }
}
