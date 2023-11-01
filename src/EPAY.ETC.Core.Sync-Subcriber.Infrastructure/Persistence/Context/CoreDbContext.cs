using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context
{
    [ExcludeFromCodeCoverage]
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public virtual DbSet<FeeModel> Fees { get; set; }
        public virtual DbSet<PaymentStatusModel> PaymentStatuses { get; set; }
        public virtual DbSet<PaymentModel> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
            #region Fee configuration
            modelBuilder.Entity<FeeModel>().HasKey(x => x.Id);       
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.CustomVehicleTypeId);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.PlateNumber);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.TicketId);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.RFID);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.LaneInId);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.LaneInDate);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.LaneInEpoch);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.LaneOutId);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.LaneOutDate);
            modelBuilder.Entity<FeeModel>().HasIndex(x => x.LaneOutEpoch);
            #endregion

            #region PaymentStatus configuration
            modelBuilder.Entity<PaymentStatusModel>().HasKey(x => x.Id);
            modelBuilder.Entity<PaymentStatusModel>()
                .HasOne(x => x.Payment)
                .WithMany(x => x.PaymentStatuses)
                .HasForeignKey(x => x.PaymentId);
            modelBuilder.Entity<PaymentStatusModel>().HasIndex(x => x.PaymentId);
            modelBuilder.Entity<PaymentStatusModel>().HasIndex(x => x.TransactionId);
            modelBuilder.Entity<PaymentStatusModel>()
               .Property(x => x.PaymentMethod)
               .HasMaxLength(50)
               .HasConversion(new EnumToStringConverter<PaymentMethodEnum>());
            modelBuilder.Entity<PaymentStatusModel>()
               .Property(x => x.Status)
               .HasMaxLength(50)
               .HasConversion(new EnumToStringConverter<PaymentStatusEnum>());
            #endregion

            #region Payment configuration
            modelBuilder.Entity<PaymentModel>().HasKey(x => x.Id);
            modelBuilder.Entity<PaymentModel>().HasIndex(x => x.FeeId);
            modelBuilder.Entity<PaymentModel>().HasIndex(x => x.LaneInId);
            modelBuilder.Entity<PaymentModel>().HasIndex(x => x.LaneOutId);
            modelBuilder.Entity<PaymentModel>().HasIndex(x => x.RFID);
            modelBuilder.Entity<PaymentModel>().HasIndex(x => x.PlateNumber);    
            modelBuilder.Entity<PaymentModel>()
                .HasOne(x => x.Fee)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.FeeId);
            #endregion

    
    }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}

