using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class ChargerStatus
    {
        public int Id { get; set; }
        public int? ChargerId { get; set; }
        public string Chargerstatus { get; set; }
       
        public int? ConnectorId { get; set; }
        public string ConnectorStatus { get; set; }
        public int? ReservationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? ReservationExpiryDate { get; set; }
        public string? IdTag { get; set; }

    }
}
