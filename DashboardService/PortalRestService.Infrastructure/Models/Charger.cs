using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class Charger
    {
    
        public int Id { get; set; }

    
        public string AssetId { get; set; }

       
        public string EndPointUrl { get; set; }

    
        public string FirmwareVersion { get; set; }

        public string HardwareSerialNumber { get; set; }

   
      
        public bool IsActive { get; set; }

   
       
        public bool IsAutomatic { get; set; }

        public string MeterType { get; set; }

        public bool MultiplePorts { get; set; }

      
        public string PingSchedule { get; set; }

      
       

       
       
    
       
        public string ChargeBoxId { get; set; }

        
     
      
        public string? CreatedBy { get; set; }

      
        public DateTime? InstallationDate { get; set; }

   
      
        public DateTime? CreatedOn { get; set; }
       
      
        public string? ModifiedBy { get; set; }
       
        public DateTime? ModifiedOn { get; set; }
     
    }
}
