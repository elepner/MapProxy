using System.Xml.Serialization;

namespace MapCore.Models.WMS
{
    [XmlRoot("WMT_MS_Capabilities")]
    public class WmsService
    {
        [XmlElement("Service")]
        public ServiceInfo ServiceInfo { get; set; }

        [XmlElement("Capability")]
        public Capability Capability { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }
    }

    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public class Capability
    {
        [XmlElement("Layer")]
        public Layer RootLayer { get; set; }
    }
}