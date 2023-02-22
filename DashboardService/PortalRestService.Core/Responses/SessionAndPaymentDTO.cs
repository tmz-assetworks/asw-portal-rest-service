using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Core.Responses
{
#pragma warning disable
    public class SessionAndPaymentDTO
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public SessionAndPaymentData sessionAndPaymentData { get; set; }
    }
    public class SessionAndPaymentData
    {
        public string AssetId { get; set; }
        public int AssetInternalId { get; set; } = 0;
        public string CardNumber { get; set; }
        public string ConnectorType { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime? EndDateTime { get; set; }
        public double FuelingSeconds { get; set; }
        public string HoseId { get; set; }
        public DateTime? IssueDateTime { get; set; }
        public int IssueQty { get; set; }
        public string? LocationId { get; set; }
        public int Meter1Reading { get; set; }
        public int ProductionId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string TankId { get; set; }
    }
}
