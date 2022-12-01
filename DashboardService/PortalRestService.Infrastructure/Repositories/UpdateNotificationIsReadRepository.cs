using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Models;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class UpdateNotificationIsReadRepository : OcppRepository<SaveNotificationResponse>, IUpdateIsNotificationRepository
    {
        TokenBase _tokenBase;
        public UpdateNotificationIsReadRepository(Infrastructure.DBContext.ocpp_dbContext dbContext, TokenBase token) : base(dbContext)
        {
            _tokenBase = token;
        }

        public Task<SaveNotificationResponse> UpdateNotificationIsRead(NotificationCommand notificationCommand)
        {
            SaveNotificationResponse SaveNotificationResponse = new SaveNotificationResponse();
            if (notificationCommand.flag.ToLower() == "OCPP".ToLower())
            {
                
                OcppEventLog OcppEventLogs = new OcppEventLog();

                OcppEventLogs = _dbContext.Set<OcppEventLog>().Find(notificationCommand.Id);
                OcppEventLogs.IsRead = true;
                _dbContext.Entry(OcppEventLogs);
            }
            else if (notificationCommand.flag.ToLower() == "ASSET".ToLower())
            {  TaskNotifications taskNotifications = new TaskNotifications();

                taskNotifications = _dbContext.Set<TaskNotifications>().Find(notificationCommand.Id);
                taskNotifications.IsRead = true;
                taskNotifications.UserId= _tokenBase.getObjectId();
                _dbContext.Entry(taskNotifications);
            }
            
             _dbContext.SaveChangesAsync();


            SaveNotificationResponse.StatusCode = 200;
            SaveNotificationResponse.StatusMessage = RespnoseMessage.Record_Updated_Successfully;

            return Task.FromResult(SaveNotificationResponse);
        }

    }
}
