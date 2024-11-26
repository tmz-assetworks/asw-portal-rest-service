using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
//using PortalRestService.Core.Responses;
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
#pragma warning disable
    public class EnergyUsedByLocationIDRepository : OcppRepository<EnergyUsedBOForChartResponse>, IEnergyUsedByLocationIDRepository
    {
        TokenBase _tokenBase;
        private readonly IConfiguration _configuration;
        private readonly string OccpIp = String.Empty;
        private readonly ILocationRepository _locationRepository;
        public EnergyUsedByLocationIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token, IConfiguration configuration, ILocationRepository locationRepository) : base(dbContext)
        {
            _tokenBase = token;
            this._configuration = configuration;
            OccpIp = this._configuration.GetSection("OccpIp").GetSection("ip").Value;
            _locationRepository = locationRepository;
        }
        async Task<EnergyUsedBOForChartResponse> IEnergyUsedByLocationIDRepository.GetEnergyUsedByLocationID(List<int> location, string duration, string chargeBoxId)
        {
            EnergyUsedBOForChartResponse obj = new EnergyUsedBOForChartResponse();
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
                List<EnergyUsedChartBO> res = (from s in _dbContext.ChargingSessions
                                               where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now && s.EndMeterValue>0
                                               join charger in !string.IsNullOrEmpty(chargeBoxId) == true ? _dbContext.Charger.Where(x => chargeBoxId.ToLower().Equals(x.ChargeBoxId.ToLower())) : _dbContext.Charger on s.ChargerId equals charger.Id
                                               join locations in location.Count>0? _dbContext.Locations.Where(x=>location.Contains((int)x.Id) && locationList.Contains(x.Id)) : _dbContext.Locations.Where(x => locationList.Contains(x.Id)) on charger.LocationId equals locations.Id
                                               join address in _dbContext.LocationAddress on locations.LocationAddressId equals address.Id
                                               join Status in _dbContext.LocationStatus on locations.LocationStatusId equals Status.Id
                                               //join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                               //on locations.Id equals userMap.LocationId
                                               select new EnergyUsedChartBO
                                               {
                                                   StartMeterValue = s.StartMeterValue.Value,
                                                   EndMeterValue = s.EndMeterValue.Value,
                                                   chargeboxId = charger.ChargeBoxId,
                                                   //times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),
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
                                               }).ToList<EnergyUsedChartBO>();

                List<EnergyUsedsResponse> finalon = null;

                if (res.Count <= 0)
                {
                    finalon = getstatus(duration);
                }
                else
                {
                    finalon = res
                    .GroupBy(x => new { x.times })
                    .Select(y => new EnergyUsedsResponse()
                    {

                        svalue = y.Max(f => f.svalue),
                        times = y.Key.times.Length >= 2 ? y.Key.times : "0" + y.Key.times,
                        EndMeterValue = Convert.ToInt32(y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue <= 0 ? 0 : c.StartMeterValue))/1000,
                    }
                    ).OrderBy(t => t.svalue).ToList<EnergyUsedsResponse>();
                }

                obj.StatusMessage = RespnoseMessage.Record_found;
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = RespnoseMessage.Faild;
                obj.StatusCode = 404;
                obj.data = new List<EnergyUsedsResponse>(); 
            }

            return obj;
        }
        public List<EnergyUsedsResponse> getstatus(string duration)
        {
            List<EnergyUsedsResponse> chargingSessionByLocationBOs = new List<EnergyUsedsResponse>();

            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            if (duration == "1")
            {
                duration = "1";
                interval = new TimeSpan(4, 0, 0);
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = "04", EndMeterValue = 0, svalue = "04" });


            }
            if (duration == "6")
            {
                duration = "6";
                interval = new TimeSpan(24, 0, 0);
                laveltype = "day";

                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-1).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-2).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-3).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-4).ToString("dddd"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });

            }
            else
            if (duration == "28")
            {

                interval = new TimeSpan(24 * 7, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-6).ToString("dd-MM-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-6).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-12).ToString("dd-MM-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-12).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-18).ToString("dd-MM-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-18).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddDays(-24).ToString("dd-MM-yyyy"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddDays(-24).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
            }
            else
            if (duration == "90")
            {
                interval = new TimeSpan(24, 0, 0);
                laveltype = "month";
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-1).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-2).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-3).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new EnergyUsedsResponse() { times = DateTime.Now.AddMonths(-4).ToString("MMMM"), EndMeterValue = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });


            }
            return chargingSessionByLocationBOs;

        }
    }
}
