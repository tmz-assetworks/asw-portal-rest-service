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

            [Display(Name = "#E97259")]  //or Unavailable
            Unavailable = 8,
        }


        public enum ChargingSessionsColor
        {
            [Display(Name = "#EA002A")]
            Cancelled = 1,

            [Display(Name = "#E97300")]
            Interrupted = 2,

            [Display(Name = "#90993F")]
            Completed = 3,
            [Display(Name = "#0062A6")]
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
        public enum EventlogColor
        {
            [Display(Name = "#757575")]
            Authorize = 1,

            [Display(Name = "#757234")]
            Heartbeat = 2,

            [Display(Name = "#E97454")]
            StatusNotification = 3,

            [Display(Name = "#0062A6")]
            GetConfiguration = 4,

            [Display(Name = "#346432")]
            GetLocalListVersion = 5,

            [Display(Name = "#00FFFF")]
            ClearCache = 7,
            [Display(Name = "#FF0000")]
            MeterValues = 8,
            [Display(Name = "#0000FF")]
            StopTransaction = 9,
            [Display(Name = "#00008B")]
            StartTransaction = 10,
            [Display(Name = "#ADD8E6")]
            RemoteStopTransaction = 11,
            [Display(Name = "#800080")]
            GetCompositeSchedule = 12,
            [Display(Name = "#FFFF00")]
            ChangeConfiguration = 13,
            [Display(Name = "#00FF00")]
            ChangeAvailability = 14,
            [Display(Name = "#FF00FF")]
            GetDiagnostics = 15,
            [Display(Name = "#FFC0CB")]
            SendLocalList = 16,
            [Display(Name = "#FFFFFF")]
            TriggerMessage = 17,
            [Display(Name = "#FFFFFF")]
            UnlockConnector = 18,
            [Display(Name = "#808080")]
            UpdateFirmware = 19,
            [Display(Name = "#000000")]
            ReserveNow = 20,
            [Display(Name = "#FFA500")]
            SetChargingProfile = 21,
            [Display(Name = "#FFA500")]
            RemoteStartTransaction = 22,
            [Display(Name = "#800000")]
            Reset = 23,
            [Display(Name = "#456463")]
            BootNotification = 24,

        }

    }
}
