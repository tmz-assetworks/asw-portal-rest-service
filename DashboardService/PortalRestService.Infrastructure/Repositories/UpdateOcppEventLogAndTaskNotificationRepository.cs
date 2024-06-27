using Microsoft.EntityFrameworkCore;
using PortalRestService.Core.ConstantResponse;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Infrastructure.DBContext;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net;

namespace PortalRestService.Infrastructure.Repositories
{
    public class UpdateOcppEventLogAndTaskNotificationRepository : OcppRepository<EventLogLocationResponse>, IUpdateOcppEventLogAndTaskNotificationRepository
    {
        public UpdateOcppEventLogAndTaskNotificationRepository(ocpp_dbContext dbContext) : base(dbContext)
        {
        }

        /// <inheritdoc/>
        public async Task<EventLogLocationResponse> UpdateOcppEventLogAndTaskNotification(List<OcppEventLogAndTaskNotificationRequest> ocppEventLogAndTaskNotificationRequests)
        {
            bool isUpdated = false;

            // Separate requests based on category
            var nonEmailRequests = ocppEventLogAndTaskNotificationRequests
                .Where(x => x.Category.ToLower() != "email")
                .Select(x => x.EventLogId)
                .ToArray();

            var emailRequests = ocppEventLogAndTaskNotificationRequests
                .Where(x => x.Category.ToLower() == "email")
                .Select(x => x.EventLogId)
                .ToArray();

            // Update OCPP event logs if there are non-email requests
            if (nonEmailRequests.Length > 0)
            {
                var ocppEventLogsToUpdate = await _dbContext.OcppEventLogs
                    .Where(e => nonEmailRequests.Contains(e.Id) && e.IsRead == false)
                    .ToListAsync();

                if (ocppEventLogsToUpdate.Any())
                {
                    ocppEventLogsToUpdate.ForEach(item => item.IsRead = true);
                    _dbContext.OcppEventLogs.UpdateRange(ocppEventLogsToUpdate);
                    await _dbContext.SaveChangesAsync();
                    isUpdated = true;
                }
            }

            // Update task notifications if there are email requests
            if (emailRequests.Length > 0)
            {
                var taskNotificationsToUpdate = await _dbContext.TaskNotifications
                    .Where(e => emailRequests.Contains(e.Id) && !e.IsRead)
                    .ToListAsync();

                if (taskNotificationsToUpdate.Any())
                {
                    taskNotificationsToUpdate.ForEach(item => item.IsRead = true);
                    _dbContext.TaskNotifications.UpdateRange(taskNotificationsToUpdate);
                    await _dbContext.SaveChangesAsync();
                    isUpdated = true;
                }
            }

            if (isUpdated)
            {
                return new EventLogLocationResponse
                {
                    StatusMessage = RespnoseMessage.Record_Updated_Successfully,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new EventLogLocationResponse
            {
                StatusMessage = RespnoseMessage.Record_Not_Updated,
                StatusCode = (int)HttpStatusCode.NotModified
            };
        }
    }
}