using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitorNetworkWindowService.Data;

namespace MonitorNetworkWebSite.Controllers
{
    public class NetworkController : Controller
    {
        // GET: Monitor
        [HttpPost]
        public ActionResult Index(string value)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<List<NetworkAddressItems>>(value);
                return Json(new { Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { Status = "Error", Message = "Exception when server tried deserialize object" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
