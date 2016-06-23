using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MapCore.Models.WMS
{
    public class WebWMSService : StreamReadingWMSService
    {
        private readonly string _url;

        public WebWMSService(string url)
        {
            _url = url;
        }

        
        protected override async Task<Stream> GetWMSCapabilitiesStream()
        {
            WebRequest webRequest = WebRequest.Create(_url + "?service=wms&version=1.1.1&request=GetCapabilities");
            webRequest.Method = "GET";
            var result = await webRequest.GetResponseAsync();
            return result.GetResponseStream();
        }
    }
}
