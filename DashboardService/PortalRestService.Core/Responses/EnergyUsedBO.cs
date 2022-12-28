using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Entities.Charger
{

    public class EnergyUsedChartBO
    {

        public double StartMeterValue { get; set; }
        public double EndMeterValue { get; set; }
        public string chargeboxId { get; set; }
        public string? svalue { get; set; }
        public string? times { get; set; }
    }

}
