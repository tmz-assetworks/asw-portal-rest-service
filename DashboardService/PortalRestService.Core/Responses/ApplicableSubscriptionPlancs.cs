using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
    public partial class ApplicableSubscriptionPlancs
    {
        public string SubscriptionPlanName { get; set; }
        public string Type { get; set; }
        public string SubscriptionsValue { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string RfIdNumbers { get; set; }
    }
}
