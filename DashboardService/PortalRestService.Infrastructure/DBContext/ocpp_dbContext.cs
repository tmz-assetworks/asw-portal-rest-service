using Microsoft.EntityFrameworkCore;
using PortalRestService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalRestService.Infrastructure.DBContext
{
    public partial class ocpp_dbContext : DbContext
    {
        public ocpp_dbContext()
        {
        }

        public ocpp_dbContext(DbContextOptions<ocpp_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Charger> Chargers { get; set; } = null!;
        public virtual DbSet<ChargerConfigDetail> ChargerConfigDetails { get; set; } = null!;
        public virtual DbSet<ChargerConfiguration> ChargerConfigurations { get; set; } = null!;
        public virtual DbSet<ChargerResponse> ChargerResponses { get; set; } = null!;
        public virtual DbSet<ChargerStatus> ChargerStatuses { get; set; } = null!;
        public virtual DbSet<ChargerStatusHistory> ChargerStatusHistories { get; set; } = null!;
        public virtual DbSet<ChargingSession> ChargingSessions { get; set; } = null!;
        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<Connector> Connectors { get; set; } = null!;
        public virtual DbSet<DiagnosticReport> DiagnosticReports { get; set; } = null!;
        public virtual DbSet<FirmwareStatus> FirmwareStatuses { get; set; } = null!;
        public virtual DbSet<MeterValue> MeterValues { get; set; } = null!;
        public virtual DbSet<OcppEventLog> OcppEventLogs { get; set; } = null!;
        public virtual DbSet<Rfid> Rfids { get; set; } = null!;
        public virtual DbSet<VendorDetail> VendorDetails { get; set; } = null!;

      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Charger>(entity =>
            {
                entity.ToTable("charger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(255)
                    .HasColumnName("device_id");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            });

            modelBuilder.Entity<ChargerConfigDetail>(entity =>
            {
                entity.ToTable("charger_config_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.GetConfigurationResponsePayload).HasColumnName("get_configuration_response_payload");

                entity.Property(e => e.IsBootAccepted).HasColumnName("is_boot_accepted");

                entity.Property(e => e.IsDefault).HasColumnName("is_default");

                entity.Property(e => e.MaxKey).HasColumnName("max_key");
            });

            modelBuilder.Entity<ChargerConfiguration>(entity =>
            {
                entity.ToTable("charger_configurations");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargePointModel)
                    .HasMaxLength(20)
                    .HasColumnName("charge_point_model");

                entity.Property(e => e.ChargePointSerialNumber)
                    .HasMaxLength(25)
                    .HasColumnName("charge_point_serial_number");

                entity.Property(e => e.ChargePointVendor)
                    .HasMaxLength(50)
                    .HasColumnName("charge_point_vendor");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeviceSerialNumber)
                    .HasMaxLength(25)
                    .HasColumnName("device_serial_number");

                entity.Property(e => e.FirmwareVersion)
                    .HasMaxLength(50)
                    .HasColumnName("firmware_version");

                entity.Property(e => e.Iccid)
                    .HasMaxLength(20)
                    .HasColumnName("iccid");

                entity.Property(e => e.Imsi)
                    .HasMaxLength(20)
                    .HasColumnName("imsi");

                entity.Property(e => e.MeterSerialNumber)
                    .HasMaxLength(25)
                    .HasColumnName("meter_serial_number");

                entity.Property(e => e.MeterType)
                    .HasMaxLength(25)
                    .HasColumnName("meter_type");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            });

            modelBuilder.Entity<ChargerResponse>(entity =>
            {
                entity.ToTable("charger_response");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(255)
                    .HasColumnName("device_id");

                entity.Property(e => e.RequestId)
                    .HasMaxLength(255)
                    .HasColumnName("request_id");

                entity.Property(e => e.ResponsePayload)
                    .HasMaxLength(2600)
                    .HasColumnName("response_payload");

                entity.Property(e => e.ResponseType)
                    .HasMaxLength(255)
                    .HasColumnName("response_type");
            });

            modelBuilder.Entity<ChargerStatus>(entity =>
            {
                entity.ToTable("charger_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.ChargerStatus1)
                    .HasMaxLength(255)
                    .HasColumnName("charger_status");

                entity.Property(e => e.ConnectorId).HasColumnName("connector_id");

                entity.Property(e => e.ConnectorStatus)
                    .HasMaxLength(255)
                    .HasColumnName("connector_status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            });

            modelBuilder.Entity<ChargerStatusHistory>(entity =>
            {
                entity.ToTable("charger_status_history");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.ChargerStatus)
                    .HasMaxLength(255)
                    .HasColumnName("charger_status");

                entity.Property(e => e.ConnectorId).HasColumnName("connector_id");

                entity.Property(e => e.ConnectorStatus)
                    .HasMaxLength(255)
                    .HasColumnName("connector_status");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Operation)
                    .HasMaxLength(255)
                    .HasColumnName("operation");
            });

            modelBuilder.Entity<ChargingSession>(entity =>
            {
                entity.ToTable("charging_sessions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.ChargingCost).HasColumnName("charging_cost");

                entity.Property(e => e.ChargingStatus)
                    .HasMaxLength(255)
                    .HasColumnName("charging_status");

                entity.Property(e => e.ConnectorId).HasColumnName("connector_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(255)
                    .HasColumnName("device_id");

                entity.Property(e => e.EndMeterValue).HasColumnName("end_meter_value");

                entity.Property(e => e.EndSoc).HasColumnName("end_soc");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");

                entity.Property(e => e.ReasonForStop)
                    .HasMaxLength(255)
                    .HasColumnName("reason_for_stop");

                entity.Property(e => e.StartMeterValue).HasColumnName("start_meter_value");

                entity.Property(e => e.StartSoc).HasColumnName("start_soc");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<Connector>(entity =>
            {
                entity.ToTable("connector");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.ConnectorId).HasColumnName("connector_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            });

            modelBuilder.Entity<DiagnosticReport>(entity =>
            {
                entity.ToTable("diagnostic_reports");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .HasColumnName("location");
            });

            modelBuilder.Entity<FirmwareStatus>(entity =>
            {
                entity.ToTable("firmware_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.FirmwareStatus1)
                    .HasMaxLength(255)
                    .HasColumnName("firmware_status");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            });

            modelBuilder.Entity<MeterValue>(entity =>
            {
                entity.ToTable("meter_values");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChargerId).HasColumnName("charger_id");

                entity.Property(e => e.ChargingSessionId).HasColumnName("charging_session_id");

                entity.Property(e => e.ConnectorId).HasColumnName("connector_id");

                entity.Property(e => e.Context)
                    .HasMaxLength(30)
                    .HasColumnName("context");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Format)
                    .HasMaxLength(30)
                    .HasColumnName("format");

                entity.Property(e => e.Location)
                    .HasMaxLength(30)
                    .HasColumnName("location");

                entity.Property(e => e.Measurand)
                    .HasMaxLength(31)
                    .HasColumnName("measurand");

                entity.Property(e => e.Phase)
                    .HasMaxLength(30)
                    .HasColumnName("phase");

                entity.Property(e => e.Unit)
                    .HasMaxLength(30)
                    .HasColumnName("unit");

                entity.Property(e => e.Value)
                    .HasMaxLength(50)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<OcppEventLog>(entity =>
            {
                entity.ToTable("ocpp_event_logs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(255)
                    .HasColumnName("device_id");

                entity.Property(e => e.EventLogDataSource)
                    .HasMaxLength(255)
                    .HasColumnName("event_log_data_source");

                entity.Property(e => e.IsRead).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");

                entity.Property(e => e.RequestId)
                    .HasMaxLength(255)
                    .HasColumnName("request_id");

                entity.Property(e => e.RequestPayload)
                    .HasMaxLength(2600)
                    .HasColumnName("request_payload");

                entity.Property(e => e.RequestType)
                    .HasMaxLength(255)
                    .HasColumnName("request_type");

                entity.Property(e => e.ResponsePayload)
                    .HasMaxLength(2600)
                    .HasColumnName("response_payload");
            });

            modelBuilder.Entity<Rfid>(entity =>
            {
                entity.ToTable("rfid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.IsBlocked).HasColumnName("is_blocked");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");

                entity.Property(e => e.RfidNumber)
                    .HasMaxLength(255)
                    .HasColumnName("rfid_number");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<VendorDetail>(entity =>
            {
                entity.ToTable("vendor_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MessageId)
                    .HasMaxLength(255)
                    .HasColumnName("message_id");

                entity.Property(e => e.VendorId)
                    .HasMaxLength(255)
                    .HasColumnName("vendor_id");

                entity.Property(e => e.VendorName)
                    .HasMaxLength(255)
                    .HasColumnName("vendor_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
