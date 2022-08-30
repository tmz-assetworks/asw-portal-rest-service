using MediatR;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Queries
{
    public class GetAllVehicleQuery : IRequest<vehicleWithPagination>
    {

        public GetAllVehicleRequest GetAllVehicleRequest { get; set; }

        public GetAllVehicleQuery(GetAllVehicleRequest getAllVehicleRequest)
        {
            this.GetAllVehicleRequest = getAllVehicleRequest;

        }

    }
}