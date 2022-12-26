using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
    public partial class SubscriptionsGroup
    {
        
        public long Id { get; set; }

       
        public string SubscriptionGroupName { get; set; }

       
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

      
        public DateTime CreatedOn { get; set; }

      
        public string ModifiedBy { get; set; }

     
        public DateTime ModifiedOn { get; set; }
    }
}
