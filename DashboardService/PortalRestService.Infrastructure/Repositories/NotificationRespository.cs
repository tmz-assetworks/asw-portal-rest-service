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
    public class NotificationRespository : OcppRepository<NotificationResponse>, INotificationRepository
    {
        TokenBase _tokenBase;
        private readonly IConfiguration _configuration;
        private readonly string OccpIp = String.Empty;
        public NotificationRespository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token, IConfiguration configuration) : base(dbContext)
        {
            _tokenBase = token;
            this._configuration = configuration;
            OccpIp = this._configuration.GetSection("OccpIp").GetSection("ip").Value;
        }

        public async Task<NotificationResponse> GetNotificationCountsByUserid(NotificationRequest notificationRequest)
        {
            List<AlertResponse>? res = new List<AlertResponse>();
            DispenserByLocationIdResponse? dispenserByLocationIdResponse = new DispenserByLocationIdResponse();
            NotificationResponse notificationResponse = new NotificationResponse();
          
            int Notificationcount=0;
            string listobj = JsonConvert.SerializeObject(new LocationOpratorRequest()
            {
                operatorid = notificationRequest.UserId,
                LocationIds = new List<int>()
            });
            try
            {
                OperatorAlertRequest operatorAlertRequest=new OperatorAlertRequest();
                operatorAlertRequest.LocationIds = new List<int>();
                operatorAlertRequest.operatorId = "";
                operatorAlertRequest.chargerBoxIds = new List<string>();
                List<string> str = new List<string>();

                string callingMethoddispenser = APIConstant.GetDispenserByLocations;
                string dd = JsonConvert.SerializeObject(new LocationOpratorRequest()
                {
                    operatorid = operatorAlertRequest.operatorId,
                    LocationIds = operatorAlertRequest.LocationIds
                });
                StringContent httpContent = new StringContent(dd, Encoding.UTF8, "application/json");
                HttpResponseMessage responsedispenser = await Helpers.Helper.GetCallAssetWithBodyAuthAPIAsync(callingMethoddispenser, httpContent, _tokenBase.acces_token);

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
                           IPAddress = OccpIp == null ? "" : OccpIp,
                           LocationsName = c.LocationName,
                           RequestPayload = s.RequestPayload == null ? "" : s.RequestPayload.Replace(",", ",\r\n"),
                           ResponsePayload = s.ResponsePayload == null ? "" : s.ResponsePayload.Replace(",", ",\r\n"),
                           IsRead = s.IsRead == false ? false : true,
                           Flag = "OCPP"


                       }).OrderByDescending(a => a.DateTime).DistinctBy(d => d.EventLogId).Where(r => r.ChargeBoxId != null && r.IsRead == false).ToList<AlertResponse>();
                //if (!string.IsNullOrEmpty(notificationRequest.UserId))
                Notificationcount = _dbContext.TaskNotifications.Where(r=>r.UserId.Equals(_tokenBase.getObjectId()) && r.IsRead == false).Count();
                //else
                //    Notificationcount = _dbContext.TaskNotifications.Where(r=>r.IsRead==false).Count();

                int finalcount = res.Count() + Notificationcount;
                if (finalcount > 0)
                {
                    notificationResponse.StatusMessage = RespnoseMessage.Record_found;
                    notificationResponse.StatusCode = RespnoseCode.OK;
                    notificationResponse.Counts = finalcount;
                }
                else
                {
                    notificationResponse.StatusMessage = RespnoseMessage.Record_not_found;
                    notificationResponse.StatusCode = RespnoseCode.OK;
                    notificationResponse.Counts = 0;
                }
                   
            }
            catch (Exception ex)
            {
                notificationResponse.StatusMessage = RespnoseMessage.Faild;
                notificationResponse.StatusCode = RespnoseCode.Bad_Request;
                notificationResponse.Counts = 0;
            }


            return notificationResponse;
        }
    }
}
