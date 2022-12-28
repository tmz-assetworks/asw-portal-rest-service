using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public partial class Vehicle
    {
        
        public long Id { get; set; }

        public string VIN { get; set; }

        public string LicencePlate { get; set; }
       
        public string Department { get; set; }

        public string DomicileLocation { get; set; }

        public string VehicleMacAddress { get; set; }

        public bool IsActive { get; set; }

      
        public string CreatedBy { get; set; }

     
        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public long ModelYear { get; set; }          

      
        public string MakeName { get; set; }

        public string ModelName { get; set; }

        public string? UnitNumber { get; set; }
        /// <summary>
        public  ICollection<VehicleRFID> vehicleRFID { get; set; }

        public List<ApplicableSubscriptionPlan> applicableSubscriptionPlans { get; set; }
    }
}
