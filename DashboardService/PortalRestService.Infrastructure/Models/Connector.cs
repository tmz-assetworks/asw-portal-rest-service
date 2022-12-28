using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class Connector
    {
     

       
        public long Id { get; set; }

       
        public string CreatedBy { get; set; }

       
        public string ConnectorType { get; set; }

     
        public string Color { get; set; }

       
        public DateTime CreatedOn { get; set; }

       
        public string ModifiedBy { get; set; }


       
        public DateTime ModifiedOn { get; set; }
    }
}
