using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
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
		async Task<PagedList<EventLogLocation>> IEventLogByLocationRepository.GetEventLogByLocation(EventLogRequest request)
        {
            List<EventLogLocation> res = new List<EventLogLocation>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
			int Rcount = 0;
			try
            {
				List<long> locationIdList = await _locationRepository.GetAllLocationIdByObjectId();
				int countr = request.PageNumber * request.PageSize;
				var allchargerBoxIds = (from charger in _dbContext.Charger
										join locations in request.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => request.LocationIds.Contains((int)x.Id) && locationIdList.Contains(x.Id)) : _dbContext.Locations.Where(x => locationIdList.Contains(x.Id)) on charger.LocationId equals locations.Id
										join address in _dbContext.LocationAddress on locations.LocationAddressId equals address.Id
										join Status in _dbContext.LocationStatus on locations.LocationStatusId equals Status.Id
										select charger.ChargeBoxId

				   ).Distinct().ToList();
				if (allchargerBoxIds.Count > 0)
				{
					List<string> list = new List<string>();
					if (request.ChargerBoxIds.Count > 0)
					{
						list = request.ChargerBoxIds.Intersect(allchargerBoxIds).ToList();
						request.ChargerBoxIds.Clear();
					}
					else
					{
						list = allchargerBoxIds;
					}
					request.ChargerBoxIds = list;
					if (request.ChargerBoxIds.Count > 0)
					{
						if (!string.IsNullOrEmpty(request.SearchParam))
						{
							Rcount = (from s in _dbContext.OcppEventLogs.Where(o => request.ChargerBoxIds.Contains(o.DeviceId)).Where(d => d.RequestType.ToLower().StartsWith(request.SearchParam.ToLower()) || d.DeviceId.ToLower() == request.SearchParam.ToLower())
									  select s).Count();
						}
						else
						{
							Rcount = (from s in _dbContext.OcppEventLogs.Where(o => request.ChargerBoxIds.Contains(o.DeviceId))
									  select s).Count();
						}
					}
				}
				res = (from s in string.IsNullOrEmpty(request.SearchParam) ? _dbContext.OcppEventLogs.Where(o => request.ChargerBoxIds.Contains(o.DeviceId)).OrderByDescending(o => o.Id).Take(countr).ToList() : _dbContext.OcppEventLogs.Where(o => request.ChargerBoxIds.Contains(o.DeviceId)).Where(d => d.RequestType.ToLower().StartsWith(request.SearchParam.ToLower()) || d.DeviceId.ToLower() == request.SearchParam.ToLower()).OrderByDescending(o => o.Id).Take(countr).ToList()
					   join charger in _dbContext.Charger on s.DeviceId equals charger.ChargeBoxId
                       join locations in request.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => request.LocationIds.Contains((int)x.Id) && locationIdList.Contains(x.Id)) : _dbContext.Locations.Where(x => locationIdList.Contains(x.Id)) on charger.LocationId equals locations.Id
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
			var dataResult = PagedList<EventLogLocation>.ToPageList(res,
			  request.PageNumber,
			  request.PageSize, Rcount);
			return await Task.FromResult(dataResult);

        }

    }
}
