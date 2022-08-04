using Newtonsoft.Json;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
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
    public class GetChargerByLocationIDRepository : Repository<ChargerStatusForChartResponse>, IChargerByLocationRepository
    {
        public GetChargerByLocationIDRepository() : base()
        {

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

        
        async Task<ChargerStatusForChartResponse> IChargerByLocationRepository.GetChargerStatusByLocationID(List<int> location, string duration)
        {
            ChargerStatusForChartResponse obj = new ChargerStatusForChartResponse();
            List<ChargingSession> ChargingSessions = new List<ChargingSession>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {

                string callingMethodSession = APIConstant.GetChargerSessionAll;
                HttpResponseMessage responseSession = await Helpers.Helper.GetCallOCPPAPIAsync(callingMethodSession);

                var chargingSessions = await responseSession.Content.ReadAsStringAsync();
                ChargingSessions = JsonConvert.DeserializeObject<List<ChargingSession>>(chargingSessions);

                //string callingMethoddispenser = APIConstant.GetDispenserByLocation + location[0];
                //HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetAPIAsync(callingMethoddispenser);

                //var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();

                //dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);

                string callingMethoddispenser = APIConstant.GetDispenserByLocations;
                string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
                {
                    opratorid = "",
                    LocationIds = location
                });
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");

                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAPIAsync(callingMethoddispenser, httpContent);

                var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();

                dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);


                List<ChargerByLocationBO> res = (from s in ChargingSessions
                                                 where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                                 join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                                                 on s.ChargerId equals c.ChargerId
                                                 select new ChargerByLocationBO
                                                 {

                                                     ChargerId = s.ChargerId,
                                                     ChargingStatus = (
                                                                s.ChargingStatus.Equals("Completed") ? "Available" :
                                                                s.ChargingStatus.Equals("cancelled") ? "Available" :
                                                                s.ChargingStatus.Equals("Interrupted") ? "Faulted" : "Busy"
                                                        ),
                                                     StartTime = s.StartTime,
                                                     ChargeBoxId = c.ChargeBoxId,
                                                     times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),
                                                 }).ToList<ChargerByLocationBO>();

                                        List<ChargerByLocationChartBO> finalon = null;
                                        finalon = res
                                        .GroupBy(x => new { x.times, x.ChargingStatus })
                                        .Select(y => new ChargerByLocationChartBO()
                                        {
                                            ChargeStatus = y.Key.ChargingStatus,
                                            times = y.Key.times.Length == 2 ? y.Key.times : "0" + y.Key.times,
                                            Counts =y.ToList().Count,
                                            Color = Extensions.GetColorCodesByCharger(y.Key.ChargingStatus)
                                        }
                                        ).OrderBy(t => t.times).ThenBy(t=>t.ChargeStatus).ToList<ChargerByLocationChartBO>();


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
