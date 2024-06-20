using AutoMapper.Configuration.Annotations;
using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class UpdateOcppEventLogAreReadByOperatorIdHandler : IRequestHandler<UpdateOcppEventLogAreReadByOperatorIdQuery, EventLogLocationResponse>
    {
        private readonly IUpdateOcppEventLogAreReadByOperatorIDRepository _updateOcppEventLogAreReadByOperatorIDRepository;

        public UpdateOcppEventLogAreReadByOperatorIdHandler(IUpdateOcppEventLogAreReadByOperatorIDRepository updateOcppEventLogAreReadByOperatorIDRepository)
        {
            _updateOcppEventLogAreReadByOperatorIDRepository = updateOcppEventLogAreReadByOperatorIDRepository;
        }

        public async Task<EventLogLocationResponse> Handle(UpdateOcppEventLogAreReadByOperatorIdQuery request, CancellationToken cancellationToken)
        {
            return await _updateOcppEventLogAreReadByOperatorIDRepository.UpdateOcppEventLogAreReadByOperator(request.EventLogIds);
        }
    }
}