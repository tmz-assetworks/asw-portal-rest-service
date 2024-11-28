using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{

    public interface IMilesAddedByLocationQueryRepository : IRepository<MilesAddedByLocationChartResponse>
    {
        Task<MilesAddedByLocationChartResponse> GetMilesAddedByLocation(List<int> location, string duration, string chargeBoxId);
        Task<DurationAndIntervalDTO> durationAndIntervalAsync(string duration);
    }
}
