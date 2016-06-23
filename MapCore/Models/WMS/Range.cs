using System.Xml.Serialization;

namespace MapCore.Models.WMS
{
    public class Range
    {
        [XmlAttribute("min")]
        public double Min { get; set; }
        [XmlAttribute("max")]
        public double Max { get; set; }
    }
}