using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetAllChartDetailsListHandler : IRequestHandler<GetChartDetailsListQuery, List<ChartDetailsList>>
    {
        private readonly IChartDetailsListRepository _chartDetailsListRepository;

        public GetAllChartDetailsListHandler(IChartDetailsListRepository chartDetailsListRepository)
        {
            _chartDetailsListRepository = chartDetailsListRepository;
        }


        public async Task <List<ChartDetailsList>> Handle(GetChartDetailsListQuery request, CancellationToken cancellationToken)
        {
            return await _chartDetailsListRepository.GetChartDetailsList(request.chartDetailsListRequest);
        }
    }
}
