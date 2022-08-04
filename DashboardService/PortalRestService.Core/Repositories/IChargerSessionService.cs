using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;

namespace PortalRestService.Core.Repositories
{

    public interface IChargerSessionRepository : IRepository<ChargingSessionByLocationForChartResponse>
    {
        //custom operations here
       Task<ChargingSessionByLocationForChartResponse> GetChargerSession(List<int> location, string duration);
    }
  
}
