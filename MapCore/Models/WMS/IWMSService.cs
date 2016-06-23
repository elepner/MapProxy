using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MapCore.Models.WMS
{
    public interface IWMSService
    {
        Task<WmsService> GetServiceInformation();
    }
}
