using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;

namespace PortalRestService.Core.Repositories
{
    public interface IDispenserDetailRepository : IRepository<DispensersDetailResponse>
    {
        Task<PagedList<DispensersDetail>> GetDispensersDetail(DispensersDetailRequest dispensersDetailRequest);
    }

}