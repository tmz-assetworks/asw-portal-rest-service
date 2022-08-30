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
    public class GetMilesAddedByLocationHandler : IRequestHandler<GetMilesAddedByLocationQuery, MilesAddedByLocationChartResponse>
    {
        private readonly IMilesAddedByLocationQueryRepository _MilesAddedByLocationRepository;

        public GetMilesAddedByLocationHandler(IMilesAddedByLocationQueryRepository MilesAddedByLocationRepository)
        {
            _MilesAddedByLocationRepository = MilesAddedByLocationRepository;
        }

        public async Task<MilesAddedByLocationChartResponse> Handle(GetMilesAddedByLocationQuery request, CancellationToken cancellationToken)
        {
            return await _MilesAddedByLocationRepository.GetMilesAddedByLocation(request.location, request.duration, request.chargeBoxId);
        }
    }
}
