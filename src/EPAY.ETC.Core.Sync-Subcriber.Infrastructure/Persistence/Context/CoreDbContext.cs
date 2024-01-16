using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context
{
    [ExcludeFromCodeCoverage]
    public class CoreDbContext : DbContext
    {
        public CoreDbContext() { }
        public CoreDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public virtual DbSet<FeeModel> Fees { get; set; }
        public virtual DbSet<PaymentStatusModel> PaymentStatuses { get; set; }
        public virtual DbSet<PaymentModel> Payments { get; set; }
        public virtual DbSet<ETCCheckoutModel> ETCCheckOuts { get; set; }
        public virtual DbSet<ParkingLogModel> ParkingLogs { get; set; }
        public virtual DbSet<CustomVehicleTypeModel> CustomVehicleTypes { get; set; }
        public virtual DbSet<VehicleCategoryModel> VehicleCategories { get; set; }

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

            #region ETCCheckout configuration
            modelBuilder.Entity<ETCCheckoutModel>().HasKey(x => x.Id);
            modelBuilder.Entity<ETCCheckoutModel>()
                .HasOne(x => x.Payment)
                .WithMany(x => x.ETCCheckOuts)
                .HasForeignKey(x => x.PaymentId);
            modelBuilder.Entity<ETCCheckoutModel>().HasIndex(x => x.PaymentId);
            modelBuilder.Entity<ETCCheckoutModel>().HasIndex(x => x.TransactionId);
            modelBuilder.Entity<ETCCheckoutModel>().HasIndex("TransactionId", "RFID", "PlateNumber");
            modelBuilder.Entity<ETCCheckoutModel>()
               .Property(x => x.ServiceProvider)
               .HasMaxLength(50)
               .HasConversion(new EnumToStringConverter<ETCServiceProviderEnum>());
            modelBuilder.Entity<ETCCheckoutModel>()
               .Property(x => x.TransactionStatus)
               .HasMaxLength(50)
               .HasConversion(new EnumToStringConverter<TransactionStatusEnum>());
            #endregion

            #region Parking log configuration
            modelBuilder.Entity<ParkingLogModel>().HasKey(x => x.Id);
            modelBuilder.Entity<ParkingLogModel>()
           .Property(x => x.PaidStatus)
           .HasMaxLength(50)
           .HasConversion(new EnumToStringConverter<PaidStatusEnum>());
            modelBuilder.Entity<ParkingLogModel>()
                .HasOne(x => x.Fee)
                .WithMany(x => x.ParkingLogs)
                .HasForeignKey(x => x.FeeId);
            #endregion

            #region Ticket type
            modelBuilder.Entity<TicketTypeModel>().HasKey(x => x.Id);
            #endregion

            #region Custom vehicle type configuration
            modelBuilder.Entity<CustomVehicleTypeModel>().HasKey(x => x.Id);
            modelBuilder.Entity<CustomVehicleTypeModel>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .HasConversion(new EnumToStringConverter<CustomVehicleTypeEnum>());
            #endregion

            #region Vehicle category configuration
            modelBuilder.Entity<VehicleCategoryModel>().HasKey(x => x.Id);
            modelBuilder.Entity<VehicleCategoryModel>()
                .Property(x => x.VehicleCategoryType)
                .HasMaxLength(20)
                .HasConversion(new EnumToStringConverter<VehicleCategoryTypeEnum>());
            #endregion
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}

