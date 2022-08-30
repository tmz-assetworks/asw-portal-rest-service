using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
    
    public class EnergyUsedsResponse
    {

        public long Counts { get; set; }
        public int EndMeterValue { get; set; }
        public string?  svalue { get; set; }
        public string? times { get; set; }
    }
    public class EnergyUsedBOForChartResponse
    {
        public EnergyUsedBOForChartResponse()
        {
            data = new List<EnergyUsedsResponse>();
        }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<EnergyUsedsResponse> data { get; set; }
    }
}
