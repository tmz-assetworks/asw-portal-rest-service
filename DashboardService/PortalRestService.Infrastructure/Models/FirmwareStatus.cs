using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class FirmwareStatus
    {
        public int Id { get; set; }
        public int ChargerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? FirmwareStatus1 { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
