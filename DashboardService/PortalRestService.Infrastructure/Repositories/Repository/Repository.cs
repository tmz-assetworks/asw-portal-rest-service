
using Microsoft.EntityFrameworkCore;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Infrastructure.DBContext;

namespace PortalRestService.Infrastructure.Repositories.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        
    }
}
