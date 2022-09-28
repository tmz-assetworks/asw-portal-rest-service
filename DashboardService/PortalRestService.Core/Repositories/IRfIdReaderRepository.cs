using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;


namespace PortalRestService.Core.Repositories
{

    public interface IRfIdReaderRepository : IRepository<RfIdReaderDetailsResponse>
    {
     Task<RfIdReaderDetailsResponse> GetRfIdReaderById(long Id);
    }
  
}
