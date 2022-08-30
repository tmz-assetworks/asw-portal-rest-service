using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class ChargerResponse
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? DeviceId { get; set; }
        public string? RequestId { get; set; }
        public string? ResponsePayload { get; set; }
        public string? ResponseType { get; set; }
    }
}
