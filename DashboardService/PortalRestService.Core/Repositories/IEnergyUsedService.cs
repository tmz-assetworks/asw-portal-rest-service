using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;


namespace PortalRestService.Core.Repositories
{

    public interface IEnergyUsedRepository : IRepository<EnergyUsedResponse>
    {
        //custom operations here
       Task<EnergyUsedResponse> GetEnergyUsed();
    }
  
}
