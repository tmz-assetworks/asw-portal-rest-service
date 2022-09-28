using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;

namespace PortalRestService.Application.Handlers.QueryHandlers
{

    public class GetRfIdReaderHandler : IRequestHandler<GetRfIdReaderQuery, RfIdReaderDetailsResponse>
    {

        private readonly IRfIdReaderRepository _rfIdReaderRepository;

        public GetRfIdReaderHandler(IRfIdReaderRepository rfIdReaderRepository)
        {
            _rfIdReaderRepository = rfIdReaderRepository;
        }

        public async Task<RfIdReaderDetailsResponse> Handle(GetRfIdReaderQuery request, CancellationToken cancellationToken)
        {
            return await _rfIdReaderRepository.GetRfIdReaderById(request.Id);
        }
    }
}