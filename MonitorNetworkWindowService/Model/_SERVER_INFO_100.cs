using System.Runtime.InteropServices;

namespace MonitorNetworkWindowService.Model
{
    //create a _SERVER_INFO_100 STRUCTURE
    [StructLayout(LayoutKind.Sequential)]
    public struct _SERVER_INFO_100
    {
        internal int sv100_platform_id;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string sv100_name;
    }
}
