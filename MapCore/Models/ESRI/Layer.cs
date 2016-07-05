using System.Linq;
using Newtonsoft.Json;

namespace MapCore.Models.ESRI
{
    public class Layer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Type => "FeatureLayer";
        public string GeometryType { get; set; }
        public double MinScale { get; set; }
        public double MaxScale { get; set; }

        public bool DefaultVisibility => true;

        public int[] SubLayerIds
        {
            get { return Sublayers?.Where(sublayer => Sublayers != null).Select(sublayers => sublayers.Id).ToArray(); }
        }
        public int ParentLayerId { get; set; }
        [JsonIgnore]
        public Layer[] Sublayers { get; set; }
    }
}