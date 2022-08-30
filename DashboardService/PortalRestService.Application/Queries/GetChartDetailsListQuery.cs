using MediatR;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetChartDetailsListQuery : IRequest<PagedList<ChartDetailsList>>
    {
        public ChartDetailsListRequest chartDetailsListRequest { get; set; }
        public GetChartDetailsListQuery(ChartDetailsListRequest chartDetailsListRequest)
        {
            this.chartDetailsListRequest = chartDetailsListRequest;
        }
    }
}
