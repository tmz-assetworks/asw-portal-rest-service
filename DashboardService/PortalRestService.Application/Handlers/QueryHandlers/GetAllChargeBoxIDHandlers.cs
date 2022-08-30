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
    public class GetAllChargeBoxIDHandlers : IRequestHandler<GetChargeBoxIDQuery, ChargeBoxIDListResponse>
    {
        private readonly IGetAllChargeBoxIDRepository _getChargeBoxIDRepository;

        public GetAllChargeBoxIDHandlers(IGetAllChargeBoxIDRepository getChargeBoxIDRepository)
        {
            _getChargeBoxIDRepository = getChargeBoxIDRepository;
        }

        public async Task<ChargeBoxIDListResponse> Handle(GetChargeBoxIDQuery request, CancellationToken cancellationToken)
        {
            return await _getChargeBoxIDRepository.GetAllChargeBoxID();
        }
    }
}
