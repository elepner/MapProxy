using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MapCore.Models.ESRI
{
    public class MapService
    {
        [JsonProperty("mapName")]
        public string Name { get; set; }
        [JsonProperty("ServiceDescription")]
        public string Descritpion { get; set; }
        [JsonIgnore]
        public Layer Root { get; set; }
        [JsonIgnore]
        public Dictionary<int, Layer> AllLayers { get; set; }

        public SpatialReference SpatialReference { get; set; }

        public Extent InitialExtent { get; set; }
        public Extent FullExtent { get; set; }

        public string CurrentVersion => "10.3";

        public Layer[] Layers
        {
            get
            {
                return AllLayers?.Where(kvPair => kvPair.Key != -1).Select(kvPair => kvPair.Value).ToArray();
            }
        }

        public bool SupportsDynamicLayers => false;

        public string Units => "esriMeters";
        public string SupportedImageFormatTypes => "PNG32,PNG24";
        public bool ExportTilesAllowed => false;
        public string[] Tables => new string[0];

    }
}