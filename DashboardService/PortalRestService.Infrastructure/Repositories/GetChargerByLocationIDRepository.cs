using Newtonsoft.Json;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
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
#pragma warning disable
    public class GetChargerByLocationIDRepository : OcppRepository<ChargerStatusForChartResponse>, IChargerByLocationRepository
    {
        TokenBase _tokenBase;
        public GetChargerByLocationIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase=token;
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


        async Task<ChargerStatusForChartResponse> IChargerByLocationRepository.GetChargerStatusByLocationID(List<int> location, string duration, string chargeBoxId)
        {
            ChargerStatusForChartResponse obj = new ChargerStatusForChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
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


                List<ChargerByLocationBO> res = (from s in _dbContext.ChargingSessions.ToList()
                                                 where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                                 join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                           on s.ChargerId equals c.DispenserId
                                                 select new ChargerByLocationBO
                                                 {

                                                     ChargerId = s.ChargerId,
                                                     ChargingStatus = (
                                                                s.ChargingStatus.ToLower().Equals("completed") ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("cancelled") ? "Available" :
                                                                s.ChargingStatus.ToLower().Equals("interrupted") ? "Faulted" : "Busy"
                                                        ),
                                                     StartTime = s.StartTime,
                                                     ChargeBoxId = c.ChargeBoxId,
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
                                 if (!string.IsNullOrEmpty(chargeBoxId))
                                 {
                                     if (res != null)
                                     {
                                         res = res.Where(f => f.ChargeBoxId == chargeBoxId).ToList();
                                     }
                                 }
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


                obj.StatusMessage = "Record Found";
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = "Operation Failed!";
                obj.StatusCode = 404;
                obj.data = null;
            }

            return obj;
        }
    }
}
