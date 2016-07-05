using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapCore.Models.ESRI
{
    public class SpatialReference
    {

        public SpatialReference()
        {
            
        }

        public SpatialReference(int wkid)
        {
            Wkid = LatestWkid = wkid;
        }
        public int Wkid { get; set; }
        public int LatestWkid { get; set; }
    }
}
