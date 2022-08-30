using MediatR;
using PortalRestService.Core.PagingHelper;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class GetChargerSessionDetailsListQuery : IRequest<PagedList<ChargerSessionDetailsList>>
    {
        public ChargerSessionListRequest chargerSessionListRequest { get; set; }
        public GetChargerSessionDetailsListQuery(ChargerSessionListRequest chargerSessionDetailsListRequest)
        {
            this.chargerSessionListRequest = chargerSessionDetailsListRequest;
        }
    }
}
