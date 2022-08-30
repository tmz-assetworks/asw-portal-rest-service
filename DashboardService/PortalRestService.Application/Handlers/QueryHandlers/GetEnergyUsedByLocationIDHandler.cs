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
    public class GetEnergyUsedByLocationIDHandler : IRequestHandler<GetEnergyUsedsByLocationIDQuery, EnergyUsedBOForChartResponse>
    {
        private readonly IEnergyUsedByLocationIDRepository _energyUsedByLocationIDRepository;

        public GetEnergyUsedByLocationIDHandler(IEnergyUsedByLocationIDRepository chargersessionRepository)
        {
            _energyUsedByLocationIDRepository = chargersessionRepository;
        }

        public async Task<EnergyUsedBOForChartResponse> Handle(GetEnergyUsedsByLocationIDQuery request, CancellationToken cancellationToken)
        {
            return await _energyUsedByLocationIDRepository.GetEnergyUsedByLocationID(request.location, request.duration, request.chargeboxId);
        }
    }
}
