using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MapCore.Models.WMS
{
    public class BoundingBox
    {
        [XmlIgnore]
        public int Wkid { get; private set; }

        [XmlAttribute("minx")]
        public double XMin { get; set; }
        [XmlAttribute("miny")]
        public double YMin { get; set; }
        [XmlAttribute("maxx")]
        public double XMax { get; set; }
        [XmlAttribute("maxy")]
        public double YMax { get; set; }

        [XmlAttribute("SRS")]
        public string Srs
        {
            get { return "EPSG:" + Wkid; }
            set
            {
                string wkid = value.Replace("EPSG:", "");
                Wkid = int.Parse(wkid);
            }
        }
    }
}
