using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using MapCore.Models.ESRI;
using MapCore.Models.WMS;
using Microsoft.AspNetCore.Mvc;
using Layer = MapCore.Models.ESRI.Layer;

namespace MapCore.Controllers
{
    [Route("[controller]")]
    public class MapServiceController : Controller
    {
        
        // GET api/values
        [HttpGet]
        public async Task<MapService> Get()
        {
            var wmsServiceReader = new FileWMSService(@"C:\Users\edle\Desktop\Capabilities.xml");
            var wmsServiceInfo = await wmsServiceReader.GetServiceInformation();

            if (wmsServiceInfo == null) return null;

            var mapService = new MapService();
            mapService.Name = wmsServiceInfo.ServiceInfo.Name;
            mapService.Descritpion = wmsServiceInfo.ServiceInfo.Title;
            List<Layer> esriLayers = new List<Layer>();
            Dictionary<int, Layer> allLayers = new Dictionary<int, Layer>();
            int count = 0;
            TraverseLayers(new[] { wmsServiceInfo.Capability.RootLayer }, esriLayers, allLayers, ref count);
            mapService.Root = esriLayers.FirstOrDefault();
            mapService.AllLayers = allLayers;
            return mapService;
        }
        
        [HttpGet("{id}")]
        public async Task<Layer> GetLayer(int id)
        {
            var mapService = await Get();
            return mapService.AllLayers[id];
        }

        [HttpGet("export")]
        public async Task<ExportParameters> Export([FromUri]ExportParameters exportParameters)
        {
            var wmsServiceReader = new FileWMSService(@"C:\Users\edle\Desktop\Capabilities.xml");
            var wmsServiceInfo = await wmsServiceReader.GetServiceInformation();


            var requestParams = new Dictionary<string, object>();
            requestParams.Add("width", (int)exportParameters.Size.Values[0]);
            requestParams.Add("height", (int)exportParameters.Size.Values[1]);
            requestParams.Add("bbox", string.Join(",", exportParameters.Bbox.Values.Select(x => x.ToString(CultureInfo.InvariantCulture))));
            var requestUriString = wmsServiceInfo.ServiceInfo.Resource.Url;
            requestUriString += String.Join("&", requestParams.Select(x => x.Key + "=" + x.Value.ToString()));
            WebRequest webRequest = WebRequest.Create(requestUriString);
            
            webRequest.Method = "GET";
            //var result = await webRequest.GetResponseAsync();
            
            return exportParameters;
        }

        private void TraverseLayers(Models.WMS.Layer[] wmsLayers, List<Layer> esriLayers, Dictionary<int, Layer> allLayers, ref int count)
        {
            foreach (var wmsLayer in wmsLayers)
            {
                var esriLayer = new Layer();
                esriLayers.Add(esriLayer);
                esriLayer.Name = wmsLayer.Name;
                esriLayer.Id = count++;
                allLayers.Add(esriLayer.Id, esriLayer);
                if (wmsLayer.Layers != null && wmsLayer.Layers.Length > 0)
                {
                    var subLayers = new List<Layer>(wmsLayer.Layers.Length);
                    TraverseLayers(wmsLayer.Layers, subLayers, allLayers, ref count);
                    esriLayer.Sublayers = subLayers.ToArray();
                }
            }
        }
    }
}
