using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using MapCore.Models.ESRI;
using MapCore.Models.WMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Chunks;
using Layer = MapCore.Models.ESRI.Layer;

namespace MapCore.Controllers
{
    [Route("arcgis/rest/services/test/[controller]")]
    public class MapServerController : Controller
    {
        
        // GET api/values
        [HttpGet]
        public async Task<MapService> Get()
        {
            var wmsServiceReader = new FileWMSService(@"C:\Users\edle\Desktop\Capabilities.xml");
            var wmsServiceInfo = await wmsServiceReader.GetServiceInformation();

            if (wmsServiceInfo == null) return null;

            var esriMapServiceDefinition = new MapService();
            esriMapServiceDefinition.Name = wmsServiceInfo.ServiceInfo.Name;
            esriMapServiceDefinition.Descritpion = wmsServiceInfo.ServiceInfo.Title;
            List<Layer> esriLayers = new List<Layer>();
            Dictionary<int, Layer> allLayers = new Dictionary<int, Layer>();
            int count = -1;
            TraverseLayers(new[] { wmsServiceInfo.Capability.RootLayer }, esriLayers, allLayers, -1, ref count);
            esriMapServiceDefinition.Root = esriLayers.FirstOrDefault();
            esriMapServiceDefinition.AllLayers = allLayers;
            esriMapServiceDefinition.SpatialReference = new SpatialReference(32632);
            var extent = new Extent();
            var bbox = wmsServiceInfo.Capability.RootLayer.BoundingBox;
            extent.SpatialReference = new SpatialReference(bbox.Wkid);
            extent.Xmax = bbox.XMax;
            extent.Ymax = bbox.YMax;
            extent.Xmin = bbox.XMin;
            extent.Ymin = bbox.YMin;

            esriMapServiceDefinition.FullExtent = extent;
            esriMapServiceDefinition.InitialExtent = extent;
            return esriMapServiceDefinition;
        }
        
        [HttpGet("{id}")]
        public async Task<Layer> GetLayer(int id)
        {
            var mapService = await Get();
            return mapService.AllLayers[id];
        }

        [HttpGet("export")]
        public async Task<FileStreamResult> Export([FromUri]ExportParameters exportParameters)
        {
            var wmsServiceReader = new FileWMSService(@"C:\Users\edle\Desktop\Capabilities.xml");
            var wmsServiceInfo = await wmsServiceReader.GetServiceInformation();
            var esriServiceInfo = await Get();

            var requestParams = new Dictionary<string, object>
            {
                {"width", (int) exportParameters.Size.Values[0]},
                {"height", (int) exportParameters.Size.Values[1]},
                {
                    "bbox",
                    string.Join(",", exportParameters.Bbox.Values.Select(x => x.ToString(CultureInfo.InvariantCulture)))
                },
                {"transparent", exportParameters.Transparent},
                {"version", "1.1.1"},
                {"request", "GetMap"},
                {"service", "WMS"},
                {"SRS", "EPSG:32632"},
                {"format", "image/png"}
            };

            var layerNames = new List<string>();
            foreach (var layerIdDouble in exportParameters.Layers.Values)
            {
                int layerID = (int) layerIdDouble;
                Layer layer;
                if (esriServiceInfo.AllLayers.TryGetValue(layerID, out layer))
                {
                    layerNames.Add(layer.Name);
                }
            }

            requestParams.Add("layers", string.Join(",", layerNames));

            var requestUriString = wmsServiceInfo.ServiceInfo.Resource.Url + "?";
            requestUriString += string.Join("&", requestParams.Select(x => x.Key + "=" + x.Value.ToString()));


            WebRequest webRequest = WebRequest.Create(requestUriString);
                        
            webRequest.Method = "GET";
                        
            var wmsResult = await webRequest.GetResponseAsync();
            var stream = wmsResult.GetResponseStream();
            
            return new FileStreamResult(stream, "image/png");

        }

        [HttpGet("info")]
        public ServerInfo ServerInfo()
        {
            return new ServerInfo();
        }

        private void TraverseLayers(Models.WMS.Layer[] wmsLayers, List<Layer> esriLayers, Dictionary<int, Layer> allLayers,int parentId, ref int count)
        {
            foreach (var wmsLayer in wmsLayers)
            {
                var esriLayer = new Layer();
                esriLayer.ParentLayerId = parentId;
                esriLayers.Add(esriLayer);
                esriLayer.Name = wmsLayer.Name;
                esriLayer.Id = count++;
                if (wmsLayer.ScaleHint != null)
                {
                    esriLayer.MinScale = wmsLayer.ScaleHint.Min;
                    esriLayer.MaxScale = wmsLayer.ScaleHint.Max;
                }

                allLayers.Add(esriLayer.Id, esriLayer);
                if (wmsLayer.Layers != null && wmsLayer.Layers.Length > 0)
                {
                    var subLayers = new List<Layer>(wmsLayer.Layers.Length);
                    TraverseLayers(wmsLayer.Layers, subLayers, allLayers, esriLayer.Id, ref count);
                    esriLayer.Sublayers = subLayers.ToArray();
                }
            }
        }
    }

    
}
