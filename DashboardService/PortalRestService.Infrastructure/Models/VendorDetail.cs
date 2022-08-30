using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class VendorDetail
    {
        public int Id { get; set; }
        public string? MessageId { get; set; }
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
    }
}
