using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetAllAlertsQuery : IRequest<OperatorAlertResponse>
    {
        public OperatorAlertRequest _operatorAlertRequest { get; set; }

        public GetAllAlertsQuery(OperatorAlertRequest operatorAlertRequest)
        {
            this._operatorAlertRequest = operatorAlertRequest;
        }
    }
}
