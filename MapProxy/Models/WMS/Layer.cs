using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace MapProxy.Models.WMS
{
    public class Layer
    {
        [XmlAttribute("queryable")]
        public bool Queryable { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        [XmlElement("SRS")]
        public string SpatialReference { get; set; }
        public Range ScaleHint { get; set; }
        [XmlElement("Layer")]
        public Layer[] Layers { get; set; }

    }
}