using PortalRestService.Application;
using PortalRestService.Infrastructure.EnumData;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PortalRestService.Helper
{
    public static class Extensions
    {

        public static string GetEnumDisplayName(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].GetName();
            else
                return value.ToString();
        }

        public static string GetColorCodesByStatus(this string value)
        {

            if (Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.LocationsColor.Commissioned.GetEnumDisplayName();
            }
            if (Status_Indication.LocationStatus.Decommissioned.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.LocationsColor.DeCommissioned.GetEnumDisplayName();
            }

            if (Status_Indication.LocationStatus.Upcoming.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.LocationsColor.Upcoming.GetEnumDisplayName();
            }

            if (Status_Indication.LocationStatus.Installed.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.LocationsColor.Installed.GetEnumDisplayName();
            }
            if (Status_Indication.LocationStatus.UnderMaintenance.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.LocationsColor.UnderMaintenance.GetEnumDisplayName();
            }
            if (Status_Indication.LocationStatus.Live.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.LocationsColor.Live.GetEnumDisplayName();
            }
            return "";
        }

        public static string GetColorCodesByChargingSession(this string value)
        {

            if (Status_Indication.ChargingSessionStatus.Cancelled.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargingSessionsColor.Cancelled.GetEnumDisplayName();
            }
            if (Status_Indication.ChargingSessionStatus.Interrupted.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargingSessionsColor.Interrupted.GetEnumDisplayName();
            }
            if (Status_Indication.ChargingSessionStatus.Completed.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargingSessionsColor.Completed.GetEnumDisplayName();
            }
            if (Status_Indication.ChargingSessionStatus.Charging.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargingSessionsColor.Charging.GetEnumDisplayName();
            }
            if (Status_Indication.ChargingSessionStatus.Aborted.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargingSessionsColor.Aborted.GetEnumDisplayName();
            }
            return "";
        }
        public static string GetColorCodesByCharger(this string value)
        {

            if (Status_Indication.ChargerStatus.Available.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Available.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Connected.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Connected.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Offline.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Offline.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Active.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Active.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Abort.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Abort.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Faulted.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Faulted.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Busy.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Busy.GetEnumDisplayName();
            }
            if (Status_Indication.ChargerStatus.Unavailable.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.ChargerStatus.Unavailable.GetEnumDisplayName();
            }
            return "";
        }

        public static string GetEventlogColorCodes(this string value)
        {

            if (Status_Indication.EventlogRequest.Authorize.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.Authorize.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.Heartbeat.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.Heartbeat.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.StatusNotification.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.StatusNotification.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.GetConfiguration.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.GetConfiguration.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.GetLocalListVersion.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.GetLocalListVersion.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.ClearCache.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.ClearCache.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.MeterValues.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.MeterValues.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.StopTransaction.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.StopTransaction.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.StartTransaction.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.StartTransaction.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.RemoteStopTransaction.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.RemoteStopTransaction.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.GetCompositeSchedule.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.GetCompositeSchedule.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.ChangeConfiguration.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.ChangeConfiguration.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.ChangeAvailability.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.ChangeAvailability.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.GetDiagnostics.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.GetDiagnostics.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.SendLocalList.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.SendLocalList.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.TriggerMessage.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.TriggerMessage.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.UnlockConnector.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.UnlockConnector.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.UpdateFirmware.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.UpdateFirmware.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.ReserveNow.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.ReserveNow.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.SetChargingProfile.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.SetChargingProfile.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.RemoteStartTransaction.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.RemoteStartTransaction.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.Reset.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.Reset.GetEnumDisplayName();
            }
            if (Status_Indication.EventlogRequest.BootNotification.GetEnumDisplayName().ToLower() == value.ToLower())
            {
                return ColorsEnum.EventlogColor.BootNotification.GetEnumDisplayName();
            }
            return "";
        }
    }
}
