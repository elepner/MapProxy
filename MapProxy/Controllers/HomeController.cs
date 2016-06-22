using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using MapProxy.Models.ESRI;
using MapProxy.Models.WMS;
using WebGrease.Css.Extensions;
using Layer = MapProxy.Models.ESRI.Layer;

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
            WmsService wmsServiceInfo = null;
            using (var response = webRequest.GetResponse())
            {
                var serializer = new XmlSerializer(typeof(WmsService));
                wmsServiceInfo = (WmsService)serializer.Deserialize(response.GetResponseStream());
            }

            

            if (wmsServiceInfo == null) return View("Error");

            var mapService = new MapService();
            mapService.Name = wmsServiceInfo.ServiceInfo.Name;
            mapService.Descritpion = wmsServiceInfo.ServiceInfo.Title;
            List<Models.ESRI.Layer> esriLayers = new List<Layer>();
            int count = 0;
            TraverseLayers(new[] {wmsServiceInfo.Capability.RootLayer}, esriLayers, ref count);
            mapService.Root = esriLayers.FirstOrDefault();


            ViewBag.Test = "Test String";
            return View();
        }

        private void TraverseLayers(Models.WMS.Layer[] wmsLayers, List<Models.ESRI.Layer> esriLayers, ref int count)
        {
            foreach (var wmsLayer in wmsLayers)
            {
                var esriLayer = new Models.ESRI.Layer();
                esriLayers.Add(esriLayer);
                esriLayer.Name = wmsLayer.Name;
                esriLayer.Id = count++;
                if (wmsLayer.Layers != null && wmsLayer.Layers.Length > 0)
                {
                    var subLayers = new List<Layer>(wmsLayer.Layers.Length);
                    TraverseLayers(wmsLayer.Layers, subLayers, ref count);
                    esriLayer.Sublayers = subLayers.ToArray();
                }
            }
        }
    }
}