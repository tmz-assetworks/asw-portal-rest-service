using PortalRestService.Application;
using PortalRestService.Core.PagingHelper;
using MediatR;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Queries
{

    public class GetDispensersDetailQuery : IRequest<PagedList<DispensersDetail>>
    {
        public DispensersDetailRequest _dispensersDetailRequest = null;
        public GetDispensersDetailQuery(DispensersDetailRequest dispensersDetailRequest)
        {
            this._dispensersDetailRequest = dispensersDetailRequest;
        }
    }


}
