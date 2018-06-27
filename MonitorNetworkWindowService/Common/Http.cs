using System.IO;
using System.Net;
using System.Text;


namespace MonitorNetworkWindowService.Common
{
    public static class Http
    {
        public static string Post(string url, string postData = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (postData != null)
            {
                var data = Encoding.ASCII.GetBytes(postData);
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var responseString = new StreamReader(response.GetResponseStream()))
                {
                    return responseString.ReadToEnd();
                }
            }
        }
    }
}
