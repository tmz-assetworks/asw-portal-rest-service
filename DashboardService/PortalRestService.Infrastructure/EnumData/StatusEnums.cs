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
            Busy = 7,
            [Display(Name = "Unavailable")]
            Unavailable = 8

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
            [Display(Name = "Inactive")]
            Inactive = 7,



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
            Charging = 4,
            [Display(Name = "Aborted")]
            Aborted = 5

        }


        public enum Errors
        {
            Critical = 1,
            High = 2, 
            Medium = 3
        }
         public enum EventlogRequest
        {
            [Display(Name = "Authorize")]
            Authorize = 1,
            [Display(Name = "Heartbeat")]
            Heartbeat = 2,
            [Display(Name = "StatusNotification")]
            StatusNotification = 3,
            [Display(Name = "GetConfiguration")]
            GetConfiguration = 4,
            [Display(Name = "GetLocalListVersion")]
            GetLocalListVersion = 5,
            [Display(Name = "ClearCache")]
            ClearCache = 6,
            [Display(Name = "MeterValues")]
            MeterValues = 7,
            [Display(Name = "StopTransaction")]
            StopTransaction = 8,
            [Display(Name = "StartTransaction")]
            StartTransaction = 9,
            [Display(Name = "RemortStartTransaction")]
            RemortStartTransaction = 10,
            [Display(Name = "RemoteStopTransaction")]
            RemoteStopTransaction = 11,
            [Display(Name = "GetCompositeSchedule")]
            GetCompositeSchedule = 12,
            [Display(Name = "ChangeConfiguration")]
            ChangeConfiguration = 13,
            [Display(Name = "ChangeAvailability")]
            ChangeAvailability = 14,
            [Display(Name = "GetDiagnostics")]
            GetDiagnostics = 15,
            [Display(Name = "SendLocalList")]
            SendLocalList = 16,
            [Display(Name = "TriggerMessage")]
            TriggerMessage = 17,
            [Display(Name = "UnlockConnector")]
            UnlockConnector = 18,
            [Display(Name = "UpdateFirmware")]
            UpdateFirmware = 19,
            [Display(Name = "FirmwareStatusNotification")]
            ReserveNow = 20,
            [Display(Name = "SetChargingProfile")]
            SetChargingProfile = 21,
            [Display(Name = "RemoteStartTransaction")]
            RemoteStartTransaction = 22,
            [Display(Name = "Reset")]
            Reset = 23,
            [Display(Name = "BootNotification")]
            BootNotification = 24
        }
        

    }
    
}
