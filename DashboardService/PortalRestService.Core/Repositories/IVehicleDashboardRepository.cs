using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
namespace PortalRestService.Core.Repositories
{
    public interface IVehicleDashboardRepository : IRepository<VehicleByIdData>
    {
       public Task<VehicleByIdData> VehicleDetailsById(long Id);
    }
}