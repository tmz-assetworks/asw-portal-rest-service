using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace AssetsService.Application.Handlers.Assets.QueryHandlers.Assets
{

    public class GetDispensersDetailHandler : IRequestHandler<GetDispensersDetailQuery, PagedList<DispensersDetail>>
    {
        private readonly IDispenserDetailRepository _dispenserRepo;

        public GetDispensersDetailHandler(IDispenserDetailRepository dispenserRepo)
        {
            this._dispenserRepo = dispenserRepo;
        }

        public async Task<PagedList<DispensersDetail>> Handle(GetDispensersDetailQuery request, CancellationToken cancellationToken)
        {
            return (PagedList<DispensersDetail>)await _dispenserRepo.GetDispensersDetail(request._dispensersDetailRequest);
        }
    }
}
