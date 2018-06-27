using MonitorNetworkWindowService.Common;
using System;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Timers;

namespace MonitorNetworkWindowService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.timer = new Timer();
            this.timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
            this.timer.Elapsed += Timer_Tick;
            this.timer.Enabled = true;
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }

            NetworkMonitor.Initialize();
        }

        protected override void OnStop()
        {
            this.timer.Enabled = false;
        }
    }
}
