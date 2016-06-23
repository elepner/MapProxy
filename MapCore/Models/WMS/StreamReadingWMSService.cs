using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MapCore.Models.WMS
{
    public abstract class StreamReadingWMSService : IWMSService
    {

        protected abstract Task<Stream> GetWMSCapabilitiesStream();

        public async Task<WmsService> GetServiceInformation()
        {
            WmsService wmsServiceInfo;
            using (var stream = await GetWMSCapabilitiesStream())
            {
                var serializer = new XmlSerializer(typeof(WmsService));
                wmsServiceInfo = (WmsService)serializer.Deserialize(stream);
            }
            if(wmsServiceInfo == null) throw new FormatException("Input stream was in incorrect format.");

            return wmsServiceInfo;
        }
    }
}
