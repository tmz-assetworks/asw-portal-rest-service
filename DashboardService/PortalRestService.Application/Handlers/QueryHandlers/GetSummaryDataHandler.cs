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
    internal class GetSummaryDataHandler : IRequestHandler<GetSummaryDataQuery, SummaryData>
    {
        private readonly IGetSummaryDataRepository _getSummaryDataRepository;

        public GetSummaryDataHandler(IGetSummaryDataRepository getSummaryDataRepository)
        {
            _getSummaryDataRepository = getSummaryDataRepository;
        }

        public async Task<SummaryData> Handle(GetSummaryDataQuery request, CancellationToken cancellationToken)
        {
            return await _getSummaryDataRepository.GetSummaryData(request.locationId);
        }
    }
}
