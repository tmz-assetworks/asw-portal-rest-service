using MediatR;
using PortalRestService.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application.Queries
{
    public class UpdateIsReadEventLogByIDQuery : IRequest<EventLogLocationResponse>
    {
        public int Id { get; set; }
       
        public UpdateIsReadEventLogByIDQuery(int id)
        {
            Id = id;
        }
    }
}
