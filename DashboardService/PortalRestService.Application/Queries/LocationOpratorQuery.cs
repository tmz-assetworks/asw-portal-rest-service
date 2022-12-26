using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class LocationOpratorQuery : IRequest<LocationsDispenserpResponce>
    {
        public LocationOpratorRequest _locationOpratorRequest { get; set; }

        public LocationOpratorQuery(LocationOpratorRequest locationOpratorRequest)
        {
            this._locationOpratorRequest = locationOpratorRequest;
        }
    }
}
