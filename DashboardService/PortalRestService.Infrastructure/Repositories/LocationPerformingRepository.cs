using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
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
#pragma warning disable
    public class LocationPerformingRepository : OcppRepository<LocationPerformingChartResponse>, ILocationPerformingRepository
    {
        TokenBase _tokenBase;
        public LocationPerformingRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }
        async Task<LocationPerformingChartResponse> ILocationPerformingRepository.GetLocationPerforming(List<int> locations, string duration, int orderby)
        {
            List<LocationPerformingResponse> finalon = null;
            LocationPerformingChartResponse obj = new LocationPerformingChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                if (duration == "7")
                {
                    duration = "6";
                    
                }
                else
                if (duration == "30")
                {
                    duration = "28";
                   
                }
                //Add condition we will get data without current charging session
                List<ChargingSessionByLocationBO> res = (from s in _dbContext.ChargingSessions.ToList()
                                                         where s.ChargingStatus != "Charging" && s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                                         join charger in _dbContext.Charger on s.ChargerId equals charger.Id
                                                        join location in locations.Count > 0 ? _dbContext.Locations.Where(x => locations.Contains((int)(x.Id))) : _dbContext.Locations on charger.LocationId equals location.Id
                                                         join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                                                         join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id
                                                         join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                                         on location.Id equals userMap.LocationId
                                                         select new ChargingSessionByLocationBO
                                                         {
                                                             Id = s.Id,
                                                             ChargerId = (long)s.ChargerId,
                                                             StartMeterValue = s.StartMeterValue,
                                                             StartSoc = s.StartSoc,
                                                             StartTime = s.StartTime,
                                                             EndMeterValue = s.EndMeterValue,
                                                             EndSoc = s.EndSoc,
                                                             EndTime = s.EndTime,

                                                             LocationId = location.Id,
                                                             LocationName = location.LocationName,
                                                             LocationStatusName = Status.LocationStatusName,
                                                             LocationStatusId = location.LocationStatusId,
                                                             ChargeBoxId = charger.ChargeBoxId,
                                                             times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),

                                                             SerialNumber = "",
                                                         }).ToList<ChargingSessionByLocationBO>();
                if (res.Count <= 0)
                {
                    finalon = getstatus();
                }
                else
                {
                    if (orderby > 0)
                    {
                        finalon = res
                    .GroupBy(x => new { x.LocationName, x.LocationStatusName })
                    .Select(y => new LocationPerformingResponse()
                    {
                        LocationName = y.Key.LocationName,
                        MeterValue = Convert.ToInt32((y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue))/1000),
                        Orderby = "TOP",
                    }
                    ).OrderByDescending(t => t.MeterValue).Take(5).ToList<LocationPerformingResponse>();
                    }
                    else
                    {
                        finalon = res
                        .GroupBy(x => new { x.LocationName, x.LocationStatusId, x.LocationStatusName })
                        .Select(y => new LocationPerformingResponse()
                        {

                            LocationName = y.Key.LocationName,
                            MeterValue = Convert.ToInt32((y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue)) / 1000),
                            Orderby = "BOTTOM",

                        }
                    ).OrderBy(t => t.MeterValue).Take(5).ToList<LocationPerformingResponse>();
                    }
                }
                
                if (finalon != null && finalon.Count > 0)
                {
                    Dictionary<int, string> locationcolors = CommonHelpers.LocationStaticColorList();
                    int i = 1;
                    for (int j = 0; j < finalon.Count; j++)
                    {

                        finalon[j].Color = locationcolors[j + 1].Trim();
                    }
                    obj.StatusMessage = RespnoseMessage.Record_found;
                }
                else
                {
                    obj.StatusMessage = RespnoseMessage.Record_not_found;
                }
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = RespnoseMessage.Opeartion_Failed;
                obj.StatusCode = 404;
                obj.data = new List<LocationPerformingResponse>();
            }
            return obj;
        }
        public List<LocationPerformingResponse> getstatus()
        {
            List<LocationPerformingResponse> chargingSessionByLocationBOs = new List<LocationPerformingResponse>();


            chargingSessionByLocationBOs.Add(new LocationPerformingResponse() { Orderby = "TOP", LocationName = "Philadelphia", MeterValue = 0 });
            chargingSessionByLocationBOs.Add(new LocationPerformingResponse() { Orderby = "TOP",LocationName = "Austin Public Works", MeterValue = 0 });
            chargingSessionByLocationBOs.Add(new LocationPerformingResponse() { Orderby = "TOP", LocationName = "Alaska", MeterValue = 0 });
            chargingSessionByLocationBOs.Add(new LocationPerformingResponse() { Orderby = "TOP",LocationName = "Fleet Services", MeterValue = 0 });
            return chargingSessionByLocationBOs;
        }
    }
}
