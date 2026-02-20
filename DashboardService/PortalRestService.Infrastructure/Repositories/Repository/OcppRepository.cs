using Microsoft.EntityFrameworkCore;
using PortalRestService.Core.Repositories.Base;
using PortalRestService.Infrastructure.DBContext;
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

    public class OcppRepositoryFactory<T> : IOcppRepository<T> where T : class
    {
        protected readonly IDbContextFactory<ocpp_dbContext> _dbFactory;

        public OcppRepositoryFactory(IDbContextFactory<ocpp_dbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        protected async Task<ocpp_dbContext> CreateDbContextAsync()
        {
            return await _dbFactory.CreateDbContextAsync();
        }
    }




}