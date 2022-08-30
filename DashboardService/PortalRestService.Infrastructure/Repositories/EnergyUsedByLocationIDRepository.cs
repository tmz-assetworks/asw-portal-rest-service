using Newtonsoft.Json;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
//using PortalRestService.Core.Responses;
using PortalRestService.Helper;
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
        public EnergyUsedByLocationIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext) : base(dbContext)
        {
        }
        async Task<EnergyUsedBOForChartResponse> IEnergyUsedByLocationIDRepository.GetEnergyUsedByLocationID(List<int> location, string duration, string chargeBoxId)
        {
            EnergyUsedBOForChartResponse obj = new EnergyUsedBOForChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                string callingMethoddispenser = APIConstant.GetDispenserByLocations;
                string dd = JsonConvert.SerializeObject(new Core.Responses.LocationOpratorRequest()
                {
                    opratorid = "",
                    LocationIds = location
                });
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethoddispenser, httpContent);
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
                List<EnergyUsedChartBO> res = (from s in _dbContext.ChargingSessions.ToList()
                                               where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now && s.EndMeterValue>0
                                               join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                                               on s.ChargerId equals c.ChargerId
                                               select new EnergyUsedChartBO
                                               {
                                                   StartMeterValue = s.StartMeterValue.Value,
                                                   EndMeterValue = s.EndMeterValue.Value,
                                                   chargeboxId = c.ChargeBoxId,
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

                if (!string.IsNullOrEmpty(chargeBoxId))
                {
                    if (res != null)
                    {
                        res = res.Where(f => f.chargeboxId ==chargeBoxId).ToList();
                    }
                }
                finalon = res
                .GroupBy(x => new { x.times })
                .Select(y => new EnergyUsedsResponse()
                {

                    svalue = y.Max(f => f.svalue),
                    times = y.Key.times.Length >= 2 ? y.Key.times : "0" + y.Key.times,
                    EndMeterValue = y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue <= 0 ? 0 : c.StartMeterValue),
                }
                ).OrderBy(t =>t.svalue).ToList<EnergyUsedsResponse>();


                obj.StatusMessage = "Record Found";
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = "Record not Found";
                obj.StatusCode = 404;
                obj.data = null;
            }

            return obj;
        }
    }
}
