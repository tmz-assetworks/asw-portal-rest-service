using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
    public class NotificationResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public long Counts { get; set; }

    }
    public class NotificationRequest
    {
        public string UserId { get; set; }
    }
}
