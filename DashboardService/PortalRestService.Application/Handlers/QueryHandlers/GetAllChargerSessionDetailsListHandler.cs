using MediatR;
using PortalRestService.Application.Queries;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Handlers.QueryHandlers
{
    public class GetAllChargerSessionDetailsListHandler : IRequestHandler<GetChargerSessionDetailsListQuery, PagedList<ChargerSessionDetailsList>>
    {
        private readonly IGetChargerSessionDetailsListRepository _chargerSessionDetailsListRepository;

        public GetAllChargerSessionDetailsListHandler(IGetChargerSessionDetailsListRepository chargerSessionDetailsListRepository)
        {
            _chargerSessionDetailsListRepository = chargerSessionDetailsListRepository;
        }


        public async Task<PagedList<ChargerSessionDetailsList>> Handle(GetChargerSessionDetailsListQuery request, CancellationToken cancellationToken)
        {
            return (PagedList<ChargerSessionDetailsList>)await _chargerSessionDetailsListRepository.GetChargerSessionDetailsList(request.chargerSessionListRequest);
        }
    }
}
