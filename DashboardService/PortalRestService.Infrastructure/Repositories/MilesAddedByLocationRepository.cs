using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
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
    public class MilesAddedByLocationRepository : OcppRepository<MilesAddedByLocationChartResponse>, IMilesAddedByLocationQueryRepository
    {
        private IConfiguration Configuration;
        TokenBase _tokenBase;
        public MilesAddedByLocationRepository(IConfiguration iConfig, Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            Configuration = iConfig;
            _tokenBase = token;
        }

        async Task<MilesAddedByLocationChartResponse> IMilesAddedByLocationQueryRepository.GetMilesAddedByLocation(List<int> locations, string duration, string chargeBoxId)
        {
            List<MilesAddedByLocationResponse> finalon = null;
            MilesAddedByLocationChartResponse obj = new MilesAddedByLocationChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                DurationAndIntervalDTO dTO = await durationAndIntervalAsync(duration);


                string laveltype = dTO.laveltype;
                TimeSpan interval = dTO.interval;
                duration = dTO.duration;

                List<ChargingSessionByLocationBO> res = (from s in _dbContext.ChargingSessions.ToList()
                                                         where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
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

                                                             SerialNumber = "",
                                                         }).ToList<ChargingSessionByLocationBO>();

                string fuleeffersiancy = this.Configuration.GetSection("Variable")["fuleeffersiancy"];
                finalon = null;
                if (!string.IsNullOrEmpty(chargeBoxId))
                {
                    if (res != null)
                    {
                        res = res.Where(f => f.ChargeBoxId == chargeBoxId).ToList();
                    }
                }
                if (res.Count <= 0)
                {
                    finalon = getstatus(duration);
                }
                else
                {
                    finalon = res
                .GroupBy(x => new { x.times })
                .Select(y => new MilesAddedByLocationResponse()
                {
                    // Times = y.Key.times.Length == 2 ? y.Key.times : "0" + y.Key.times,
                    svalue = y.Max(f => f.svalue),
                    Times = y.Key.times.Length >= 2 ? y.Key.times : "0" + y.Key.times,
                    RangeAdded = Math.Round(((Convert.ToDouble(y.Sum(t => t.EndMeterValue)) - (Convert.ToDouble(y.Sum(t => t.StartMeterValue)) <= 0 ? 0 : Convert.ToDouble(y.Sum(t => t.StartMeterValue)))) / (100 * Convert.ToDouble(fuleeffersiancy)))/1000, 2)

                }
                ).OrderBy(t => t.svalue).ToList<MilesAddedByLocationResponse>();
                }
               

                if (finalon.Count > 0)
                    obj.StatusMessage = RespnoseMessage.Record_found;
                else
                    obj.StatusMessage = RespnoseMessage.Record_not_found;
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = RespnoseMessage.Opeartion_Failed;
                obj.StatusCode = RespnoseCode.Bad_Request;
                obj.data = new List<MilesAddedByLocationResponse>();
            }
            return obj;
        }
        public List<MilesAddedByLocationResponse> getstatus(string duration)
        {
            List<MilesAddedByLocationResponse> chargingSessionByLocationBOs = new List<MilesAddedByLocationResponse>();

            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            if (duration == "1")
            {
                duration = "1";
                interval = new TimeSpan(4, 0, 0);
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = "04", svalue = "04", RangeAdded = 0 });


            }
            if (duration == "6")
            {
                duration = "6";
                interval = new TimeSpan(24, 0, 0);
                laveltype = "day";

                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-1).ToString("dddd"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-2).ToString("dddd"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-3).ToString("dddd"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-4).ToString("dddd"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });

            }
            else
            if (duration == "28")
            {

                interval = new TimeSpan(24 * 7, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-6).ToString("dd-MM-yyyy"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-6).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-12).ToString("dd-MM-yyyy"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-12).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-18).ToString("dd-MM-yyyy"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-18).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddDays(-24).ToString("dd-MM-yyyy"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddDays(-24).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
            }
            else
            if (duration == "90")
            {
                interval = new TimeSpan(24, 0, 0);
                laveltype = "month";
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddMonths(-1).ToString("MMMM"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddMonths(-2).ToString("MMMM"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddMonths(-3).ToString("MMMM"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new MilesAddedByLocationResponse() { Times = DateTime.Now.AddMonths(-4).ToString("MMMM"), RangeAdded = 0, svalue = (new DateTime((DateTime.Now.AddMonths(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });


            }
            return chargingSessionByLocationBOs;

        }

        public Task<DurationAndIntervalDTO> durationAndIntervalAsync(string duration)
        {
            DurationAndIntervalDTO dTO = new DurationAndIntervalDTO();
            if (string.IsNullOrEmpty(duration) || duration.ToLower() == "string")
                duration = "1";
            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            dTO.interval = interval;
            dTO.laveltype = laveltype;
            dTO.duration = duration;
            if (duration == "7")
            {
                dTO.duration = "6";
                dTO.interval = new TimeSpan(24, 0, 0);
                dTO.laveltype = "day";
            }
            else
            if (duration == "30")
            {
                dTO.duration = "28";
                dTO.interval = new TimeSpan(24 * 7, 0, 0);
                dTO.laveltype = "date";
            }
            else
            if (duration == "90")
            {
                dTO.interval = new TimeSpan(24, 0, 0);
                dTO.laveltype = "month";
            }
            return Task.FromResult(dTO);
        }
    }
}
