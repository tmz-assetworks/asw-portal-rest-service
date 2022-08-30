using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetSummaryDataQuery : IRequest<SummaryData>
    {
        public int locationId { get; set; }

        public GetSummaryDataQuery(int _locationId)
        {
            this.locationId = _locationId;
        }
    }
}