using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.Assets.QueryHandlers
{
    public class GetChargingSessionHandler : IRequestHandler<GetAllChargingSessionQuery, ChargingSessionByLocationForChartResponse>
    {
        private readonly IChargerSessionRepository _chargerSessionRepository;

        public GetChargingSessionHandler(IChargerSessionRepository chargersessionRepository)
        {
            _chargerSessionRepository = chargersessionRepository;
        }

        public async Task<ChargingSessionByLocationForChartResponse> Handle(GetAllChargingSessionQuery request, CancellationToken cancellationToken)
        {
            return await _chargerSessionRepository.GetChargerSession(request.location, request.duration);
        }

    }
}
    
