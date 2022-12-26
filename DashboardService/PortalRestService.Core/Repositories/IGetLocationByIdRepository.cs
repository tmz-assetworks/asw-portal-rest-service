using PortalRestService.Core.Repositories.Base;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Repositories
{
    
    public interface IGetLocationByIdRepository : IRepository<GetLocatinByIdResponse>
    {
        //custom operations here
        Task<GetLocatinByIdResponse> GetLocationById(LocationRequest ocationRequest);


    }
}
