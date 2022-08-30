using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class Charger
    {
        public int Id { get; set; }
        public string? DeviceId { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
