using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class Rfid
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsBlocked { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? RfidNumber { get; set; }
        public int UserId { get; set; }
    }
}
