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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class GetAllAlertsRepository : OcppRepository<EventLogLocationResponse>, IGetAllAlertsRepository
    {
        TokenBase _tokenBase;
        public GetAllAlertsRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase = token;
        }
        public async Task<OperatorAlertResponse> GetAllAlerts(OperatorAlertRequest operatorAlertRequest)
        {
            OperatorAlertResponse operatorAlertResponse = new OperatorAlertResponse();
            var emptyvalue = AlertValidation(operatorAlertRequest);
            if (!string.IsNullOrEmpty(emptyvalue))
            {
                operatorAlertResponse.StatusMessage = emptyvalue;
                operatorAlertResponse.StatusCode = 400;
                return operatorAlertResponse;
            }
            var PaginationValidation = Helper.CommonHelpers.PagenationValidation(operatorAlertRequest);
            if (!string.IsNullOrEmpty(PaginationValidation))
            {
                operatorAlertResponse.StatusMessage = PaginationValidation;
                operatorAlertResponse.StatusCode = 400;
                return operatorAlertResponse;
            }
            //EventLogLocationResponse eventLogLocationResponse = new EventLogLocationResponse();

            List<AlertResponse> res = new List<AlertResponse>();
            DispenserByLocationIdResponse dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            try
            {
                List<string> str = new List<string>();

                //string eventlogre = JsonConvert.SerializeObject(new OcppEventLogRequest()
                //{
                //    chargerboxid = operatorAlertRequest.chargerBoxIds
                //});
                //StringContent httpContenteventlog = new StringContent(eventlogre, Encoding.UTF8, "application/json");
                //string EventLogMethodName = APIConstant.GetEventLogByLocationAll;
                //HttpResponseMessage responseSession = await Helpers.Helper.GetCallOCPPWithBodyAPIAsync(EventLogMethodName, httpContenteventlog);  // TODO

                //var EventLogData = await responseSession.Content.ReadAsStringAsync();
                //eventLogLocationResponse = JsonConvert.DeserializeObject<EventLogLocationResponse>(EventLogData);

                string callingMethoddispenser = APIConstant.GetDispenserByLocations;
                string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
                {
                    operatorid = operatorAlertRequest.operatorId,
                    LocationIds = operatorAlertRequest.LocationIds
                });
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent,_tokenBase.acces_token);

                var DispenserByLocation = await responsedispenser.Content.ReadAsStringAsync();

                dispenserByLocationIdResponse = JsonConvert.DeserializeObject<DispenserByLocationIdResponse>(DispenserByLocation);

                  res = (from s in operatorAlertRequest.chargerBoxIds.Count > 0 ? _dbContext.OcppEventLogs.ToList().Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId) && o.DeviceId != null) : _dbContext.OcppEventLogs.ToList().Where(o => o.DeviceId != null)

                       join c in dispenserByLocationIdResponse.data.ToList()
                                  on s.DeviceId.ToLower() equals c.ChargeBoxId.ToLower()
                       select new AlertResponse
                           {
                               EventLogId = s.Id,
                               ChargeBoxId = c.ChargeBoxId,
                               Category = "OCPP",
                               MessageType = s.RequestType,
                               DateTime = s.CreatedAt,
                               IPAddress = "192.168.0.1",
                               LocationsName = c.LocationName,
                               RequestPayload = s.RequestPayload == null ? "" : s.RequestPayload.Replace(",", ",\r\n"),
                               ResponsePayload = s.ResponsePayload == null ? "" : s.ResponsePayload.Replace(",", ",\r\n")

                           }).DistinctBy(d => d.EventLogId).Where(r => r.ChargeBoxId != null).ToList<AlertResponse>();
               

                if (res != null && res.Count > 0)
                {
                    res = res != null ? res.OrderByDescending(a => a.DateTime).ToList() : res;
                    if (!string.IsNullOrEmpty(operatorAlertRequest.SearchParam))
                        res = res.Where(d => d.MessageType.ToLower().Contains( operatorAlertRequest.SearchParam.ToLower())
                     ).ToList<AlertResponse>();
                    //Paging on Records           

                    var dataResult = PagedList<AlertResponse>.ToPagedList(res,
                   operatorAlertRequest.PageNumber,
                  operatorAlertRequest.PageSize);
                    if (res.Count>0)
                    {
                        operatorAlertResponse.StatusMessage = "Record Found";
                        operatorAlertResponse.StatusCode = 200;
                        operatorAlertResponse.data = dataResult;
                        operatorAlertResponse.paginationResponse = new PaginationResponse()
                        {
                            TotalCount = dataResult.TotalCount,
                            PageSize = dataResult.PageSize,
                            CurrentPage = dataResult.CurrentPage,
                            TotalPages = dataResult.TotalPages,
                            HasNext = dataResult.HasNext,
                            HasPrevious = dataResult.HasPrevious
                        };
                    }
                    else
                    {
                        operatorAlertResponse.StatusCode = 200;
                        operatorAlertResponse.StatusMessage = "Record not Found";
                    }
                }
                else
                {
                    operatorAlertResponse.StatusCode = 404;
                    operatorAlertResponse.StatusMessage = "Record not Found";
                }
            }
            catch (Exception ex)
            {
                operatorAlertResponse.StatusMessage = "Failed!";
                operatorAlertResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed ;
                operatorAlertResponse.data = null;
            }
            return operatorAlertResponse;
        }

        public string AlertValidation(OperatorAlertRequest operatorAlertRequest)
        {
            string checkemptys;
            if (operatorAlertRequest.LocationIds != null && operatorAlertRequest.LocationIds.Count() > 0)
            {
                bool isNotValid = false;
                for (int i = 0; i < operatorAlertRequest.LocationIds.Count; i++)
                {
                    if (operatorAlertRequest.LocationIds[i] < 0)
                    {
                        isNotValid = true;
                        break;
                    }
                }
                if (isNotValid == true)
                {
                    return "Please check locationId!";
                }

            }

            if (operatorAlertRequest.chargerBoxIds != null && operatorAlertRequest.chargerBoxIds.Count() > 0)
            {
                string checkempty;
                for (int i = 0; i < operatorAlertRequest.chargerBoxIds.Count; i++)
                {
                    checkempty = operatorAlertRequest.chargerBoxIds[i];

                    if (!string.IsNullOrEmpty(checkempty))
                    {

                        if (checkempty.Trim() == "")
                        {

                            return "Please check checkBoxID!";

                        }
                        break;
                    }
                }
            }
            if (operatorAlertRequest.SearchParam != null)
            {
                bool isNotValid = false;
                if (!string.IsNullOrEmpty(operatorAlertRequest.SearchParam))
                {
                    if (operatorAlertRequest.SearchParam.Trim() == "")
                    {
                        return "Please check Searchpermcolumn!";
                    }
                }
            }
            return string.Empty;

        }
    }
}
