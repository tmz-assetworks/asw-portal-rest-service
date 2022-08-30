using System;
using System.Collections.Generic;

namespace PortalRestService.Core.Models
{
    public partial class ChargerConfiguration
    {
        public int Id { get; set; }
        public string? DeviceSerialNumber { get; set; }
        public string? ChargePointModel { get; set; }
        public string? ChargePointSerialNumber { get; set; }
        public string? ChargePointVendor { get; set; }
        public int? ChargerId { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? Iccid { get; set; }
        public string? Imsi { get; set; }
        public string? MeterSerialNumber { get; set; }
        public string? MeterType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
