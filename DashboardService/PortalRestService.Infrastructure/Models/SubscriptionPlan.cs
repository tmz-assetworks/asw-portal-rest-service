using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public partial class SubscriptionPlan
    {
        
        public long Id { get; set; }

       
        public long CustomerId { get; set; }
       

        public string SubscriptionPlanName { get; set; }


     
        public long CurrencyId { get; set; }
      
     
       

        public long UnitId { get; set; }

        public  PriceType PriceType { get; set; }
        public DateTime ValidFrom { get; set; }


        public DateTime ValidTo { get; set; }

        public long? SubscriptionsGroupId { get; set; }

        public long PriceTypeId { get; set; }


        public string SubscriptionsDetails { get; set; }

        public double Price { get; set; }

    
        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }


        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
