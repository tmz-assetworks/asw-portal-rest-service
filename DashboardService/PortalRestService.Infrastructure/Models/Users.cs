using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
    public partial class Users
    {
        
        public long Id { get; set; }

       
        public string ObjectId { get; set; }

       
        public string userPrincipalName { get; set; }

       
        public string name { get; set; }

     
        public string EmailId { get; set; }

     
        public long PhoneNumber { get; set; }

      
      
        public string AddressLine1 { get; set; }

      
        public string AddressLine2 { get; set; }

      
     
    }
}
