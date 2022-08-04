using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.EnumData
{
    public static class ColorsEnum
    {

        public enum LocationsColor
        {
            [Display(Name = "#757575")]
            Commissioned = 1,

            [Display(Name = "#757234")]
            DeCommissioned = 2,

            [Display(Name = "#E97454")]
            UnderMaintenance = 3,

            [Display(Name = "#0062A6")]
            Upcoming = 4,

            [Display(Name = "#346432")]
            Installed =5,

            [Display(Name = "#088532")]
            Live = 6,

        }

        public enum ChargerStatus
        {
            [Display(Name = "#90993F")]
            Available = 1,

            [Display(Name = "#E97300")]
            Connected = 2,

            [Display(Name = "#757575")]
            Offline = 3,

            [Display(Name = "#675633")]
            Active = 4,

            [Display(Name = "#345653")]
            Abort = 5,
            
            [Display(Name = "#757575")]
            Faulted = 6,

            [Display(Name = "#E97300")]  //or Conected
            Busy = 7,
        }


        public enum ChargingSessionsColor
        {
            [Display(Name = "#EA002A")]
            Cancelled = 1,

            [Display(Name = "#E97300")]
            Interrupted = 2,

            [Display(Name = "#90993F")]
            Completed = 3,
            [Display(Name = "#90993F")]
            Charging = 4,

        }
        public enum ErrorsColor
        {
            [Display(Name = "#E97300")]
            Critical = 1,

            [Display(Name = "#EA002A")]
            High = 2,

            [Display(Name = "#0062A6")]
            Medium = 3,
        }

        public enum AlertsColor
        {
            [Display(Name = "#E97300")]
            Critical = 5,

            [Display(Name = "#EA002A")]
            High = 3,

            [Display(Name = "#0062A6")]
            Medium = 2,
        }
        public enum ChargingSession
        {
            [Display(Name = "#757575")]
            Commissioned = 1,

            [Display(Name = "#757974")]
            DeCommissioned = 2,

            [Display(Name = "#0062A6")]
            UnderMaintenance = 3,
            [Display(Name = "#90993F")]
            Charging = 4,


        }

    }
}
