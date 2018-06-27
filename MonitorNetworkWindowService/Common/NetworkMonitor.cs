using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using MonitorNetworkWindowService.Model;
using MonitorNetworkWindowService.Data;

namespace MonitorNetworkWindowService.Common
{
    public static class NetworkMonitor
    {
        private static List<NetworkAddressItems> networkAddressItems = null;

        private static readonly int[] CHECK_PORTS = { 1000, 3389 };

        static NetworkMonitor()
        {
            networkAddressItems = new List<NetworkAddressItems>();
        }

        [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        // The NetServerEnum API function lists all servers of the 
        // specified type that are visible in a domain.
        private static extern int NetServerEnum(
            string ServerNane, // must be null
            int dwLevel,
            ref IntPtr pBuf,
            int dwPrefMaxLen,
            out int dwEntriesRead,
            out int dwTotalEntries,
            int dwServerType,
            string domain, // null for login domain
            out int dwResumeHandle
            );

        [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        // Netapi32.dll : The NetApiBufferFree function frees 
        // the memory that the NetApiBufferAllocate function allocates.         
        private static extern int NetApiBufferFree(IntPtr pBuf);


        private static List<NetworkAddressItems> GetNetworkMachines()
        {
            const int MAX_PREFERRED_LENGTH = -1;
            int SV_TYPE_ALL = -1;
            IntPtr buffer = IntPtr.Zero;
            IntPtr tmpBuffer = IntPtr.Zero;
            int entriesRead = 0;
            int totalEntries = 0;
            int resHandle = 0;
            int sizeofINFO = Marshal.SizeOf(typeof(_SERVER_INFO_100));

            if (networkAddressItems.Count > 0)
                networkAddressItems.Clear();

            try
            {
                int ret = NetServerEnum(null, 100, ref buffer,
                    MAX_PREFERRED_LENGTH,
                    out entriesRead,
                    out totalEntries,
                    SV_TYPE_ALL,
                    null,
                    out resHandle);
                if (ret == 0)
                {
                    for (int i = 0; i < totalEntries; i++)
                    {
                        tmpBuffer = new IntPtr((int)buffer + (i * sizeofINFO));

                        _SERVER_INFO_100 svrInfo = (_SERVER_INFO_100)Marshal.PtrToStructure(tmpBuffer, typeof(_SERVER_INFO_100));

                        string computerName = svrInfo.sv100_name;
                        IPAddress[] ipAddresses = Dns.GetHostAddresses(computerName);
                        if (ipAddresses != null)
                        {
                            IPAddress ip = ipAddresses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
                            networkAddressItems.Add(new NetworkAddressItems(computerName, ip != null ? ip.ToString() : "", CHECK_PORTS));
                        }
                    }
                }
                else
                {
                    switch (ret)
                    {
                        case 6118:
                            {
                                Logger.WriteErrorMessage("Not found machines in your network");
                                break;
                            }
                        default:
                            {
                                Logger.WriteErrorMessage($"Error code : {ret}");
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex);
            }
            finally
            {
                NetApiBufferFree(buffer);
            }

            return networkAddressItems;
        }

        private static Task<bool> IsOpenPort(string ip, int port)
        {
            IPAddress[] ipAddressItems = Dns.GetHostAddresses(ip);

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    var ipAddress = ipAddressItems.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
                    var result = socket.BeginConnect(ipAddress, port, null, null);

                    bool success = result.AsyncWaitHandle.WaitOne(10000, true);
                    if (success)
                    {
                        socket.EndConnect(result);
                        return Task.FromResult(true);
                    }
                    else
                    {
                        socket.Close();
                        return Task.FromResult(false);
                    }
                }
                catch (SocketException ex)
                {
                    //Logger.WriteErrorLog(ex);
                    return Task.FromResult(false);
                }
            }
        }

        private static async void CheckOpenPorts()
        {
            try
            {
                for (int i = 0; i < networkAddressItems.Count; i++)
                {
                    var networkAddressItem = networkAddressItems[i];
                    for (int j = 0; j < networkAddressItem.PortsItems.Count; j++)
                    {
                        var portItem = networkAddressItem.PortsItems[j];
                        portItem.IsOpened = await IsOpenPort(networkAddressItem.IP, portItem.Port);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex);
            }
        }

        public static void Initialize()
        {
            GetNetworkMachines();
            CheckOpenPorts();
            SendRequest();
        }

        public static void SendRequest()
        {
            try
            {
                string json = JsonConvert.SerializeObject(networkAddressItems);
                string postData = "value=" + json;
                string response = Http.Post("http://localhost:51447/network/index", postData);
                Logger.WriteErrorMessage(response);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex);
            }
        }
    }
}
