using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.QueryHandlers
{

    public class GetVehicleByIdHandler : IRequestHandler<GetVehicleByIdQuery,VehicleByIdData>
    {

        private readonly IVehicleDashboardRepository _vehicleDashboardRepository;

        public GetVehicleByIdHandler(IVehicleDashboardRepository vehicleDashboardRepository)
        {
            _vehicleDashboardRepository = vehicleDashboardRepository;
        }

        public async Task<VehicleByIdData> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _vehicleDashboardRepository.VehicleDetailsById(request.vehicle);
        }
    }
}