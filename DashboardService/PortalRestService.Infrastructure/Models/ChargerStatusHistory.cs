using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class ChargerStatusHistory
    {
        public int Id { get; set; }
        public int ChargerId { get; set; }
        public string ChargerStatus { get; set; } = null!;
        public int? ConnectorId { get; set; }
        public string? ConnectorStatus { get; set; }
        public string Operation { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
