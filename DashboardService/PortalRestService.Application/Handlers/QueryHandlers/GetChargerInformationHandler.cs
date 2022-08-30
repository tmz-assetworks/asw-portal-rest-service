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
    public class GetChargerInformationHandler : IRequestHandler<GetChargerInformationQuery, ChargerInformationResponse>
    {
        private readonly IGetChargerInformationRepository _getChargerInformationRepository;

        public GetChargerInformationHandler(IGetChargerInformationRepository getChargerInformationRepository)
        {
            _getChargerInformationRepository = getChargerInformationRepository;
        }
        public async Task<ChargerInformationResponse> Handle(GetChargerInformationQuery request, CancellationToken cancellationToken)
        {
            return await _getChargerInformationRepository.GetChargerInformation(request._chargeBoxId, request._operatorId);
        }
    }
}
