using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{
    public interface IEnergyUsedByLocationIDRepository : IRepository<EnergyUsedBOForChartResponse>
    {
        Task<EnergyUsedBOForChartResponse> GetEnergyUsedByLocationID(List<int> Location, string Duration, string chargeBoxId);
    }
}
