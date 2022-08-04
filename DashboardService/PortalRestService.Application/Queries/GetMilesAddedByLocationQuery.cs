using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetMilesAddedByLocationQuery : IRequest<MilesAddedByLocationChartResponse>
    {
        public List<int> location { get; set; }
        public string duration { get; set; }


        public GetMilesAddedByLocationQuery(List<int> Location, string Duration)
        {
            location = Location;
            duration = Duration;

        }
    }
}
