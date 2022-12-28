using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public partial class Location
    {
       
        public long Id { get; set; }

      
        public long LocationAddressId { get; set; }
        public LocationAddress LocationAddress { get; set; }

        
        public long LocationStatusId { get; set; }
        public  LocationStatus? LocationStatus { get; set; }
        public string DepartmentName { get; set; }

      
        public string LocationId { get; set; }

     
        public string Email { get; set; }

       
        public string AlternateMobileNumber { get; set; }

      
        public string ContactPersonName { get; set; }

       
        public string ContactPersonNumber { get; set; }


       
        public string GlobalTax { get; set; }


        
        public string TotalCapacity { get; set; }

       
        public string UtilityService { get; set; }


       
        public string CreatedBy { get; set; }

       
        public DateTime CreatedOn { get; set; }

      
        public string Description { get; set; }

      
        public bool IsActive { get; set; }

        public string ModifiedBy { get; set; }

    
        public DateTime ModifiedOn { get; set; }

        public string LocationName { get; set; }

   
        public string TimeZone { get; set; }

        public string FuelProtectType { get; set; }



        //public virtual ICollection<LocationSchedule> LocationSchedule { get; set; }
        public virtual ICollection<OperatorUserMapper>? OperatorUserMapper { get; set; }
    }
}
