using MediatR;
using PortalRestService.Core.Responses;


namespace PortalRestService.Application.Queries
{
    public class UpdateOcppEventLogAndTaskNotificationQuery : IRequest<EventLogLocationResponse>
    {
        public List<OcppEventLogAndTaskNotificationRequest> OcppEventLogAndTaskNotificationRequest { get; set; }
        
        public UpdateOcppEventLogAndTaskNotificationQuery(List<OcppEventLogAndTaskNotificationRequest> ocppEventLogAndTaskNotificationRequest)
        {
            OcppEventLogAndTaskNotificationRequest = ocppEventLogAndTaskNotificationRequest;
        }
    }
}