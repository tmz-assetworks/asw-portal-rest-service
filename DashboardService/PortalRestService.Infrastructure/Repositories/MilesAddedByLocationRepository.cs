using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
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
    public class MilesAddedByLocationRepository : OcppRepository<MilesAddedByLocationChartResponse>, IMilesAddedByLocationQueryRepository
    {
        private IConfiguration Configuration;
        TokenBase _tokenBase;
        public MilesAddedByLocationRepository(IConfiguration iConfig, Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token) : base(dbContext)
        {
            Configuration = iConfig;
            _tokenBase = token; 
        }

        async Task<MilesAddedByLocationChartResponse> IMilesAddedByLocationQueryRepository.GetMilesAddedByLocation(List<int> location, string duration, string chargeBoxId)
        {
            List<MilesAddedByLocationResponse> finalon = null;
            MilesAddedByLocationChartResponse obj = new MilesAddedByLocationChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                if (string.IsNullOrEmpty(duration) || duration.ToLower() == "string")
                    duration = "1";
                string callingMethoddispenser = APIConstant.GetDispenserByLocations;
                string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
                {
                    operatorid = "",
                    LocationIds = location
                });
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");

                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent,_tokenBase.acces_token);

                var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();

                dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);
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
                                                         join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                                                         on s.ChargerId equals c.DispenserId
                                                         select new ChargingSessionByLocationBO
                                                         {
                                                             Id = s.Id,
                                                             ChargerId = s.ChargerId,
                                                             StartMeterValue = s.StartMeterValue,
                                                             StartSoc = s.StartSoc,
                                                             StartTime = s.StartTime,
                                                             EndMeterValue = s.EndMeterValue,
                                                             EndSoc = s.EndSoc,
                                                             EndTime = s.EndTime,

                                                             LocationId = c.LocationId,
                                                             LocationName = c.LocationName,
                                                             LocationStatusName = c.LocationStatusName,
                                                             LocationStatusId = c.LocationStatusId,
                                                             ChargeBoxId = c.ChargeBoxId,
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

                                                             SerialNumber = c.SerialNumber,
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

                finalon = res
                .GroupBy(x => new { x.times })
                .Select(y => new MilesAddedByLocationResponse()
                {
                   // Times = y.Key.times.Length == 2 ? y.Key.times : "0" + y.Key.times,
                    svalue = y.Max(f => f.svalue),
                    Times = y.Key.times.Length >= 2 ? y.Key.times : "0" + y.Key.times,
                    RangeAdded = Math.Round((Convert.ToDouble(y.Sum(t => t.EndMeterValue)) - (Convert.ToDouble(y.Sum(t => t.StartMeterValue)) <= 0 ? 0 : Convert.ToDouble(y.Sum(t => t.StartMeterValue)))) / (100 * Convert.ToDouble(fuleeffersiancy)), 2)

                }
                ).OrderBy(t => t.svalue).ToList<MilesAddedByLocationResponse>();

               if(finalon.Count > 0)
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
    }
}
