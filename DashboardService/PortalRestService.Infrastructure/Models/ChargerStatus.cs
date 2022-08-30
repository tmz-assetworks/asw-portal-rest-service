using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class ChargerStatus
    {
        public int Id { get; set; }
        public int? ChargerId { get; set; }
        public string? ChargerStatus1 { get; set; }
        public int? ConnectorId { get; set; }
        public string? ConnectorStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
