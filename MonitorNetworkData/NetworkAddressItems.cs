using System.Collections.Generic;
using System.Linq;

namespace MonitorNetworkWindowService.Data
{
    public class NetworkAddressItems
    {
        public NetworkAddressItems()
        {
            this.PortsItems = new List<NetworkPortItem>();
        }

        public NetworkAddressItems(string name, string ip, int[] portsArray)
            : this()
        {
            this.ComputerName = name;
            this.IP = ip;
            this.PortsItems = portsArray.Select(c => new NetworkPortItem(c)).ToList();
        }

        public string ComputerName { get; set; }
        public string IP { get; set; }
        public List<NetworkPortItem> PortsItems { get; set; }
    }
}
