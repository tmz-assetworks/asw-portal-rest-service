
using MediatR;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Queries
{
    public class GetAllChargingSessionQuery : IRequest<ChargingSessionByLocationForChartResponse>
    {
        public List<int> location { get; set; }
        public string duration { get; set; }
        public GetAllChargingSessionQuery(List<int> Location, string Duration)
        {
            location = Location;
            duration = Duration;
        }
    }
}