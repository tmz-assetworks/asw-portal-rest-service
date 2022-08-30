using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{
    public interface IGetSummaryStatusRepository : IOcppRepository<CardDataResponse>
    {
        Task<CardDataResponse> GetSummaryStatus(int locationId, bool isChargersReq);
    }
}