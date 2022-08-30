using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class ChargerConfigDetail
    {
        public int Id { get; set; }
        public int ChargerId { get; set; }
        public string? GetConfigurationResponsePayload { get; set; }
        public bool? IsBootAccepted { get; set; }
        public bool? IsDefault { get; set; }
        public int? MaxKey { get; set; }
    }
}
