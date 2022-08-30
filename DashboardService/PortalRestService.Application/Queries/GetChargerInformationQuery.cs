using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetChargerInformationQuery : IRequest<ChargerInformationResponse>
    {
        public string _chargeBoxId { get; set; }
        public string _operatorId { get; set; }
        public GetChargerInformationQuery(string ChargeBoxId, string OperatorId)
        {
            _chargeBoxId = ChargeBoxId;
            _operatorId = OperatorId;
        }
    }
}
