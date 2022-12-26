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
    public class GetAllLocationHandler : IRequestHandler<GetGetAllLocationQuery, AllLocationQueryResponse>
    {
        private readonly ILocationRepository _LocationRepo;

        public GetAllLocationHandler(ILocationRepository LocationRepository)
        {
            _LocationRepo = LocationRepository;
        }

        
        

        public async Task<AllLocationQueryResponse> Handle(GetGetAllLocationQuery request, CancellationToken cancellationToken)
        {
            return await _LocationRepo.GetAllLocation();
        }
    }
}
