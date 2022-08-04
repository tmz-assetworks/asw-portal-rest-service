using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{
    public interface ILocationPerformingRepository : IRepository<LocationPerformingChartResponse>
    {
        Task<LocationPerformingChartResponse> GetLocationPerforming(List<int> location, string duration, int orderby);
    }
}
