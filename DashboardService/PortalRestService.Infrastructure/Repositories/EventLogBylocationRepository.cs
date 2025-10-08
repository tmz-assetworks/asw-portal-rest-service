using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class EventLogBylocationRepository : OcppRepository<EventLogLocationResponse>, IEventLogByLocationRepository
    {
        TokenBase _tokenBase;
        private readonly ILocationRepository _locationRepository;
       public EventLogBylocationRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token,ILocationRepository locationRepository) : base(dbContext)
       {
            this._tokenBase=token;
			this._locationRepository=locationRepository;
       }

        async Task<PagedList<EventLogLocation>> GetEventLogByLocationOld(EventLogRequest request)
        {
            List<EventLogLocation> res = new List<EventLogLocation>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {

                res = (from s in request.ChargerBoxIds.Count > 0 ? _dbContext.OcppEventLogs.Where(o => request.ChargerBoxIds.Contains(o.DeviceId) && o.DeviceId != null) : _dbContext.OcppEventLogs.Where(o => o.DeviceId != null)
                       join charger in _dbContext.Charger on s.DeviceId equals charger.ChargeBoxId
                       join locations in request.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => request.LocationIds.Contains((int)x.Id)) : _dbContext.Locations on charger.LocationId equals locations.Id
                       join address in _dbContext.LocationAddress on locations.LocationAddressId equals address.Id
                       join Status in _dbContext.LocationStatus on locations.LocationStatusId equals Status.Id
                       join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                       on locations.Id equals userMap.LocationId
                       select new EventLogLocation
                       {
                           Id = s.Id,
                           CreatedAt = s.CreatedAt,
                           DeviceId = s.DeviceId,
                           EventLogDataSource = s.EventLogDataSource,
                           ModifiedAt = s.ModifiedAt,
                           RequestId = s.RequestId,
                           RequestPayload = s.RequestPayload == null ? "" : s.RequestPayload.Replace(",", ",\r\n"),
                           RequestType = s.RequestType,
                           ResponsePayload = s.ResponsePayload == null ? "" : s.ResponsePayload.Replace(",", ",\r\n"),
                           LocationId = locations.LocationId.ToString(),
                           LocationName = locations.LocationName,
                           RequestTypeColor = Extensions.GetEventlogColorCodes(s.RequestType == null ? "" : s.RequestType),
                           IsRead = s.IsRead.HasValue == true ? s.IsRead.Value : false
                       }).AsEnumerable()
                       .DistinctBy(d => d.Id).Where(s => s.DeviceId != null).ToList();

            }
            catch (Exception ex)
            {

            }

            res = res != null ? res.OrderByDescending(a => a.ModifiedAt).ToList() : res;
            if (!string.IsNullOrEmpty(request.SearchParam))
                res = res.Where(d => d.RequestType.ToLower().StartsWith(request.SearchParam.ToLower()) || d.DeviceId.ToLower() == request.SearchParam.ToLower()).ToList();

            var dataResult = PagedList<EventLogLocation>.ToPagedList(res,
              request.PageNumber,
              request.PageSize);
            return await Task.FromResult(dataResult);

        }
        public async Task<PagedList<EventLogLocation>> GetEventLogByLocation(EventLogRequest request)
        {
            try
            {
                var allowedLocationIds = await _locationRepository.GetAllLocationIdByObjectId();

                var locationsQuery = request.LocationIds.Count > 0
                    ? _dbContext.Locations.Where(l => request.LocationIds.Contains((int)l.Id) && allowedLocationIds.Contains(l.Id))
                    : _dbContext.Locations.Where(l => allowedLocationIds.Contains(l.Id));

                var eventLogsBase = request.ChargerBoxIds.Count > 0
                    ? _dbContext.OcppEventLogs.Where(o => o.DeviceId != null && request.ChargerBoxIds.Contains(o.DeviceId))
                    : _dbContext.OcppEventLogs.Where(o => o.DeviceId != null);

                var eventLogsQuery =
                    from log in eventLogsBase
                    join charger in _dbContext.Charger on log.DeviceId equals charger.ChargeBoxId
                    join loc in locationsQuery on charger.LocationId equals loc.Id
                    select new EventLogLocation
                    {
                        Id = log.Id,
                        CreatedAt = log.CreatedAt,
                        DeviceId = log.DeviceId,
                        EventLogDataSource = log.EventLogDataSource,
                        ModifiedAt = log.ModifiedAt,
                        RequestId = log.RequestId,
                        RequestPayload = log.RequestPayload != null ? log.RequestPayload.Replace(",", ",\r\n") : "",
                        RequestType = log.RequestType,
                        ResponsePayload = log.ResponsePayload != null ? log.ResponsePayload.Replace(",", ",\r\n") : "",
                        LocationId = loc.LocationId.ToString(),
                        LocationName = loc.LocationName,
                        RequestTypeColor = Extensions.GetEventlogColorCodes(log.RequestType ?? ""),
                        IsRead = log.IsRead ?? false
                    };

                if (!string.IsNullOrEmpty(request.SearchParam))
                {
                    string searchLower = request.SearchParam.ToLower();

                    eventLogsQuery = eventLogsQuery.Where(e =>
                        e.RequestType.ToLower().StartsWith(searchLower) ||
                        e.DeviceId.ToLower() == searchLower);
                }

                return await PagedList<EventLogLocation>.CreateAsync(
                    eventLogsQuery.OrderByDescending(e => e.Id),
                    request.PageNumber,
                    request.PageSize
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetEventLogByLocation for locationIDs {LocationIds}, chargerIDs {ChargerIds}, and search parameter {SearchParam}",
                    request.LocationIds, request.ChargerBoxIds, request.SearchParam);

                return await PagedList<EventLogLocation>.CreateAsync(
                    Enumerable.Empty<EventLogLocation>().AsQueryable(),
                    request.PageNumber,
                    request.PageSize
                );
            }
        }

    }
}
