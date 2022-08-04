using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;

namespace PortalRestService.Core.Repositories
{

    public interface ILocationStatusByLocationIdRepository : IRepository<AllLocationStatusChartBO>
    {
       Task<List<AllLocationStatusChartBO>> GetLocationStatusByLocatonId(List<int> location, string duration);
    }

}