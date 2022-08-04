
using MediatR;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Queries
{
    public class GetAllChargerQuery : IRequest<ChargerResponse>
    {
    }
}
