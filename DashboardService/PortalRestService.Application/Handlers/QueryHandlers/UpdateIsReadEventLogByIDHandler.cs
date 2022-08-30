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
  
    public class UpdateIsReadEventLogByIDHandler : IRequestHandler<UpdateIsReadEventLogByIDQuery, EventLogLocationResponse>
    {
        private readonly IUpdateIsReadEventLogByIDRepository _UpdateIsReadEventLogByIDRepository;

        public UpdateIsReadEventLogByIDHandler(IUpdateIsReadEventLogByIDRepository updateIsReadEventLogByIDRepository)
        {
            _UpdateIsReadEventLogByIDRepository = updateIsReadEventLogByIDRepository;
        }

      
        public async Task<EventLogLocationResponse> Handle(UpdateIsReadEventLogByIDQuery request, CancellationToken cancellationToken)
        {
            return await _UpdateIsReadEventLogByIDRepository.UpdateOcppEventLogIsRead(request.Id);
        }
    }
}
