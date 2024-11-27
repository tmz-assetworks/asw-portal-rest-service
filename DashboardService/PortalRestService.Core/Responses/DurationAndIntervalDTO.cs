using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
    public class DurationAndIntervalDTO
    {
        public string laveltype { get; set; }
        public string duration { get; set; }
        public TimeSpan interval { get; set; }
    }
}
