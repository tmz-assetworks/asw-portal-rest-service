using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class UpdateNotificationIsReadQuery : IRequest<SaveNotificationResponse>
    {
        public NotificationCommand NotificationCommand { get; set; }
     
        public UpdateNotificationIsReadQuery(NotificationCommand notificationCommand)
        {
            this.NotificationCommand = notificationCommand;
        }
    }
}
