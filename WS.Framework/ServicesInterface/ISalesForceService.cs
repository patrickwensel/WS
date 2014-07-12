using System.Threading.Tasks;
using WS.Framework.Objects.SalesForce;

namespace WS.Framework.ServicesInterface
{
    public interface ISalesForceService
    {
        Task<bool> PostLead(SFLead sfLead);
    }
}
