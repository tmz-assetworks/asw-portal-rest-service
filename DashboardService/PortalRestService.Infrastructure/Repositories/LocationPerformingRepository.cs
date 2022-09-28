using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.EnumData;
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
    public class LocationPerformingRepository : OcppRepository<LocationPerformingChartResponse>, ILocationPerformingRepository
    {
        TokenBase _tokenBase;
        public LocationPerformingRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase tokenBase) : base(dbContext)
        {
            _tokenBase = tokenBase;
        }
        async Task<LocationPerformingChartResponse> ILocationPerformingRepository.GetLocationPerforming(List<int> location, string duration, int orderby)
        {
            List<LocationPerformingResponse> finalon = null;
            LocationPerformingChartResponse obj = new LocationPerformingChartResponse();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                var random = new Random();
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
                if (duration == "7")
                {
                    duration = "6";
                    
                }
                else
                if (duration == "30")
                {
                    duration = "28";
                   
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
                                                             times = (s.StartTime.HasValue == true ? s.StartTime.ToString() : "").Split(" ")[1].Split(":")[0].ToString(),

                                                             SerialNumber = c.SerialNumber,
                                                         }).ToList<ChargingSessionByLocationBO>();

                if (orderby > 0)
                {
                    finalon = res
                .GroupBy(x => new { x.LocationName, x.LocationStatusName })
                .Select(y => new LocationPerformingResponse()
                {
                    LocationName = y.Key.LocationName,
                    MeterValue = y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue),
                    Orderby = "TOP",
                }
                ).OrderByDescending(t => t.MeterValue).Take(5).ToList<LocationPerformingResponse>();
                }
                else
                {
                    finalon = res
                    .GroupBy(x => new { x.LocationName, x.LocationStatusId, x.LocationStatusName })
                    .Select(y => new LocationPerformingResponse()
                    {

                        LocationName = y.Key.LocationName,
                        MeterValue = y.Sum(c => c.EndMeterValue) - y.Sum(c => c.StartMeterValue),
                        Orderby = "BOTTOM",

                    }
                ).OrderBy(t => t.MeterValue).Take(5).ToList<LocationPerformingResponse>();
                }
                if (finalon != null && finalon.Count > 0)
                {
                    Dictionary<int, string> locationcolors = CommonHelpers.LocationStaticColorList();
                    int i = 1;
                    for (int j = 0; j < finalon.Count; j++)
                    {

                        finalon[j].Color = locationcolors[j + 1].Trim();
                    }
                    obj.StatusMessage = "Record Found";
                }
                else
                {
                    obj.StatusMessage = "Record not Found";
                }
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
