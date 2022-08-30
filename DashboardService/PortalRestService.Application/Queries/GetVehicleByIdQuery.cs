using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetVehicleByIdQuery : IRequest<VehicleByIdData>
    {
        public long vehicle { get; set; }
        public GetVehicleByIdQuery(long Vehicle)
        {
            vehicle = Vehicle;
        }
    }
}

