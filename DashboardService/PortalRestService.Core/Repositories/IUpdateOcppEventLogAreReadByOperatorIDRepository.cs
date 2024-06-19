using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;

namespace PortalRestService.Core.Repositories
{
    public interface IUpdateOcppEventLogAreReadByOperatorIDRepository : IRepository<EventLogLocationResponse>
    {
        /// <summary>
        /// Updates the read status of the specified OCPP event logs to 'read'.
        /// </summary>
        /// <param name="eventLogIds">A list of event log IDs to be marked as read.</param>
        /// <returns>An EventLogLocationResponse indicating the result of the update operation.</returns>
        Task<EventLogLocationResponse> UpdateOcppEventLogAreReadByOperator(List<int> eventLogIds);
    }
}