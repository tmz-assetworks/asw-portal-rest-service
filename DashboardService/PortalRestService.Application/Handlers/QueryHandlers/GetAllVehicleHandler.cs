using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.Assets.QueryHandlers
{
    public class GetAllVehicleHandler : IRequestHandler<GetAllVehicleQuery, vehicleWithPagination>
    {

     private readonly IVehicleRepository _vehicleRepository;

        public GetAllVehicleHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<vehicleWithPagination> Handle(GetAllVehicleQuery request, CancellationToken cancellationToken)
        {
            return await _vehicleRepository.GetAllVehicle(request.GetAllVehicleRequest);
        }
    }
}
