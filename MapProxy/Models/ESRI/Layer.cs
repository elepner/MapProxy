using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapProxy.Models.ESRI
{
    public class Layer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpatialReference { get; set; }
        public string Type => "FeatureLayer";
        public string GeometryType { get; set; }
        public Layer[] Sublayers { get; set; }
    }
}