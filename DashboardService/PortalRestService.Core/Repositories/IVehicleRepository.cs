using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;


namespace PortalRestService.Core.Repositories
{

    public interface IVehicleRepository : IRepository<vehicleWithPagination>
    {
       Task<vehicleWithPagination> GetAllVehicle(GetAllVehicleRequest getAllVehicleRequest);
    }
  
}
