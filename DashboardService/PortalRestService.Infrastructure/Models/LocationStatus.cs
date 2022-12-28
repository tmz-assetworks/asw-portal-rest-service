using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public partial class LocationStatus
    {
        
        public long Id { get; set; }

        
        public string LocationStatusName { get; set; }

        
        public string CreatedBy { get; set; }

       
        public DateTime CreatedOn { get; set; }

        
        public bool IsActive { get; set; }

        
        public string ModifiedBy { get; set; }

       
        public DateTime ModifiedOn { get; set; }
    }
}
