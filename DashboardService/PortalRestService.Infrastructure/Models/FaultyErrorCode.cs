using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public class FaultyErrorCode
    {
        public int Id { get; set; }
        public string Names { get; set; }

        public int ErrorSeverityId { get; set; }
        public bool IsActive { get; set; }
    }
}
