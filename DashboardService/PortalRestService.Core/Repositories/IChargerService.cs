using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;


namespace PortalRestService.Core.Repositories
{
    public interface IChargerRepository : IRepository<ChargerResponse>
    {
        //custom operations here
        Task<ChargerResponse> GetAllCharger();
    }
}
