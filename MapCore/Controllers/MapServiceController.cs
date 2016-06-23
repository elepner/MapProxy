using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            int count = 0;
            TraverseLayers(new[] { wmsServiceInfo.Capability.RootLayer }, esriLayers, ref count);
            mapService.Root = esriLayers.FirstOrDefault();

            return mapService;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return id.ToString();
        }

        private void TraverseLayers(Models.WMS.Layer[] wmsLayers, List<Layer> esriLayers, ref int count)
        {
            foreach (var wmsLayer in wmsLayers)
            {
                var esriLayer = new Layer();
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
