using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class OcppEventLog
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? DeviceId { get; set; }
        public string? EventLogDataSource { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? RequestId { get; set; }
        public string? RequestPayload { get; set; }
        public string? RequestType { get; set; }
        public string? ResponsePayload { get; set; }
        public bool? IsRead { get; set; }
        public string? ErrorCode { get; set; }


    }
    public partial class OcppEventLogErrorName
    {
        public string errorCode { get; set; }
    }
}
