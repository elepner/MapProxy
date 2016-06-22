using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MapProxy.Models.WMS
{
    public class Range
    {
        [XmlAttribute("min")]
        public double Min { get; set; }
        [XmlAttribute("max")]
        public double Max { get; set; }
    }
}