using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetDispenserByLocationIdQuery : IRequest<LocationDispenserForLocationResponse>
    {
        public LocationDispensersRequest _LocationDispensersRequest = null;
        public GetDispenserByLocationIdQuery(LocationDispensersRequest dispensersDetailRequest)
        {
            this._LocationDispensersRequest = dispensersDetailRequest;
        }
    }
}
