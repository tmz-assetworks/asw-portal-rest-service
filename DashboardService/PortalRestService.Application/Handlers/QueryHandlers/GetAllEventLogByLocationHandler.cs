using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetAllEventLogByLocationHandler : IRequestHandler<EventLogByLocationQuery, PagedList<EventLogLocation>>
    {
        private readonly IEventLogByLocationRepository _eventLogByLocationRepository;

        public GetAllEventLogByLocationHandler(IEventLogByLocationRepository eventLogByLocationRepository)
        {
            _eventLogByLocationRepository = eventLogByLocationRepository;
        }

      
        public async Task<PagedList<EventLogLocation>> Handle(EventLogByLocationQuery request, CancellationToken cancellationToken)
        {
            return (PagedList<EventLogLocation>)await _eventLogByLocationRepository.GetEventLogByLocation(request.eventLogRequest);
        }
    }
}
