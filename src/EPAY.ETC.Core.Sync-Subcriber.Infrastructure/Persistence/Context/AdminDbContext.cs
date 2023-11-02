using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<LaneOutTransactionModel> LaneOutTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
            modelBuilder
             .Entity<LaneOutTransactionModel>()
             .Ignore(x => x.Name);

            modelBuilder
                .Entity<LaneOutTransactionModel>()
                .Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasConversion(new EnumToStringConverter<PaymentMethodEnum>());

            modelBuilder
                .Entity<LaneOutTransactionModel>()
                .Property(e => e.VehicleTransactionType)
                .HasMaxLength(50)
                .HasConversion(new EnumToStringConverter<VehicleChargeTypeEnum>());

            modelBuilder
                .Entity<LaneOutTransactionModel>()
                .Property(e => e.ReconciliationResult)
                .HasMaxLength(50)
                .HasConversion(new EnumToStringConverter<ReconciliationActionEnum>());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
