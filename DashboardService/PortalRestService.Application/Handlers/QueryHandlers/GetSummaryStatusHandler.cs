using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetSummaryStatusHandler : IRequestHandler<GetSummaryStatusQuery, CardDataResponse>
    {
        private readonly IGetSummaryStatusRepository _getSummaryStatusRepository;

        public GetSummaryStatusHandler(IGetSummaryStatusRepository getSummaryDataRepository)
        {
            _getSummaryStatusRepository = getSummaryDataRepository;
        }

        public async Task<CardDataResponse> Handle(GetSummaryStatusQuery request, CancellationToken cancellationToken)
        {
            return await _getSummaryStatusRepository.GetSummaryStatus(request.locationId, request.isChargersReq);
        }
    }
}
