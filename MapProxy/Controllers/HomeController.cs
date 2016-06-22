using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MapProxy.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string WmsUrl = "";

        // GET: Home
        public ActionResult Index()
        {
            WebRequest webRequest = WebRequest.Create(WmsUrl + "?service=wms&version=1.1.1&request=GetCapabilities");
            webRequest.Method = "GET";
            using (var response = webRequest.GetResponse())
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                var stringResponse = streamReader.ReadToEnd();

            }
            
            
            ViewBag.Test = "Test String";
            return View();
        }
    }
}