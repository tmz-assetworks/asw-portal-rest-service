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
    public class ChargingSessionAndPaymentTransactionHandler : IRequestHandler<ChargingSessionAndPaymentTransactionQuery, SessionAndPaymentDTO>
    {
        private readonly IChargingSessionAndPaymentTransactionRepository _chargingSessionAndPaymentTransactionRepository;
        public ChargingSessionAndPaymentTransactionHandler(IChargingSessionAndPaymentTransactionRepository chargingSessionAndPaymentTransactionRepository)
        {
            _chargingSessionAndPaymentTransactionRepository = chargingSessionAndPaymentTransactionRepository;
        }
        public async Task<SessionAndPaymentDTO> Handle(ChargingSessionAndPaymentTransactionQuery request, CancellationToken cancellationToken)
        {
            return await _chargingSessionAndPaymentTransactionRepository.GetSessionAndPaymentDTOAsync(request.transactionId);
        }
    }
}
