using System;
using System.IO;

namespace MonitorNetworkWindowService.Common
{
    public static class Logger
    {
        public static void WriteErrorLog(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true))
            {
                string source = !string.IsNullOrEmpty(ex.Source) ? ex.Source.ToString() : "";
                string message = !string.IsNullOrEmpty(ex.Message) ? ex.Message.ToString() : "";

                sw.WriteLine($"{DateTime.Now.ToString()} : {source}; {message}");
                sw.Flush();
                sw.Close();
            }
        }

        public static void WriteErrorMessage(string message)
        {
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true))
            {
                sw.WriteLine($"{DateTime.Now.ToString()} : {message}");
                sw.Flush();
                sw.Close();
            }
        }
    }
}
