using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public class ErrorSeverity
    {
        public int Id { get; set; }
        public string Names { get; set; }
        public bool IsActive { get; set; }
    }
}
