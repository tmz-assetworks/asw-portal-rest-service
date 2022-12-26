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
    
    public  class GetLocationsDispenserHandler : IRequestHandler<LocationOpratorQuery, LocationsDispenserpResponce>
    {
        private readonly ILocationsDispenserRepository _ilocationsDispenserRepository;

        public GetLocationsDispenserHandler(ILocationsDispenserRepository ilocationsDispenserRepository)
        {
            _ilocationsDispenserRepository = ilocationsDispenserRepository;
        }

        public async Task<LocationsDispenserpResponce> Handle(LocationOpratorQuery request, CancellationToken cancellationToken)
        {
            return await _ilocationsDispenserRepository.GetLocationsDispenserformap(request._locationOpratorRequest);
        }
    }
}
