using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalRestService.Core.ConstantResponse;
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
        private readonly IConfiguration _configuration;
        private readonly string OccpIp = String.Empty;
        public GetAllAlertsRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token, IConfiguration configuration) : base(dbContext)
        {
            _tokenBase = token;
            this._configuration = configuration;
            OccpIp = this._configuration.GetSection("OccpIp").GetSection("ip").Value;
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

            List<AlertResponse>? res = new List<AlertResponse>();
            List<AlertResponse>? tasknotification = new List<AlertResponse>();
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            
            TaskCount _taskCount = new TaskCount(); 
            try
            {
                res = (from s in operatorAlertRequest.chargerBoxIds.Count > 0 ? _dbContext.OcppEventLogs.ToList().Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId) && o.DeviceId != null) : _dbContext.OcppEventLogs.ToList().Where(o => o.DeviceId != null)
                       join charger in _dbContext.Charger on s.DeviceId equals charger.ChargeBoxId
                       join locations in operatorAlertRequest.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => operatorAlertRequest.LocationIds.Contains((int)x.Id)) : _dbContext.Locations on charger.LocationId equals locations.Id
                       join address in _dbContext.LocationAddress on locations.LocationAddressId equals address.Id
                       join Status in _dbContext.LocationStatus on locations.LocationStatusId equals Status.Id
                       join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
                       on locations.Id equals userMap.LocationId
                       select new AlertResponse
                       {
                           EventLogId = s.Id,
                           ChargeBoxId = charger.ChargeBoxId,
                           Category = "OCPP",
                           MessageType = s.RequestType,
                           DateTime = s.CreatedAt,
                           IPAddress = OccpIp == null ? "" : OccpIp,
                           LocationsName = locations.LocationName,
                           RequestPayload = s.RequestPayload == null ? "" : s.RequestPayload.Replace(",", ",\r\n"),
                           ResponsePayload = s.ResponsePayload == null ? "" : s.ResponsePayload.Replace(",", ",\r\n"),
                           IsRead = s.IsRead == false ? false:true,
                           Flag="OCPP"
                           

                           }).OrderByDescending(a => a.DateTime).DistinctBy(d => d.EventLogId).Where(r => r.ChargeBoxId != null).ToList<AlertResponse>();

                            tasknotification = (from s in _dbContext.TaskNotifications
                           select new AlertResponse
                           {
                               EventLogId = s.Id,
                               ChargeBoxId = "",
                               Category = s.Category,
                               MessageType = s.Messagetype,
                               DateTime = s.CreatedAt,
                               IPAddress = s.Ipaddress == null ? "" : s.Ipaddress,
                               LocationsName = "",
                               RequestPayload = "",
                               ResponsePayload = s.Content,
                               IsRead = s.IsRead == false ? false : true,
                               UserId = s.UserId==null?"": s.UserId,
                               Flag="ASSET"
                                
                           }).Distinct().OrderByDescending(a => a.DateTime)
                           //.Where(e=>e.UserId.Equals(_tokenBase.getObjectId()))
                           .ToList<AlertResponse>();

                res.AddRange(tasknotification);
                if (res != null)
                {
                    _taskCount =(new TaskCount() {Counts = res.Where(s => s.IsRead==false).Count() });
                }
                

                if (res != null && res.Count > 0)
                {
                    res = res != null ? res.OrderByDescending(a => a.DateTime).ToList() : res;
                    if (!string.IsNullOrEmpty(operatorAlertRequest.SearchParam))
                        res = res.Where(d => d.MessageType.ToLower().Contains( operatorAlertRequest.SearchParam.ToLower())
                     ).OrderByDescending(a => a.DateTime).ToList<AlertResponse>();
                    //Paging on Records           

                    var dataResult = PagedList<AlertResponse>.ToPagedList(res,
                   operatorAlertRequest.PageNumber,
                  operatorAlertRequest.PageSize);
                    if (res.Count>0)
                    {
                        operatorAlertResponse.StatusMessage = RespnoseMessage.Record_found;
                        operatorAlertResponse.StatusCode = 200;
                        operatorAlertResponse.data = dataResult;
                        operatorAlertResponse.TaskCount = _taskCount;
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
                        operatorAlertResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    }
                }
                else
                {
                    operatorAlertResponse.StatusCode = 404;
                    operatorAlertResponse.StatusMessage = RespnoseMessage.Record_not_found;
                }
            }
            catch (Exception ex)
            {
                operatorAlertResponse.StatusMessage = RespnoseMessage.Faild;
                operatorAlertResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed ;

                operatorAlertResponse.data = new List<AlertResponse>();
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
