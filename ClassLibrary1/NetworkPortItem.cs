namespace MonitorNetworkWindowService.Data
{
    public class NetworkPortItem
    {
        public NetworkPortItem(int port)
        {
            this.Port = port;
        }

        public int Port { get; set; }
        public bool IsOpened { get; set; }
    }
}
