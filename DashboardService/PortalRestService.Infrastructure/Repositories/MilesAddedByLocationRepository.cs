using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.Repositories;
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
    public class MilesAddedByLocationRepository : Repository<MilesAddedByLocationChartResponse>, IMilesAddedByLocationQueryRepository
    {
        private IConfiguration Configuration;
        public MilesAddedByLocationRepository(IConfiguration iConfig) : base()
        {
            Configuration = iConfig;
        }

        async Task<MilesAddedByLocationChartResponse> IMilesAddedByLocationQueryRepository.GetMilesAddedByLocation(List<int> location, string duration)
        {
            List<MilesAddedByLocationResponse> finalon = null;
            MilesAddedByLocationChartResponse obj = new MilesAddedByLocationChartResponse();
            List<ChargingSession> ChargingSessions = new List<ChargingSession>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                if (string.IsNullOrEmpty(duration) || duration.ToLower() == "string")
                    duration = "1";


                string callingMethodSession = APIConstant.GetChargerSessionAll;
                HttpResponseMessage responseSession = await Helpers.Helper.GetCallOCPPAPIAsync(callingMethodSession);

                var chargingSessions = await responseSession.Content.ReadAsStringAsync();
                ChargingSessions = JsonConvert.DeserializeObject<List<ChargingSession>>(chargingSessions);

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

                List<ChargingSessionByLocationBO> res = (from s in ChargingSessions
                                                         where s.StartTime >= DateTime.Now.AddDays(-Convert.ToInt32(duration)) && s.StartTime <= DateTime.Now
                                                         join c in dispenserByLocationIdResponse.data.ToList<DispenserByLocation>()
                                                         on s.ChargerId equals c.ChargerId
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
                                                             times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),

                                                             SerialNumber = c.SerialNumber,
                                                         }).ToList<ChargingSessionByLocationBO>();

                string fuleeffersiancy = this.Configuration.GetSection("Variable")["fuleeffersiancy"];

                finalon = res
                .GroupBy(x => new { x.times })
                .Select(y => new MilesAddedByLocationResponse()
                {
                    Times = y.Key.times.Length == 2 ? y.Key.times : "0" + y.Key.times,
                    RangeAdded = (Math.Round(Convert.ToDouble(y.Sum(t => t.EndMeterValue)) - (Convert.ToDouble(y.Sum(t => t.StartMeterValue) <= 0 ? 0 : Convert.ToDouble(y.Sum(t => t.StartMeterValue))) / (100 * Convert.ToDouble(fuleeffersiancy))), 2))

                }
                ).OrderBy(t => t.Times).ToList<MilesAddedByLocationResponse>();

               if(finalon.Count > 0)
                obj.StatusMessage = "Record Found";
                else
                    obj.StatusMessage = "Record not Found";
                obj.StatusCode = 200;
                obj.data = finalon;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = "Failed!";
                obj.StatusCode = 404;
                obj.data = null;
            }
            return obj;
        }
    }
}
