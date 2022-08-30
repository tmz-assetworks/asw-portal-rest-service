using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class MeterValue
    {
        public int Id { get; set; }
        public int ChargerId { get; set; }
        public int ChargingSessionId { get; set; }
        public int ConnectorId { get; set; }
        public string Location { get; set; } = null!;
        public string? Value { get; set; }
        public string Context { get; set; } = null!;
        public string Format { get; set; } = null!;
        public string? Measurand { get; set; }
        public string? Phase { get; set; }
        public string Unit { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
