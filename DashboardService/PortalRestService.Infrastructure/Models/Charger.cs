using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PortalRestService.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PortalRestService.Core.Models
{
    public partial class Charger
    {
        public int Id { get; set; }
        public string AssetId { get; set; }

        public string EndPointUrl { get; set; }
        public string FirmwareVersion { get; set; }
        public string HardwareSerialNumber { get; set; }

        public bool IsActive { get; set; }

        public bool IsAutomatic { get; set; }
        public string? MakeName { get; set; }
        //public LocationAddress LocationAddress { get; set; }
        public string? ModelName { get; set; }
        public string? MeterType { get; set; }
        public bool MultiplePorts { get; set; }
        public string? PingSchedule { get; set; }
        public bool FleetStation { get; set; }
        public string? ReadingSchedule { get; set; }
        public long? LocationId { get; set; }
         public  Location? Location { get; set; }
        public string ChargeBoxId { get; set; }
        public long? RFIDReaderId { get; set; }
        public long? PowerCabinetId { get; set; }
        //public virtual PowerCabinet PowerCabinet { get; set; }

        public long? PadId { get; set; }
        public long? ModemId { get; set; }
        //public virtual Modem Modem { get; set; }
        //public virtual Pad Pad { get; set; }
        public string ProtocolName { get; set; }
        public long? CableId { get; set; }
        //public virtual Cable Cable { get; set; }
        public long? SwitchGearId { get; set; }
        //public virtual SwitchGear SwitchGear { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? InstallationDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public virtual ICollection<Port> Ports { get; set; }
        public virtual List<ChargerStatus> ChargerStatuses { get; set; }
        public virtual ICollection<ChargerStatusHistory> ChargerStatusHistories { get; set; }
    }

}
