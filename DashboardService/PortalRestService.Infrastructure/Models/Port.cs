using PortalRestService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.Models
{
   public partial class Port
    {
        
        public long Id { get; set; }

       
        public int ChargerId { get; set; }
       
        public int Connectorid { get; set; }

       
        public long ConnectorType { get; set; }
        public Connector Connector { get; set; }


        public string CreatedBy { get; set; }


       
        public DateTime CreatedOn { get; set; }



      
        public long IncrementalPower { get; set; }


       
        public bool IsActive { get; set; }


     
        public long MaxPower { get; set; }


       
        public long MinPower { get; set; }


      
        public string ModifiedBy { get; set; }


        public DateTime ModifiedOn { get; set; }


       
        public long ChargerTypeId { get; set; }
       

        public string PortName { get; set; }


        public long Power { get; set; }
    }
}
