using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class UpdateOcppEventLogAndTaskNotificationHandler : IRequestHandler<UpdateOcppEventLogAndTaskNotificationQuery, EventLogLocationResponse>
    {
        private readonly IUpdateOcppEventLogAndTaskNotificationRepository _updateOcppEventLogAndTaskNotificationRepository;

        public UpdateOcppEventLogAndTaskNotificationHandler(IUpdateOcppEventLogAndTaskNotificationRepository updateOcppEventLogAndTaskNotificationRepository)
        {
            _updateOcppEventLogAndTaskNotificationRepository = updateOcppEventLogAndTaskNotificationRepository;
        }

        public async Task<EventLogLocationResponse> Handle(UpdateOcppEventLogAndTaskNotificationQuery request, CancellationToken cancellationToken)
        {
            return await _updateOcppEventLogAndTaskNotificationRepository.UpdateOcppEventLogAndTaskNotification(request.OcppEventLogAndTaskNotificationRequest);
        }
    }
}