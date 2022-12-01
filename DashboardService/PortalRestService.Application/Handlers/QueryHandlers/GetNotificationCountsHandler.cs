using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetNotificationCountsHandler : IRequestHandler<GetNotificationCountQuery, NotificationResponse>
    {
        private readonly INotificationRepository _INotificationRepository;

        public GetNotificationCountsHandler(INotificationRepository notificationRepository)
        {
            _INotificationRepository = notificationRepository;
        }

        public async Task<NotificationResponse> Handle(GetNotificationCountQuery request, CancellationToken cancellationToken)
        {
            return await _INotificationRepository.GetNotificationCountsByUserid(request._NotificationRequest);
        }
    }
}
