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
    public class GetEnergyUsedsByLocationIDQuery : IRequest<EnergyUsedBOForChartResponse>
    {
        public List<int> location { get; set; }
        public string duration { get; set; }
        public string chargeboxId { get; set; }
        public GetEnergyUsedsByLocationIDQuery(List<int> Location, string Duration, string ChargeboxId)
        {
            location = Location;
            duration = Duration;
            chargeboxId = ChargeboxId;
        }
    }
}
