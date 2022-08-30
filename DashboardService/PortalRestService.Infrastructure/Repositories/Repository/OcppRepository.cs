using PortalRestService.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories.Repository
{
    public class OcppRepository<T> : IOcppRepository<T> where T : class
    {
        protected readonly PortalRestService.Infrastructure.DBContext.ocpp_dbContext _dbContext;
        
        public OcppRepository(PortalRestService.Infrastructure.DBContext.ocpp_dbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
