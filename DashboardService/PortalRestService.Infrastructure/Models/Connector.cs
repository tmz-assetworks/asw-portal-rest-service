using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class Connector
    {
        public int Id { get; set; }
        public int ChargerId { get; set; }
        public int ConnectorId { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
