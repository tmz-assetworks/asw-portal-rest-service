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
    public class EnergyUsedByLocationIDRepository : Repository<EnergyUsedBOForChartResponse>, IEnergyUsedByLocationIDRepository
    {
        public EnergyUsedByLocationIDRepository() : base()
        {

        }
        async Task<EnergyUsedBOForChartResponse> IEnergyUsedByLocationIDRepository.GetEnergyUsedByLocationID(List<int> location, string duration)
        {
            EnergyUsedBOForChartResponse obj = new EnergyUsedBOForChartResponse();
            
            List<ChargingSession> ChargingSessions = new List<ChargingSession>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {

                string callingMethodSession = APIConstant.GetChargerSessionAll;
                HttpResponseMessage responseSession = await Helpers.Helper.GetCallOCPPAPIAsync(callingMethodSession);

                var chargingSessions = await responseSession.Content.ReadAsStringAsync();
                ChargingSessions = JsonConvert.DeserializeObject<List<ChargingSession>>(chargingSessions);

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

                List<EnergyUsedChartBO> res = (from s in ChargingSessions
                                               where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now && s.EndMeterValue>0
                                               join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                                               on s.ChargerId equals c.ChargerId
                                               select new EnergyUsedChartBO
                                               {
                                                   StartMeterValue = s.StartMeterValue.Value,
                                                   EndMeterValue = s.EndMeterValue.Value,
                                                   times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),
                                               }).ToList<EnergyUsedChartBO>();

                List<EnergyUsedsResponse> finalon = null;
                finalon = res
                .GroupBy(x => new { x.times })
                .Select(y => new EnergyUsedsResponse()
                {
                    times = y.Key.times.Length == 2 ? y.Key.times : "0"+ y.Key.times,
                    EndMeterValue = y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue <= 0 ? 0 : c.StartMeterValue),
                }
                ).OrderBy(t =>t.times).ToList<EnergyUsedsResponse>();


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
