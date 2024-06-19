using MediatR;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Queries
{
    public class UpdateOcppEventLogAreReadByOperatorIdQuery : IRequest<EventLogLocationResponse>
    {
        public List<int> EventLogIds { get; set; }
        public UpdateOcppEventLogAreReadByOperatorIdQuery(List<int> eventLogIds)
        {
            EventLogIds = eventLogIds;
        }
    }
}
