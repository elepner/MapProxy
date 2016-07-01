using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task<IActionResult> Export([FromUri]ExportParameters exportParameters)
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
                {
                    "layers","va_avl_line,va_line,va_cablepnt,va_cableline,va_mpnt,va_cpnt,va_cpnt_line,va_lequip,va_pequip,va_lineflowdir,va_freetxt,va_freepnt"
                    //string.Join(",",
                    //    exportParameters.Layers.Values.Select(layerId => esriServiceInfo.AllLayers[(int) layerId].Name))
                },
                {"format", "image/png"}
            };

            var requestUriString = wmsServiceInfo.ServiceInfo.Resource.Url + "?";
            requestUriString += string.Join("&", requestParams.Select(x => x.Key + "=" + x.Value.ToString()));


            WebRequest webRequest = WebRequest.Create(requestUriString);
                        
            webRequest.Method = "GET";
                        
            //return Ok(requestUriString);
            var wmsResult = await webRequest.GetResponseAsync();
            var stream = wmsResult.GetResponseStream();
            var dump = @"C:\temp\dump.png";
            
            CopyStream(stream, dump);

            return new StreamResult(stream);

        }



        public void CopyStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
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

        class StreamResult : IActionResult
        {
            private readonly Stream _stream;

            public StreamResult(Stream stream)
            {
                _stream = stream;
            }

            public Task ExecuteResultAsync(ActionContext context)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(_stream)
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                return Task.FromResult(response);
            }
        }
    }

    
}
