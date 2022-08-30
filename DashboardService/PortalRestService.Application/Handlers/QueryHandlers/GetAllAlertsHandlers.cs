using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetAllAlertsHandlers : IRequestHandler<GetAllAlertsQuery, OperatorAlertResponse>
    {
        private readonly IGetAllAlertsRepository _getAlertsRepository;

        public GetAllAlertsHandlers(IGetAllAlertsRepository getAlertsRepository)
        {
            _getAlertsRepository = getAlertsRepository;
        }

        public async Task<OperatorAlertResponse> Handle(GetAllAlertsQuery request, CancellationToken cancellationToken)
        {
            return await _getAlertsRepository.GetAllAlerts(request._operatorAlertRequest);
        }
    }
}
