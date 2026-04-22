using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.EnumData
{
    public static class EnumControlTexts
    {
        public enum DisplayingLabels
        {
            //chargingInfustructure
            [Display(Name = "Charging Infustructure")]
            chargingInfustructure = 1,
            [Display(Name = "Total Location")]
            TotalLocation = 2,
            [Display(Name = "Total Charger")]
            TotalCharger = 3,

            //Revenue 
            [Display(Name = "Revenue")]
            Revenue = 2,
            [Display(Name = "Total Cost")]
            TotalRevenue = 4,
            [Display(Name = "Daily Cost")]
            DailyRevenue = 5,
            [Display(Name = "Today's Cost")]
            TodaysRevenue = 6,

            // EnergyUsed
            [Display(Name = "Energy Used")]
            EnergyUsed = 7,
            [Display(Name = "Total Energy")]
            TotalEnergy = 8,
            [Display(Name = "Daily Average")]
            DailyAverage = 9,
            [Display(Name = "Today")]
            Todays = 10,

            // Energy Points
            [Display(Name = "MT of CO2 Saved")]
            MTofco2Saved = 11,

            [Display(Name = "Gasoline Gallon Equivalent(GGE Saved)")]
            GGEofGasSaved = 12,
            // 
        }
    }
}
