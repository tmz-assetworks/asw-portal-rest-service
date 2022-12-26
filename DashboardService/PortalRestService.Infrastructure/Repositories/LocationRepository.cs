using Microsoft.EntityFrameworkCore;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Models;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class LocationRepository : OcppRepository<LocationsDispenserDetails>, ILocationRepository
    {
        string JSONString = string.Empty;
        TokenBase _tokenBase;
        public LocationRepository(PortalRestService.Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }

        public async Task<AllLocationQueryResponse> GetAllLocation()
        {
            AllLocationQueryResponse objlocationdata = new AllLocationQueryResponse();
            try
            {
                objlocationdata .data= _dbContext.Locations
                  .Select(m => new LocationData
                  {
                       Id= m.Id,
                      LocationName = m.LocationName
                  }).Where(m => m.LocationName != "").OrderBy(m => m.LocationName).ToList<LocationData>();
                if (objlocationdata.data.Count > 0)
                    objlocationdata.StatusMessage = RespnoseMessage.Record_found;
                else
                {
                    objlocationdata.StatusMessage = RespnoseMessage.Record_not_found;
                    objlocationdata.StatusCode = 200;
                }
                    

            }
            catch (Exception ex)
            {
                objlocationdata.StatusMessage = RespnoseMessage.Opeartion_Failed;
                objlocationdata.StatusCode = RespnoseCode.Bad_Request;
                objlocationdata.data = new List<LocationData>();
            }
            return objlocationdata;
        }

        public Task<PagedList<LocationsDispenserDetails>> GetLocationsDispenserDetails(LocationDispenserDetailRequest locationDispenserRequest)
        {
            List<LocationsDispenserDetails> result = new List<LocationsDispenserDetails>();
            if (locationDispenserRequest.LocationIds.Count <= 0 || locationDispenserRequest.LocationIds == null)
            {
                result = (from location in _dbContext.Locations
                          join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                         on location.Id equals userMap.LocationId
                          select new LocationsDispenserDetails
                          {
                              Address = location.LocationAddress.AddressLine1 + " " + location.LocationAddress.AddressLine2,

                              locationId = location.Id,
                              CreatedOn = location.CreatedOn,

                              LocationName = location.LocationName,

                              status = location.LocationStatus.LocationStatusName,
                              NoofPort = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id)
                                          join port in _dbContext.Port
                              on charger.Id equals port.ChargerId
                              select new Port
                              {
                                  ChargerId = charger.Id
                                          }).ToList<Port>().Count.ToString(),
                              Available = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id && x.ChargerStatuses != null && x.ChargerStatuses.ToList().Count > 0)
                                           join Status in _dbContext.ChargerStatuses.Where(s => s.ConnectorStatus == "Available")
                              on charger.Id equals Status.ChargerId   //"Available")
                                           select new LocationsDispenserStatus
                                           {
                                               Id = charger.Id,
                                               Status = charger.ChargerStatuses.ToList()[0].ConnectorStatus
                                           }
                              ).ToList()
                                           .Count.ToString(),
                              Connected = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id && x.ChargerStatuses != null && x.ChargerStatuses.ToList().Count > 0)
                                           join Status in _dbContext.ChargerStatuses.Where(s => s.ConnectorStatus == "Unavailable")
                                            on charger.Id equals Status.ChargerId
                                           select new LocationsDispenserStatus
                                           {
                                               Id = charger.Id,
                                               Status = charger.ChargerStatuses.ToList()[0].ConnectorStatus
                                           }
                              ).ToList()
                                           .Count.ToString(),
                              Faulted = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id && x.ChargerStatuses != null && x.ChargerStatuses.ToList().Count > 0)
                                         join Status in _dbContext.ChargerStatuses.Where(s => s.ConnectorStatus == "Faulted")
                                          on charger.Id equals Status.ChargerId
                                         select new LocationsDispenserStatus
                                         {
                                             Id = charger.Id,
                                             Status = charger.ChargerStatuses.ToList()[0].ConnectorStatus
                                         }
                                           ).ToList()
                                           .Count.ToString(),
                              ContactNo = location.ContactPersonNumber.ToString(),
                              ContactName = location.ContactPersonName.ToString(),

                          }).ToList<LocationsDispenserDetails>();
            }
            else
            {
                result = (from location in _dbContext.Locations.Where(x => locationDispenserRequest.LocationIds.Contains(x.Id))
                join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                         on location.Id equals userMap.LocationId
                          select new LocationsDispenserDetails
                          {
                              Address = location.LocationAddress.AddressLine1 + " " + location.LocationAddress.AddressLine2,

                              locationId = location.Id,

                              LocationName = location.LocationName,
                              CreatedOn = location.CreatedOn,

                              status = location.LocationStatus.LocationStatusName,
                              NoofPort = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id)
                                          join port in _dbContext.Port
                              on charger.Id equals port.ChargerId
                                          select new Port
                                          {
                                              ChargerId = charger.Id
                                          }).ToList<Port>().Count.ToString(),
                              Available = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id && x.ChargerStatuses != null && x.ChargerStatuses.ToList().Count > 0)
                                           join Status in _dbContext.ChargerStatuses.Where(s => s.ConnectorStatus == "Available")
                              on charger.Id equals Status.ChargerId   //"Available")
                                           select new LocationsDispenserStatus
                                           {
                                               Id = charger.Id,
                                               Status = charger.ChargerStatuses.ToList()[0].ConnectorStatus
                                           }
                              ).ToList()
                                           .Count.ToString(),
                              Connected = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id && x.ChargerStatuses != null && x.ChargerStatuses.ToList().Count > 0)
                                           join Status in _dbContext.ChargerStatuses.Where(s => s.ConnectorStatus == "Unavailable")
                                            on charger.Id equals Status.ChargerId
                                           select new LocationsDispenserStatus
                                           {
                                               Id = charger.Id,
                                               Status = charger.ChargerStatuses.ToList()[0].ConnectorStatus
                                           }
                              ).ToList()
                                           .Count.ToString(),
                              Faulted = (from charger in _dbContext.Charger.Where(x => x.LocationId == location.Id && x.ChargerStatuses != null && x.ChargerStatuses.ToList().Count > 0)
                                         join Status in _dbContext.ChargerStatuses.Where(s => s.ConnectorStatus == "Faulted")
                                          on charger.Id equals Status.ChargerId
                                         select new LocationsDispenserStatus
                                         {
                                             Id = charger.Id,
                                             Status = charger.ChargerStatuses.ToList()[0].ConnectorStatus
                                         }
                                           ).ToList()
                                           .Count.ToString(),
                              ContactNo = location.ContactPersonNumber.ToString(),
                              ContactName = location.ContactPersonName.ToString(),


                          }).ToList<LocationsDispenserDetails>();
            }
            result = result != null ? result.OrderByDescending(a => a.locationId).ToList() : result;
            if (!string.IsNullOrEmpty(locationDispenserRequest.SearchParam))
                result = result.Where(d => d.LocationName.ToLower().Contains(locationDispenserRequest.SearchParam.ToLower())
             ).ToList<LocationsDispenserDetails>();
            //Paging on Records           

            var dataResult = PagedList<LocationsDispenserDetails>.ToPagedList(result,
              locationDispenserRequest.PageNumber,
              locationDispenserRequest.PageSize);
            return Task.FromResult(dataResult);
        }
    }
}
