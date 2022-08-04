using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.Assets.QueryHandlers
{
    public class GetLocationStatusByLocationHandler : IRequestHandler<GetLocationStatusByLocationIdQuery, List<AllLocationStatusChartBO>>
    {

     private readonly ILocationStatusByLocationIdRepository _locationStatusByLocationIdRepository;

        public GetLocationStatusByLocationHandler(ILocationStatusByLocationIdRepository locationStatusByLocationIdRepository)
        {
            _locationStatusByLocationIdRepository = locationStatusByLocationIdRepository;
        }

        public async Task<List<AllLocationStatusChartBO>> Handle(GetLocationStatusByLocationIdQuery request, CancellationToken cancellationToken)
        {
            return await _locationStatusByLocationIdRepository.GetLocationStatusByLocatonId(request.location, request.duration);
        }
    }
}
