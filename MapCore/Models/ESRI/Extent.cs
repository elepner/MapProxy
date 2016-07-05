using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapCore.Models.ESRI
{
    public class Extent
    {
        public SpatialReference SpatialReference { get; set; }
        public double Xmin, Xmax, Ymin, Ymax;
    }
}
