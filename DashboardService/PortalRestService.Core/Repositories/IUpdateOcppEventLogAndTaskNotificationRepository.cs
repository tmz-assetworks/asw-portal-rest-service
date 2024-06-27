using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;

namespace PortalRestService.Core.Repositories
{
    public interface IUpdateOcppEventLogAndTaskNotificationRepository : IRepository<EventLogLocationResponse>
    {
        /// <summary>
        /// Updates OCPP event logs and task notifications based on the provided requests.
        /// </summary>
        /// <param name="ocppEventLogAndTaskNotificationRequests">List of requests containing OCPP event log and task notification data.</param>
        /// <returns>Returns a response indicating whether the records were successfully updated or not.</returns>
        Task<EventLogLocationResponse> UpdateOcppEventLogAndTaskNotification(List<OcppEventLogAndTaskNotificationRequest> ocppEventLogAndTaskNotificationRequests);
    }
}