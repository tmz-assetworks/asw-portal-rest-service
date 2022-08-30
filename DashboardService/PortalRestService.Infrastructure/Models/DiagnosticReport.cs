using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class DiagnosticReport
    {
        public int Id { get; set; }
        public int ChargerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? FileName { get; set; }
        public string? Location { get; set; }
    }
}
