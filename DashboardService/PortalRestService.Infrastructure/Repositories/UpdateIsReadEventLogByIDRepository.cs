using Newtonsoft.Json;
using PortalRestService.Core.Models;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Repositories
{
    public class UpdateIsReadEventLogByIDRepository : OcppRepository<EventLogLocationResponse>, IUpdateIsReadEventLogByIDRepository
    {
        TokenBase _tokenBase;
        public UpdateIsReadEventLogByIDRepository(Infrastructure.DBContext.ocpp_dbContext dbContext,TokenBase token) : base(dbContext)
        {
            _tokenBase = token;
        }
        public async Task<EventLogLocationResponse> UpdateOcppEventLogIsRead(int id)
        {
            EventLogLocationResponse EventLogLocationres = new EventLogLocationResponse();

             OcppEventLog OcppEventLogs = new OcppEventLog();

            OcppEventLogs = _dbContext.Set<OcppEventLog>().Find(id);
            OcppEventLogs.IsRead = true;
            _dbContext.Entry(OcppEventLogs);
            await _dbContext.SaveChangesAsync();

            EventLogLocationres.data = null;
            EventLogLocationres.StatusCode = 200;
            EventLogLocationres.StatusMessage = "updated";

            return EventLogLocationres;

        }
    }



}
