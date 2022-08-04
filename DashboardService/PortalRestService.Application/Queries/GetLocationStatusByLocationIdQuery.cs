using MediatR;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Queries
{
    public class GetLocationStatusByLocationIdQuery : IRequest<List<AllLocationStatusChartBO>>
    {
        public List<int> location { get; set; }
        public string duration { get; set; }
        public GetLocationStatusByLocationIdQuery(List<int> Location, string Duration)
        {
            location = Location;
            duration = Duration;
        }

    }
}