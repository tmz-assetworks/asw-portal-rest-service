using MediatR;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetChargerByLocationIDQuery : IRequest<ChargerStatusForChartResponse>
    {
        public List<int> location { get; set; }
        public string duration { get; set; }
        public GetChargerByLocationIDQuery(List<int> Location, string Duration)
        {
            location = Location;
            duration = Duration;
        }
    }
}
