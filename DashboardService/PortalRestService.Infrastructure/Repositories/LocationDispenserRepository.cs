using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Text;
using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Models;

namespace PortalRestService.Infrastructure.Repositories
{
    public class LocationDispenserRepository : OcppRepository<LocationDispenserForLocationResponse>, ILocationDispenserRepository
    {
        TokenBase _tokenBase;
        public LocationDispenserRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }

        async Task<LocationDispenserForLocationResponse> ILocationDispenserRepository.GetDispenserByLocation(LocationDispensersRequest locationDispensersRequest)
        {
            LocationDispenserForLocationResponse objLocationDispneser = new LocationDispenserForLocationResponse();
            
            objLocationDispneser.data =  (from location in locationDispensersRequest.locationIds.Count>0? _dbContext.Locations.Where(x=> locationDispensersRequest.locationIds.Contains(x.Id)): _dbContext.Locations
                                   join charger in !string.IsNullOrEmpty(locationDispensersRequest.SearchParam) == true ? _dbContext.Charger.Where(d => locationDispensersRequest.SearchParam.ToLower().Contains(d.ChargeBoxId.ToLower())) : _dbContext.Charger
                               on location.Id equals charger.LocationId
                               join address in _dbContext.LocationAddress
                               on location.LocationAddressId equals address.Id
                               join Status in _dbContext.LocationStatus
                               on location.LocationStatusId equals Status.Id
                               join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                               on location.Id equals userMap.LocationId
                               select new LocationDispenserForLocation
                               {
                                   DispenserId = charger.Id,
                                   locationId = location.Id,
                                   ChargeBoxId = charger.ChargeBoxId,
                                   ChargerStatus = charger.ChargerStatuses == null || charger.ChargerStatuses.Count == 0 ? "Offline" :
                                    charger.ChargerStatuses.ToList()[0].Chargerstatus.Replace("charging", "Busy").Replace("suspendedev", "Busy").Replace("uspendedevse", "Busy")
                                  .Replace("finishing", "Busy").Replace("preparing", "Busy"),
                                   ConnectorType = String.Join(",",_dbContext.Connector.Where(cnn=> charger.Ports.Where(p => p.ChargerId == charger.Id).Select(s => s.ConnectorType ).Contains(cnn.Id)).Select(z=>z.ConnectorType)),
                                   DispenserModel = charger.ModelName,
                                   ProtocolName = charger.ProtocolName,
                                   NoofPort = charger.Ports.Count.ToString(),
                                   DispenserMake = charger.MakeName,
                                   ModifiedAt= charger.CreatedOn
                               }

                           ).OrderByDescending(m => m.ModifiedAt).ToList<LocationDispenserForLocation>();
            return objLocationDispneser;
        }
    }
}
