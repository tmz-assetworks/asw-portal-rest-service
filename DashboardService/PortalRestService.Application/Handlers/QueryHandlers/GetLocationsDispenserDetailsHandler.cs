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
    public class GetLocationsDispenserDetailsHandler : IRequestHandler<GetLocationsDispenserDetailsQuery, PagedList<Core.Responses.LocationsDispenserDetails>>
    {
        private readonly ILocationRepository _LocationRepo;

        public GetLocationsDispenserDetailsHandler(ILocationRepository LocationRepository)
        {
            _LocationRepo = LocationRepository;
        }

        public async Task<PagedList<Core.Responses.LocationsDispenserDetails>> Handle(GetLocationsDispenserDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _LocationRepo.GetLocationsDispenserDetails(request.LocationDispenserRequest);
        }
        
    }
}
