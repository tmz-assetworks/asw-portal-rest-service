using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.Assets.QueryHandlers
{
    public class GetAllChargingSessionHandler : IRequestHandler<GetAllChargingSessionQuery, ChargingSessionByLocationForChartResponse>
    {
        private readonly IChargingSessionRepository _chargingSessionRepository;

        public GetAllChargingSessionHandler(IChargingSessionRepository chargingsessionRepository)
        {
            _chargingSessionRepository = chargingsessionRepository;
        }

        public async Task<ChargingSessionByLocationForChartResponse> Handle(GetAllChargingSessionQuery request, CancellationToken cancellationToken)
        {
            return await _chargingSessionRepository.GetChargerSession(request.location, request.duration,request.ChargeBoxId);
        }
    }
}
