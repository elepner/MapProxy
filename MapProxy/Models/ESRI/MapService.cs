using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapProxy.Models.ESRI
{
    public class MapService
    {
        public string Name { get; set; }
        public string Descritpion { get; set; }
        public Layer Root { get; set; }
    }
}