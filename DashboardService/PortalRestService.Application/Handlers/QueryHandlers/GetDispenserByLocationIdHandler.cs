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
    public class GetDispenserByLocationIdHandler : IRequestHandler<GetDispenserByLocationIdQuery, LocationDispenserForLocationResponse>
    {
        private readonly ILocationDispenserRepository _locationDispenserRepository;

        public GetDispenserByLocationIdHandler(ILocationDispenserRepository locationDispenserRepository)
        {
            _locationDispenserRepository = locationDispenserRepository;
        }

        public async Task<LocationDispenserForLocationResponse> Handle(GetDispenserByLocationIdQuery request, CancellationToken cancellationToken)
        {
            return await _locationDispenserRepository.GetDispenserByLocation(request.location);
        }
    }
}
