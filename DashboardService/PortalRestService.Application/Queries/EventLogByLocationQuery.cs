using MediatR;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class EventLogByLocationQuery : IRequest<PagedList<EventLogLocation>>
    {
        public EventLogRequest eventLogRequest { get; set; }
        public EventLogByLocationQuery(EventLogRequest eventLogRequest)
        {
            this.eventLogRequest = eventLogRequest;
        }


    }
}
