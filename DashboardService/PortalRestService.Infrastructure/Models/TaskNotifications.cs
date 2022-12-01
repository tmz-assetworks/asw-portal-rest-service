using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Models
{
    public partial class TaskNotifications
    {
        public int Id { get; set; }
        public  string Category { get; set; }
        public string Messagetype { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public bool IsActive { get; set; }
        public string Ipaddress { get; set; }
        public string UserId { get; set; }

    }
}
