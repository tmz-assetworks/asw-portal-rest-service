using Newtonsoft.Json;
using PortalRestService.Core.PagingHelper;
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
    public class ChargerSessionDetailsListRepository : OcppRepository<ChargerSessionDetailsListResponse>, IGetChargerSessionDetailsListRepository
    {
        TokenBase _tokenBase;
        public ChargerSessionDetailsListRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase=token;
        }

        async Task<PagedList<ChargerSessionDetailsList>> IGetChargerSessionDetailsListRepository.GetChargerSessionDetailsList(ChargerSessionListRequest request)
        {
            List<ChargerSessionDetailsList> ChargingSessionslist = new List<ChargerSessionDetailsList>();
            List<ChargerSessionDetailsList> res = new List<ChargerSessionDetailsList>();
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();


            string eventlogre = JsonConvert.SerializeObject(new OcppEventLogRequest()
            {

                chargerboxid = request.chargerboxid
            });
           
            List<int> myList = new List<int>();
            string callingMethoddispenser = APIConstant.GetDispenserByLocations;
            string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
            {
                operatorid = "",
                LocationIds = myList
            });
            StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");

            HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent, _tokenBase.acces_token);

            var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();

            dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);


            string zero = "0000000000";
            if (!string.IsNullOrEmpty(request.Fromdate) || !string.IsNullOrEmpty(request.Todate) || request.status.Count > 0)
            {
                res = (from c in request.chargerboxid.Count > 0 ? _dbContext.ChargingSessions.ToList().Where(o => request.chargerboxid.Contains(o.DeviceId) && o.DeviceId != null) : _dbContext.ChargingSessions.ToList().Where(o => o.DeviceId != null)

                       join s in dispenserByLocationIdResponse.data.ToList()
                                  on c.ChargerId equals s.DispenserId
                       select new ChargerSessionDetailsList
                       {
                           Id = c.Id,
                           Duration = "",
                           Sessionid = zero.Substring(0,(10- c.Id.ToString().Length))+ c.Id.ToString(),
                           ChargingStatus = c.ChargingStatus,
                           Usage = (Convert.ToDouble(c.EndMeterValue) - Convert.ToDouble(c.StartMeterValue <= 0 ? 0 : c.StartMeterValue)),
                           StartTime = c.StartTime,
                           EndTime = c.EndTime,
                           ChargeBoxId = c.DeviceId,
                           ModifiedAt = c.ModifiedAt,
                           CreatedAt = c.CreatedAt,
                           Startmetervalue=c.StartMeterValue,
                           Endmetervalue=c.EndMeterValue,
                           Startsoc=c.StartSoc,
                           EndSoc=c.EndSoc,
                         
                           ReasoneForStop=c.ReasonForStop
                           
                       }).DistinctBy(d => d.Id).Where(s => s.ChargeBoxId != null).ToList();
                if (res != null)
                {
                    if (!string.IsNullOrEmpty(request.Fromdate))
                    {
                        res = res.Where(o => o.StartTime >= Convert.ToDateTime(request.Fromdate) && o.EndTime <= Convert.ToDateTime(request.Todate)).ToList();
                    }
                    if (request.status.Count > 0)
                    {
                        res = res.Where(o => request.status.Contains(o.ChargingStatus)).ToList();
                    }
                }

            }
            else
            {
                res = (from c in request.chargerboxid.Count > 0 ? _dbContext.ChargingSessions.ToList().Where(o => request.chargerboxid.Contains(o.DeviceId) && o.DeviceId != null) : _dbContext.ChargingSessions.ToList().Where(o => o.DeviceId != null)

                       join s in dispenserByLocationIdResponse.data.ToList()
                                  on c.ChargerId equals s.DispenserId
                       select new ChargerSessionDetailsList
                       {
                           Id = c.Id,
                           Duration = "",
                           Sessionid = zero.Substring(0,(10- c.Id.ToString().Length))+ c.Id.ToString(),
                           Usage = (Convert.ToDouble(c.EndMeterValue) - Convert.ToDouble(c.StartMeterValue <= 0 ? 0 : c.StartMeterValue)),
                           StartTime = c.StartTime,
                           EndTime = c.EndTime,
                           ChargingStatus = c.ChargingStatus,
                           ChargeBoxId = c.DeviceId,
                           ModifiedAt = c.ModifiedAt,
                           CreatedAt = c.CreatedAt,
                           Startmetervalue = c.StartMeterValue,
                           Endmetervalue = c.EndMeterValue,
                           Startsoc = c.StartSoc,
                           EndSoc = c.EndSoc,
                           ReasoneForStop = c.ReasonForStop
                       }).DistinctBy(d => d.Id).Where(s => s.ChargeBoxId != null).ToList();
            }


            if (res == null)
            {
                res = new List<ChargerSessionDetailsList>();
            }
            if (res.Count > 0)
            {
                foreach (var s in res)
                {

                    if (s.EndTime.HasValue && s.StartTime.HasValue)
                    {
                        System.TimeSpan diff1 = (TimeSpan)(s.EndTime - s.StartTime);
                        int total_seconds = (int)diff1.TotalSeconds;
                        int hours = total_seconds / (60 * 60);
                        int remaining_seconds = total_seconds - hours * (60 * 60);
                        int minutes = remaining_seconds / 60;
                        int seconds = remaining_seconds % 60;

                        s.Duration = string.Format("{0:#00}:{1:#00}:{2:#00}", hours, minutes, seconds);
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchParam))
                    res = res.Where(d => d.ChargeBoxId.ToLower() == request.SearchParam.ToLower()).ToList();

                
            }
            var dataResult = PagedList<ChargerSessionDetailsList>.ToPagedList(res,
              request.PageNumber,
              request.PageSize);
            return await Task.FromResult(dataResult);





        }

    }
}
    
