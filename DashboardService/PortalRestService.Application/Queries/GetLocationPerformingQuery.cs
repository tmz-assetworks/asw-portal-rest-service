using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetLocationPerformingQuery : IRequest<LocationPerformingChartResponse>
    {
        public List<int> location { get; set; }
        public string duration { get; set; }

        public int orderby { get; set; }
        public GetLocationPerformingQuery(List<int> Location, string Duration,int Orderby)
        {
            location = Location;
            duration = Duration;
            orderby = Orderby;
        }
    }
}
