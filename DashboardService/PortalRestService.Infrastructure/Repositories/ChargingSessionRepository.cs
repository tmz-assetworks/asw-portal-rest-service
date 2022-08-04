using Newtonsoft.Json;
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
    public class ChargingSessionRepository : Repository<ChargerSessionByLocationResponse>, IChargingSessionRepository
    {
        public ChargingSessionRepository() : base()
        {

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
        async Task<ChargingSessionByLocationForChartResponse> IChargingSessionRepository.GetChargerSession(List<int> location, string duration)
        {

            ChargingSessionByLocationForChartResponse obj = new ChargingSessionByLocationForChartResponse();
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
                                                             LocationId = c.LocationId,
                                                             LocationName = c.LocationName,
                                                             ContactPersonName = c.ContactPersonName,
                                                             AddressLine1 = c.AddressLine1,
                                                             LocationStatusName = c.LocationStatusName,
                                                             LocationStatusId = c.LocationStatusId,
                                                             ChargeBoxId = c.ChargeBoxId,
                                                             times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),

                                                             SerialNumber = c.SerialNumber,
                                                         }).ToList<ChargingSessionByLocationBO>();

                List<ChargingSessionByLocationChartBO> finalon = null;
                finalon = res
                .GroupBy(x => new { x.times, x.ChargingStatus })
                .Select(y => new ChargingSessionByLocationChartBO()
                {
                    ChargingStatus = y.Key.ChargingStatus,
                    times = y.Key.times.Length == 2 ? y.Key.times : "0" + y.Key.times,
                    Counts = y.ToList().Count,
                    Color=Extensions.GetColorCodesByChargingSession(y.Key.ChargingStatus)
                }
                ).OrderBy(t => (t.times, t.ChargingStatus)).ToList<ChargingSessionByLocationChartBO>();



                if(finalon.Count>0)
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

