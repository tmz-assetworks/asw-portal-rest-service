using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Entities.Charger
{
  
    public class ChargingSessionByLocationChartBO
    {

        public long Counts { get; set; }
        public string ChargingStatus { get; set; }

        public string? times { get; set; }
        public string?  svalue { get; set; }
        public string? Color { get; set; }
    }
    public class ChargingSessionByLocationForChartResponse
    {
        public ChargingSessionByLocationForChartResponse()
        {
            data = new List<ChargingSessionByLocationChartBO>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ChargingSessionByLocationChartBO> data { get; set; }
    }

}
