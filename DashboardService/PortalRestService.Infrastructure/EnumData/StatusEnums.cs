using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Application
{
    public class Status_Indication
    {
        public enum ChargerStatus
        {
            [Display(Name = "Available")]
            Available = 1,
            [Display(Name = "Connected")]
            Connected = 2,
            [Display(Name = "Offline")]
            Offline = 3,
            [Display(Name = "Active")]
            Active = 4,
            [Display(Name = "Abort")]
            Abort = 5,
            [Display(Name = "Faulted")]
            Faulted = 6,
            [Display(Name = "Busy")]
            Busy = 7

        }

        public enum LocationStatus
        {
            [Display(Name = "Commissioned")]
            Commissioned = 1,
            [Display(Name = "Under Maintenance")]
            UnderMaintenance = 3,
            [Display(Name = "Upcoming")]
            Upcoming = 2,
            [Display(Name = "Decommissioned")]
            Decommissioned = 4,
            [Display(Name = "Installed")]
            Installed = 5,
            [Display(Name = "Live")]
            Live = 6,



        }
        public enum ChargingSessionStatus
        {
            [Display(Name = "Cancelled")]
            Cancelled = 1,
            [Display(Name = "Interrupted")]
            Interrupted = 2,
            [Display(Name = "Completed")]
            Completed = 3,
            [Display(Name = "Charging")]
            Charging = 4
        }


        public enum Errors
        {
            Critical = 1,
            High = 2,
            Medium = 3
        }

      
    }
}
