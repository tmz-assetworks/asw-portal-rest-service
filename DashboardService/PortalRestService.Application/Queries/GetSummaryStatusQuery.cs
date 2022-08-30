using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetSummaryStatusQuery : IRequest<CardDataResponse>
    {
        public int locationId { get; set; }
        public bool isChargersReq { get; set; }

        public GetSummaryStatusQuery(int _locationId,bool _isChargersReq)
        {
            this.locationId = _locationId;
            this.isChargersReq = _isChargersReq;
        }
    }
}