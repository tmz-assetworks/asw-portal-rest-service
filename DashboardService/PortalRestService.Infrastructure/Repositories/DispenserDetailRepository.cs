using Azure;
using PortalRestService.Core.Models;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Linq;
using System.Runtime.InteropServices;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
    public class DispensersDetailRepository : OcppRepository<DispensersDetail>, IDispenserDetailRepository
    {
        private readonly ILocationRepository _locationRepository;
        public DispensersDetailRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, ILocationRepository locationRepository) : base(dbContext)
        {
            _locationRepository = locationRepository;
        }
        public async Task<PagedList<DispensersDetail>> GetDispensersDetail(DispensersDetailRequest dispensersDetailRequest)
        {
            var locationIdList = await _locationRepository.GetAllLocationIdByObjectId();

            var query = from disp in _dbContext.Charger
                        join location in _dbContext.Locations
                            on disp.LocationId equals location.Id
                        where locationIdList.Contains(location.Id)
                        select new
                        {
                            disp,
                            location,
                            lastFault = disp.ChargerStatusHistories
                                .Where(x => x.ConnectorStatus == "Faulted")
                                .OrderByDescending(x => x.Id)
                                .Select(x => x.CreatedOn)
                                .FirstOrDefault()
                        };

            // Search filtering (server-side)
            if (!string.IsNullOrEmpty(dispensersDetailRequest.SearchParam))
            {
                string search = dispensersDetailRequest.SearchParam.ToLower();
                query = query.Where(q =>
                    q.disp.ChargeBoxId.ToLower().Contains(search) ||
                    q.disp.AssetId.ToLower().Contains(search) ||
                    q.disp.MakeName.ToLower().Contains(search) ||
                    q.disp.ModelName.ToLower().Contains(search) ||
                    q.location.LocationName.ToLower().Contains(search) ||
                    q.disp.SimCardMSIDN.ToLower().Contains(search));
            }

            // Projection (materialize only needed fields)
            var projected = query.Select(q => new DispensersDetail
            {
                AssetId = q.disp.AssetId,
                ChargerBoxId = q.disp.ChargeBoxId,
                TimeReported = q.lastFault.HasValue
                    ? q.lastFault.Value.ToString("dd-MM-yyyy HH:mm")
                    : "",
                FaultSince = q.lastFault.HasValue
                    ? (DateTime.UtcNow - q.lastFault.Value).Hours + " hours"
                    : "",
                LocationId = q.disp.LocationId ?? 0,
                State = q.location.LocationAddress != null ? q.location.LocationAddress.StateName : "",
                ChargerType = q.disp.Ports.FirstOrDefault().Connector.ConnectorType,
                LocationContactName = q.location.LocationName,
                LocationContactNumber = q.location.ContactPersonNumber,
                SimCardMSIDN = q.disp.SimCardMSIDN ?? "",
                MakeName = q.disp.MakeName,
                ModelName = q.disp.ModelName,
                ChargerStatus = q.disp.ChargerStatuses == null || q.disp.ChargerStatuses.Count == 0 ? "Offline" :
                                   q.disp.ChargerStatuses.FirstOrDefault().Chargerstatus.Replace("charging", "Busy").Replace("suspendedev", "Busy").Replace("uspendedevse", "Busy")
                                 .Replace("finishing", "Busy").Replace("preparing", "Busy"),
                NoofPort = q.disp.Ports.Count == 0 ? "0" : q.disp.Ports.Count.ToString(),
            });

            // Paging & Ordering in SQL
            var pagedResult = await PagedList<DispensersDetail>.CreateAsync(
                projected.OrderByDescending(x => x.ChargerBoxId),  // Replace ChargerName
                dispensersDetailRequest.PageNumber,
                dispensersDetailRequest.PageSize);

            return pagedResult;
        }
    }

}
