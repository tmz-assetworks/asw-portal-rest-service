using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
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
    public class GetChargerByLocationIDRepository : OcppRepository<ChargerStatusForChartResponse>, IChargerByLocationRepository
    {
        TokenBase _tokenBase;
        private readonly ILocationRepository _locationRepository;
        public GetChargerByLocationIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token, ILocationRepository locationRepository) : base(dbContext)
        {
            _tokenBase = token;
            _locationRepository = locationRepository;
        }

        public Task<ChargerStatusResponse> AddAsync(ChargerStatusResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ChargerStatusResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task<ChargerStatusResponse> UpdateAsync(ChargerStatusResponse entity)
        {
            throw new NotImplementedException();
        }


        async Task<ChargerStatusForChartResponse> IChargerByLocationRepository.GetChargerStatusByLocationID(List<int> locations, string duration, string chargeBoxId)
        {
            ChargerStatusForChartResponse obj = new ChargerStatusForChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                string laveltype = "time";
                TimeSpan interval = new TimeSpan(4, 0, 0);
                if (duration == "7")
                {
                    duration = "6";
                    interval = new TimeSpan(24, 0, 0);
                    laveltype = "day";
                }
                else
                if (duration == "30")
                {
                    duration = "28";
                    interval = new TimeSpan(24 * 7, 0, 0);
                    laveltype = "date";
                }
                else
                if (duration == "90")
                {
                    interval = new TimeSpan(24, 0, 0);
                    laveltype = "month";
                }

                List<long> locationList = await _locationRepository.GetAllLocationIdByObjectId();

                List<ChargerByLocationBO> res = (from s in !string.IsNullOrEmpty(chargeBoxId)==true ? _dbContext.ChargingSessions.ToList().Where(o => chargeBoxId.ToLower().Equals(o.DeviceId.ToLower()) && o.DeviceId != null) : _dbContext.ChargingSessions.ToList().Where(o => o.DeviceId != null)
                                                 where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                                 join charger in  _dbContext.Charger on s.ChargerId equals charger.Id
                                                 join location in locations.Count > 0 ? _dbContext.Locations.Where(x => locations.Contains((int)(x.Id)) && locationList.Contains(x.Id)) : _dbContext.Locations.Where(x => locationList.Contains(x.Id)) on charger.LocationId equals location.Id
                                                 join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                                                 join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id
                                                 //join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                                 //on location.Id equals userMap.LocationId
                                                 select new ChargerByLocationBO
                                                 {

                                                     ChargerId = (long)s.ChargerId,
                                                     ChargingStatus = (
                                                                 s.ChargingStatus.ToLower().Equals("completed") ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("cancelled") ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("interrupted") ? "Available" : "Unavailable"
                                                        ),
                                                     StartTime = s.StartTime,
                                                     ChargeBoxId = charger.ChargeBoxId,
                                                     svalue = (s.StartTime.HasValue == true ?
                                                     laveltype == "time" ? (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("HH") :
                                                     laveltype == "day" ? (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") :
                                                     laveltype == "date" ? (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") :
                                                     (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") : ""),
                                                     times = (s.StartTime.HasValue == true ?
                                                     laveltype == "time" ? (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("HH") :
                                                     laveltype == "day" ? (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("dddd") :
                                                     laveltype == "date" ? (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("dd-MM-yyyy") :
                                                     (new DateTime((s.StartTime.Value.Ticks / interval.Ticks) * interval.Ticks)).ToString("MMMM") : ""),
                                                 }).ToList<ChargerByLocationBO>();

                List<ChargerByLocationChartBO> finalon = null;
  
                if (res.Count <= 0)
                {
                    finalon = getstatus(duration);
                }
                else {
                    finalon = res
                       .GroupBy(x => new { x.times, x.ChargingStatus })
                       .Select(y => new ChargerByLocationChartBO()
                       {
                           ChargeStatus = y.Key.ChargingStatus,
                           svalue = y.Max(f => f.svalue),
                           times = y.Key.times.Length >= 2 ? y.Key.times : "0" + y.Key.times,
                           Counts = y.ToList().Count,
                           Color = Extensions.GetColorCodesByCharger(y.Key.ChargingStatus)

                       }
                       ).OrderBy(t => t.svalue).ThenBy(t => t.ChargeStatus).ToList<ChargerByLocationChartBO>();
                }
                


                obj.StatusMessage = RespnoseMessage.Record_found;
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = RespnoseMessage.Opeartion_Failed;
                obj.StatusCode = 404;
                obj.data = new List<ChargerByLocationChartBO>();
            }

            return obj;
        }
        public List<ChargerByLocationChartBO> getstatus(string duration)
        {
            List<ChargerByLocationChartBO> chargingSessionByLocationBOs = new List<ChargerByLocationChartBO>();

            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            if (duration == "1")
            {
                duration = "1";
                interval = new TimeSpan(4, 0, 0);
                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = "04", ChargeStatus = "Available", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = "04" });
                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = "08", ChargeStatus = "Unavailable", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = "08" });
                

            }
            if (duration == "6")
            {
                duration = "6";
                interval = new TimeSpan(24, 0, 0);
                laveltype = "day";

                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = DateTime.Now.AddDays(-1).ToString("dddd"), ChargeStatus = "Available", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddDays(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = DateTime.Now.AddDays(-2).ToString("dddd"), ChargeStatus = "Unavailable", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                

            }
            else
            if (duration == "28")
            {

                interval = new TimeSpan(24 * 7, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = DateTime.Now.AddDays(-6).ToString("dd-MM-yyyy"), ChargeStatus = "Available", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddDays(-6).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = DateTime.Now.AddDays(-12).ToString("dd-MM-yyyy"), ChargeStatus = "Unavailable", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-12).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                
            }
            else
            if (duration == "90")
            {
                interval = new TimeSpan(24, 0, 0);
                laveltype = "month";
                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = DateTime.Now.AddMonths(-1).ToString("MMMM"), ChargeStatus = "Available", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddMonths(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargerByLocationChartBO() { times = DateTime.Now.AddMonths(-2).ToString("MMMM"), ChargeStatus = "Unavailable", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddMonths(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
            


            }
            return chargingSessionByLocationBOs;

        }
    }
}
