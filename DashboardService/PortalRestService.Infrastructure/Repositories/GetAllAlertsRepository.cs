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
using System.Diagnostics.Metrics;
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
        public async Task<OperatorAlertResponse> GetAllAlertsOld(OperatorAlertRequest operatorAlertRequest)
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

		public async Task<OperatorAlertResponse> GetAllAlertsold2(OperatorAlertRequest operatorAlertRequest)
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
			int Rcount = 0;
            int tcount = 0;
            string ObjectId = _tokenBase.getObjectId();
            TaskCount _taskCount = new TaskCount();
			try
			{
				var allchargerBoxIds = (from charger in _dbContext.Charger
										join locations in operatorAlertRequest.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => operatorAlertRequest.LocationIds.Contains((int)x.Id)) : _dbContext.Locations on charger.LocationId equals locations.Id
										join address in _dbContext.LocationAddress on locations.LocationAddressId equals address.Id
										join Status in _dbContext.LocationStatus on locations.LocationStatusId equals Status.Id
										join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId.Equals(_tokenBase.getObjectId())).FirstOrDefault().Id))
										on locations.Id equals userMap.LocationId
										select charger.ChargeBoxId

				   ).Distinct().ToList();
				if (allchargerBoxIds.Count > 0)
				{
					List<string> list = new List<string>();
					if (operatorAlertRequest.chargerBoxIds.Count > 0)
					{
						list = operatorAlertRequest.chargerBoxIds.Intersect(allchargerBoxIds).ToList();
						operatorAlertRequest.chargerBoxIds.Clear();
					}
                    else
                    {
                        list = allchargerBoxIds;
					}
					operatorAlertRequest.chargerBoxIds = list;
					int countr = operatorAlertRequest.PageNumber * operatorAlertRequest.PageSize;
                    if(operatorAlertRequest.chargerBoxIds.Count>0)
                    {
						if (!string.IsNullOrEmpty(operatorAlertRequest.SearchParam))
						{
							Rcount = (from s in _dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId)).Where(d => d.RequestType.ToLower().Contains(operatorAlertRequest.SearchParam.ToLower()))
									  select s).Count();
							tcount = (from s in _dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId)).Where(d => d.RequestType.ToLower().Contains(operatorAlertRequest.SearchParam.ToLower())).Where(d => d.IsRead == false)
									  select s).Count();
						}
						else
						{
							Rcount = (from s in _dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId) && o.RequestType != "Heartbeat")
									  select s).Count();
							tcount = (from s in _dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId) && o.RequestType != "Heartbeat" && o.IsRead == false)
                                      select s).Count();
						}
						res = (from s in string.IsNullOrEmpty(operatorAlertRequest.SearchParam) ? _dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId) && o.RequestType != "Heartbeat").OrderByDescending(o => o.Id).Take(countr).ToList() : _dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId)).Where(d => d.RequestType.ToLower().Contains(operatorAlertRequest.SearchParam.ToLower())).OrderByDescending(o => o.Id).Take(countr).ToList()
							   join charger in _dbContext.Charger on s.DeviceId equals charger.ChargeBoxId
							   join locations in operatorAlertRequest.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => operatorAlertRequest.LocationIds.Contains((int)x.Id)) : _dbContext.Locations on charger.LocationId equals locations.Id
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
								   IsRead = s.IsRead == false ? false : true,
								   Flag = "OCPP",
                                   AssetId =charger.AssetId

							   }).OrderByDescending(a => a.DateTime).DistinctBy(d => d.EventLogId).Where(r => r.ChargeBoxId != null).ToList<AlertResponse>();
					}
				}
				
				tasknotification = (from s in _dbContext.TaskNotifications.Where(d=> d.UserId.Equals(_tokenBase.getObjectId()))
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
										UserId = s.UserId == null ? "" : s.UserId,
										Flag = "ASSET",
                                        AssetId =""

									}).Distinct().OrderByDescending(a => a.DateTime)
			   //.Where(e=>e.UserId.Equals(_tokenBase.getObjectId()))
			   .ToList<AlertResponse>();
				Rcount= Rcount+ tasknotification.Count;
				res.AddRange(tasknotification);
				if (res != null)
				{
                    _taskCount = (new TaskCount() { Counts = tasknotification.Where(s => s.IsRead == false).Count() + tcount });
				}

				if (res != null && res.Count > 0)
				{
					res = res != null ? res.OrderByDescending(a => a.DateTime).ToList() : res;
					if (!string.IsNullOrEmpty(operatorAlertRequest.SearchParam))
						res = res.Where(d => d.MessageType.ToLower().Contains(operatorAlertRequest.SearchParam.ToLower())
					 ).OrderByDescending(a => a.DateTime).ToList<AlertResponse>();
					//Paging on Records           

					var dataResult = PagedList<AlertResponse>.ToPageList(res,
				   operatorAlertRequest.PageNumber,
				  operatorAlertRequest.PageSize,Rcount);
					if (res.Count > 0)
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
				operatorAlertResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;

				operatorAlertResponse.data = new List<AlertResponse>();
			}
			return operatorAlertResponse;
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
            string TokenObjectId = _tokenBase.getObjectId();
            List<AlertResponse>? res = new List<AlertResponse>();
            List<AlertResponse>? tasknotification = new List<AlertResponse>();
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();

            TaskCount _taskCount = new TaskCount();
            try
            {
                res = (from s in operatorAlertRequest.chargerBoxIds.Count > 0 ? (_dbContext.OcppEventLogs.Where(o => operatorAlertRequest.chargerBoxIds.Contains(o.DeviceId) && o.RequestType == "StatusNotification" && o.CreatedAt>= DateTime.Now.AddDays(-30)).Where(o => o.RequestPayload.Contains("SuspendedEVSE") || o.RequestPayload.Contains("Faulted") || o.RequestPayload.Contains("Unavailable")))
                        : _dbContext.OcppEventLogs.Where(o => o.RequestType == "StatusNotification" && o.CreatedAt >= DateTime.Now.AddDays(-30)).Where(o => o.RequestPayload.Contains("SuspendedEVSE") || o.RequestPayload.Contains("Faulted") || o.RequestPayload.Contains("Unavailable"))
                       join charger in _dbContext.Charger on s.DeviceId equals charger.ChargeBoxId
                       join locations in operatorAlertRequest.LocationIds.Count > 0 ? _dbContext.Locations.Where(x => operatorAlertRequest.LocationIds.Contains((int)x.Id)) : _dbContext.Locations on charger.LocationId equals locations.Id
                       //join address in _dbContext.LocationAddress on locations.LocationAddressId equals address.Id
                       //join Status in _dbContext.LocationStatus on locations.LocationStatusId equals Status.Id
                       join userMap in _dbContext.OperatorUserMapper.Where(x => x.UserId == (_dbContext.Users.Where(z => z.ObjectId== TokenObjectId).FirstOrDefault().Id))
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
                           IsRead = s.IsRead == false ? false : true,
                           Flag = "OCPP"


                       }).OrderByDescending(a => a.DateTime).Distinct().Where(r => r.ChargeBoxId != null).ToList<AlertResponse>();

                tasknotification = (from s in _dbContext.TaskNotifications.Where(d=> d.CreatedAt>=DateTime.Now.AddDays(-30))
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
                                        UserId = s.UserId == null ? "" : s.UserId,
                                        Flag = "ASSET"

                                    }).Distinct().OrderByDescending(a => a.DateTime)
               .Where(e=>e.UserId== TokenObjectId)
               .ToList<AlertResponse>();

                res.AddRange(tasknotification);
                if (res != null)
                {
                    _taskCount = (new TaskCount() { Counts = res.Where(s => s.IsRead == false).Count() });
                }


                if (res != null && res.Count > 0)
                {
                    res = res != null ? res.OrderByDescending(a => a.DateTime).ToList() : res;
                    if (!string.IsNullOrEmpty(operatorAlertRequest.SearchParam))
                        res = res.Where(d => d.MessageType.ToLower().Contains(operatorAlertRequest.SearchParam.ToLower())
                     ).OrderByDescending(a => a.DateTime).ToList<AlertResponse>();
                    //Paging on Records           

                    var dataResult = PagedList<AlertResponse>.ToPagedList(res,
                   operatorAlertRequest.PageNumber,
                  operatorAlertRequest.PageSize);
                    if (res.Count > 0)
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
                operatorAlertResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;

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
