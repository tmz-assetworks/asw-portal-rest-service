using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public partial class LocationAddress
    {
       
        public long Id { get; set; }

       
        public string AddressLine1 { get; set; }

       
        public string AddressLine2 { get; set; }

        
        public string CityName { get; set; }

      
        public long CountryId { get; set; }

      
        public string CountryName { get; set; }

        public string CreatedBy { get; set; }

      
        public DateTime CreatedOn { get; set; }

      
        public bool IsActive { get; set; }

   
        public string LandlineNumber { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string ModifiedBy { get; set; }

      
        public DateTime ModifiedOn { get; set; }

        
        public string PinCode { get; set; }

      
        public long StateId { get; set; }

        public string StateName { get; set; }
    }
}
