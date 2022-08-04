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
           
            if(Status_Indication.LocationStatus.Commissioned.GetEnumDisplayName().ToLower() == value.ToLower())
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
            return "";
        }
    }
}
