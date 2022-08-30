using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetChargerByLocationIDHandler : IRequestHandler<GetChargerByLocationIDQuery, ChargerStatusForChartResponse>
    {
        private readonly IChargerByLocationRepository _chargerlocationRepository;

        public GetChargerByLocationIDHandler(IChargerByLocationRepository chargerlocationRepository)
        {
            _chargerlocationRepository = chargerlocationRepository;
        }

        
        public async Task<ChargerStatusForChartResponse> Handle(GetChargerByLocationIDQuery request, CancellationToken cancellationToken)
        {
            return await _chargerlocationRepository.GetChargerStatusByLocationID(request.location, request.duration,request.chargeBoxId);
        }
    }
}
