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

         public virtual DbSet<ChargingSession> ChargingSessions { get; set; } = null!;
       
        public virtual DbSet<OcppEventLog> OcppEventLogs { get; set; } = null!;
        public virtual DbSet<ChargerStatus>  ChargerStatuses { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {






            modelBuilder.Entity<ChargingSession>(entity =>
            {
                entity.ToTable("ChargingSessions");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.ChargerId).HasColumnName("ChargerId");

                entity.Property(e => e.ChargingCost).HasColumnName("ChargingCost");

                entity.Property(e => e.ChargingStatus)
                    .HasMaxLength(255)
                    .HasColumnName("ChargingStatus");

                entity.Property(e => e.ConnectorId).HasColumnName("ConnectorId");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedOn");

                entity.Property(e => e.DeviceId)
                    .HasMaxLength(255)
                    .HasColumnName("DeviceId");

                entity.Property(e => e.EndMeterValue).HasColumnName("EndMeterValue");

                entity.Property(e => e.EndSoc).HasColumnName("EndSoc");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("EndTime");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("ModifiedOn");

                entity.Property(e => e.ReasonForStop)
                    .HasMaxLength(255)
                    .HasColumnName("ReasonForStop");

                entity.Property(e => e.StartMeterValue).HasColumnName("StartMeterValue");

                entity.Property(e => e.StartSoc).HasColumnName("StartSoc");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("StartTime");
            });

            modelBuilder.Entity<OcppEventLog>(entity =>
           {
               entity.ToTable("OcppEventLogs");

               entity.Property(e => e.Id)
                   .HasColumnName("Id");

               entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("CreatedOn");

               entity.Property(e => e.DeviceId).HasMaxLength(255).HasColumnName("DeviceId");
               entity.Property(e => e.EventLogDataSource).HasMaxLength(255).HasColumnName("EventLogDataSource");

               entity.Property(e => e.ModifiedAt).HasColumnType("datetime").HasColumnName("ModifiedOn");
               entity.Property(e => e.RequestId).HasMaxLength(255).HasColumnName("RequestId");
               entity.Property(e => e.RequestPayload).HasMaxLength(2600).HasColumnName("RequestPayload");
               entity.Property(e => e.RequestType).HasMaxLength(255).HasColumnName("RequestType");
               entity.Property(e => e.ResponsePayload).HasMaxLength(2600).HasColumnName("ResponsePayload");


           });
            modelBuilder.Entity<ChargerStatus>(entity =>
            {
                entity.ToTable("ChargerStatus");

                entity.Property(e => e.Id)
                    .HasColumnName("Id");

                entity.Property(e => e.ChargerId)
                .HasColumnName("ChargerId");

                entity.Property(e => e.Chargerstatus)
                    .HasMaxLength(255)
                    .HasColumnName("ChargerStatus");

                entity.Property(e => e.ConnectorId).HasColumnName("ConnectorId");
                entity.Property(e => e.ConnectorStatus)
                    .HasMaxLength(255)
                    .HasColumnName("ConnectorStatus");
                entity.Property(e => e.ReservationId).HasColumnName("ReservationId");
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedOn");

                entity.Property(e => e.ModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("ModifiedOn");
                entity.Property(e => e.ReservationExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("ReservationExpiryDate");
                entity.Property(e => e.IdTag)
                    .HasMaxLength(20)
                    .HasColumnName("IdTag");
            });


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
