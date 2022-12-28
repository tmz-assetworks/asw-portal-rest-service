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
    public class GetLocatinByIdHandler : IRequestHandler<GetLocationByIdQuery, GetLocatinByIdResponse>
    {
        private readonly IGetLocationByIdRepository _getLocationByIdRepository;

        public GetLocatinByIdHandler(IGetLocationByIdRepository getLocationByIdRepository)
        {
            _getLocationByIdRepository = getLocationByIdRepository;
        }

        public async Task<GetLocatinByIdResponse> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            return await _getLocationByIdRepository.GetLocationById(request.locationRequest);
        }
    }
}
