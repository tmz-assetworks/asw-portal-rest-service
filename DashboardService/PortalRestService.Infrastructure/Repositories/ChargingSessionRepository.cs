using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net.Http.Headers;
using System.Text;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
    public class ChargingSessionRepository : OcppRepository<ChargerSessionByLocationResponse>, IChargingSessionRepository
    {
        TokenBase _tokenBase;
        public ChargingSessionRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }

        public Task<ChargingSessionByLocationForChartResponse> AddAsync(ChargingSessionByLocationForChartResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ChargingSessionByLocationForChartResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task<ChargingSessionByLocationForChartResponse> UpdateAsync(ChargingSessionByLocationForChartResponse entity)
        {
            throw new NotImplementedException();
        }

        public DateTime GetStartDate(int day)
        {
            return DateTime.Now.AddDays(-day);

        }
        async Task<ChargingSessionByLocationForChartResponse> IChargingSessionRepository.GetChargerSession(List<int> locations, string duration, string ChargerBoxId)
        {

            ChargingSessionByLocationForChartResponse obj = new ChargingSessionByLocationForChartResponse();
            
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                    if (string.IsNullOrEmpty(duration) || duration.ToLower() == "string")
                        duration = "1";
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
                List<ChargingSessionByLocationBO> res = (from s in _dbContext.ChargingSessions.ToList()
                                                         where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                                         join charger in !string.IsNullOrEmpty(ChargerBoxId) == true ? _dbContext.Charger.Where(x => ChargerBoxId.ToLower().Equals(x.ChargeBoxId.ToLower())) : _dbContext.Charger on s.ChargerId equals charger.Id
                                                         join location in locations.Count>0? _dbContext.Locations.Where(x=>locations.Contains((int)(x.Id))): _dbContext.Locations on charger.LocationId equals location.Id
                                                         join address in _dbContext.LocationAddress on location.LocationAddressId equals address.Id
                                                         join Status in _dbContext.LocationStatus on location.LocationStatusId equals Status.Id
                                                         join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                                                         on location.Id equals userMap.LocationId
                                                         select new ChargingSessionByLocationBO
                                                         {
                                                             Id = s.Id,
                                                          
                                                             ChargerId = (long)s.ChargerId,
                                                             ChargingCost = s.ChargingCost,
                                                             ChargingStatus = s.ChargingStatus,
                                                             ConnectorId = s.ConnectorId,
                                                             DeviceId = s.DeviceId,
                                                             ReasonForStop = s.ReasonForStop,
                                                             StartMeterValue = s.StartMeterValue,
                                                             StartSoc = s.StartSoc,
                                                             StartTime = s.StartTime,
                                                             EndMeterValue = s.EndMeterValue,
                                                             EndSoc = s.EndSoc,
                                                             EndTime = s.EndTime,
                                                             CreatedAt = s.CreatedAt,
                                                             ModifiedAt = s.ModifiedAt,
                                                             LocationId = (location.Id),
                                                             LocationName = location.LocationName,
                                                             ContactPersonName = location.ContactPersonName,
                                                             AddressLine1 = "",
                                                             LocationStatusName = "",
                                                             LocationStatusId = location.LocationStatusId,
                                                             ChargeBoxId = charger.ChargeBoxId,
                                                             // times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),
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

                List<ChargingSessionByLocationChartBO> finalon = null;
                
                if (res.Count <= 0)
                {
                    finalon = getstatus(duration);
                }
                else
                {
                        finalon = res
                    .GroupBy(x => new { x.times, x.ChargingStatus })
                    .Select(y => new ChargingSessionByLocationChartBO()
                    {
                        ChargingStatus = y.Key.ChargingStatus,
                        // times = y.Key.times.Length == 2 ? y.Key.times : "0" + y.Key.times,
                        svalue = y.Max(f => f.svalue),
                        times = y.Key.times.Length >= 2 ? y.Key.times : "0" + y.Key.times,
                        Counts = y.ToList().Count,
                        Color = Extensions.GetColorCodesByChargingSession(y.Key.ChargingStatus)
                    }
                    ).OrderBy(t => (t.svalue, t.ChargingStatus)).ToList<ChargingSessionByLocationChartBO>();
                }
                
                


                if(finalon.Count>0)
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
                obj.data = new List<ChargingSessionByLocationChartBO>();
            }
            return obj;
        }
        public List<ChargingSessionByLocationChartBO> getstatus(string duration)
        {
            List<ChargingSessionByLocationChartBO> chargingSessionByLocationBOs = new List<ChargingSessionByLocationChartBO>();

            string laveltype = "time";
            TimeSpan interval = new TimeSpan(4, 0, 0);
            if (duration == "1")
            {
                duration = "1";
                interval = new TimeSpan(4, 0, 0);
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "04", ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = "04" });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "08", ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = "08" });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "12", ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = "12" });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = "16", ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = "16" });

            }
            if (duration == "6")
            {
                duration = "6";
                interval = new TimeSpan(24, 0, 0);
                laveltype = "day";

                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-1).ToString("dddd"), ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddDays(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-2).ToString("dddd"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-3).ToString("dddd"), ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = (new DateTime((DateTime.Now.AddDays(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-4).ToString("dddd"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });

            }
            else
            if (duration == "28")
            {

                interval = new TimeSpan(24 * 7, 0, 0);
                laveltype = "date";

                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-6).ToString("dd-MM-yyyy"), ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddDays(-6).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-12).ToString("dd-MM-yyyy"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-12).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-18).ToString("dd-MM-yyyy"), ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = (new DateTime((DateTime.Now.AddDays(-18).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddDays(-24).ToString("dd-MM-yyyy"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddDays(-24).Ticks / interval.Ticks) * interval.Ticks)).ToString("MMdd") });
            }
            else
            if (duration == "90")
            {
                interval = new TimeSpan(24, 0, 0);
                laveltype = "month";
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-1).ToString("MMMM"), ChargingStatus = "Charging", Color = Extensions.GetColorCodesByChargingSession("Charging"), svalue = (new DateTime((DateTime.Now.AddMonths(-1).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-2).ToString("MMMM"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddMonths(-2).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-3).ToString("MMMM"), ChargingStatus = "Interrupted", Color = Extensions.GetColorCodesByChargingSession("Interrupted"), svalue = (new DateTime((DateTime.Now.AddMonths(-3).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });
                chargingSessionByLocationBOs.Add(new ChargingSessionByLocationChartBO() { times = DateTime.Now.AddMonths(-4).ToString("MMMM"), ChargingStatus = "Completed", Color = Extensions.GetColorCodesByChargingSession("Completed"), svalue = (new DateTime((DateTime.Now.AddMonths(-4).Ticks / interval.Ticks) * interval.Ticks)).ToString("MM") });


            }
            return chargingSessionByLocationBOs;

        }
    }
}

