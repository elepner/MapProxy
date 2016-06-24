using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapCore.Models.ESRI
{
    public class MapService
    {
        public string Name { get; set; }
        public string Descritpion { get; set; }
        public Layer Root { get; set; }
        [JsonIgnore]
        public Dictionary<int, Layer> AllLayers { get; set; }
    }
}