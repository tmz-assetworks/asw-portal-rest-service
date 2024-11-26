using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{
    public interface ILocationRepository : IRepository<LocationDTO>
    {
        Task<List<long>> GetAllLocationIdByObjectId();
        Task<PagedList<Core.Responses.LocationsDispenserDetails>> GetLocationsDispenserDetails(LocationDispenserDetailRequest locationDispenserRequest);
        Task<AllLocationQueryResponse> GetAllLocation();
    }
}
