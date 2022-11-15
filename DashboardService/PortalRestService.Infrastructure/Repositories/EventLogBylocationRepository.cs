using Microsoft.Data.SqlClient;
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
    public class EventLogBylocationRepository : OcppRepository<EventLogLocationResponse>, IEventLogByLocationRepository
    {
        //readonly SqlConnection  conn;
        TokenBase _tokenBase;
        public EventLogBylocationRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token) : base(dbContext)
        {
            _tokenBase=token;
        }
       async Task<PagedList<EventLogLocation>> IEventLogByLocationRepository.GetEventLogByLocation(EventLogRequest request)
        {
           // EventLogLocationResponse EventLogLocationres = new EventLogLocationResponse();

            List<EventLogLocation> res = new List<EventLogLocation>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {

                if (request.LocationIds != null && request.LocationIds.Count() > 0)
                {
                    string callingMethoddispensers = APIConstant.GetDispenserByLocations;
                    string locationRequest = JsonConvert.SerializeObject(new LocationOpratorRequest()
                    {
                        operatorid = "",
                        LocationIds = request.LocationIds
                    });
                   
                }
                string eventlogre = JsonConvert.SerializeObject(new OcppEventLogRequest()
                {
                    chargerboxid = request.ChargerBoxIds
                });
                StringContent httpContenteventlog = new StringContent(eventlogre, Encoding.UTF8, "application/json");
                //string EventLogMethodName = APIConstant.GetEventLogByLocationAll;
                //HttpResponseMessage responseSession = await Helpers.Helper.GetCallOCPPWithBodyAPIAsync(EventLogMethodName, httpContenteventlog);

                //var EventLogData = await responseSession.Content.ReadAsStringAsync();
                //EventLogLocationres = JsonConvert.DeserializeObject<EventLogLocationResponse>(EventLogData);

                string callingMethoddispenser = APIConstant.GetDispenserByLocations;
                string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
                {
                    operatorid = "",
                    LocationIds = request.LocationIds
                });
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent,_tokenBase.acces_token);

                var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();

                dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);
                if (dispenserByLocationIdResponse.data.Count > 0)
                {
                    res = (from s in request.ChargerBoxIds.Count > 0 ? _dbContext.OcppEventLogs.ToList().Where(o => request.ChargerBoxIds.Contains(o.DeviceId) && o.DeviceId!=null) : _dbContext.OcppEventLogs.ToList().Where(o =>o.DeviceId != null)

                           join c in dispenserByLocationIdResponse.data.ToList()
                                      on s.DeviceId.ToLower() equals c.ChargeBoxId.ToLower()
                           select new EventLogLocation
                           {
                               Id = s.Id,
                               CreatedAt = s.CreatedAt,
                               DeviceId = s.DeviceId,
                               EventLogDataSource = s.EventLogDataSource,
                               ModifiedAt = s.ModifiedAt,
                               RequestId = s.RequestId,
                               RequestPayload = s.RequestPayload == null ? "" : s.RequestPayload.Replace(",", ",\r\n"),
                               RequestType = s.RequestType,
                               ResponsePayload = s.ResponsePayload == null ? "" : s.ResponsePayload.Replace(",", ",\r\n"),
                               LocationId = c.LocationId.ToString(),
                               LocationName = c.LocationName,
                               RequestTypeColor = Extensions.GetEventlogColorCodes(s.RequestType==null?"": s.RequestType),
                               IsRead= s.IsRead.HasValue == true ? s.IsRead.Value :false 
                           }).DistinctBy(d=>d.Id).Where(s => s.DeviceId!=null ).ToList();

                }
            }
            catch (Exception ex)
            {
                
            }

            res = res != null ? res.OrderByDescending(a => a.ModifiedAt).ToList() : res;
            if (!string.IsNullOrEmpty(request.SearchParam))
                res = res.Where(d => d.RequestType.ToLower().StartsWith( request.SearchParam.ToLower()) || d.DeviceId.ToLower()== request.SearchParam.ToLower()).ToList();

            var dataResult = PagedList<EventLogLocation>.ToPagedList(res,
              request.PageNumber,
              request.PageSize);
            return await Task.FromResult(dataResult);

        }

    }
}
