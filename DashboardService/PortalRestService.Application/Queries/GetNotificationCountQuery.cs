using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetNotificationCountQuery : IRequest<NotificationResponse>
    {
        public NotificationRequest _NotificationRequest { get; set; }
        public GetNotificationCountQuery(NotificationRequest notificationRequest)
        {
            _NotificationRequest=notificationRequest;


        }
    }
}
