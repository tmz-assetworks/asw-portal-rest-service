using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{
   
    public interface IUpdateIsReadEventLogByIDRepository : IRepository<EventLogLocationResponse>
    {
        //custom operations here
        Task<EventLogLocationResponse> UpdateOcppEventLogIsRead(int id);
    }
}
