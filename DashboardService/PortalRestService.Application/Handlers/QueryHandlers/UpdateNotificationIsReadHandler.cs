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
   
    public class UpdateNotificationIsReadHandler : IRequestHandler<UpdateNotificationIsReadQuery, SaveNotificationResponse>
    {
        private readonly IUpdateIsNotificationRepository _UpdateIsReadEventLogByIDRepository;

        public UpdateNotificationIsReadHandler(IUpdateIsNotificationRepository updateIsReadEventLogByIDRepository)
        {
            _UpdateIsReadEventLogByIDRepository = updateIsReadEventLogByIDRepository;
        }


        public async Task<SaveNotificationResponse> Handle(UpdateNotificationIsReadQuery request, CancellationToken cancellationToken)
        {
            return await _UpdateIsReadEventLogByIDRepository.UpdateNotificationIsRead(request.NotificationCommand);
        }
    }
}
