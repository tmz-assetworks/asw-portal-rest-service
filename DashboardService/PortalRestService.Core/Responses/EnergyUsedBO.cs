using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Entities.Charger
{

    public class EnergyUsedChartBO
    {

        public int StartMeterValue { get; set; }
        public int EndMeterValue { get; set; }

        public string? times { get; set; }
    }

}
