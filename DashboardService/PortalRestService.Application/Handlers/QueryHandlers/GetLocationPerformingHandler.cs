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
    public class GetLocationPerformingHandler : IRequestHandler<GetLocationPerformingQuery, LocationPerformingChartResponse>
    {
        private readonly ILocationPerformingRepository _locationPerformingRepository;

        public GetLocationPerformingHandler(ILocationPerformingRepository locationPerformingRepository)
        {
            _locationPerformingRepository = locationPerformingRepository;
        }

        public async Task<LocationPerformingChartResponse> Handle(GetLocationPerformingQuery request, CancellationToken cancellationToken)
        {
            return await _locationPerformingRepository.GetLocationPerforming(request.location, request.duration, request.orderby);
        }
    }
}
