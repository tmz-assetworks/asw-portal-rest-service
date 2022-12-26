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
    public class GetLocationsDispenserDetailsQuery : IRequest<PagedList<Core.Responses.LocationsDispenserDetails>>
    {
        public LocationDispenserDetailRequest LocationDispenserRequest { get; set; }
        public GetLocationsDispenserDetailsQuery(LocationDispenserDetailRequest locationDispenserRequest)
        {
            this.LocationDispenserRequest = locationDispenserRequest;
        }
    }
}
