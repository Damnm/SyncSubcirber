using EPAY.ETC.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [Table("ETCCheckout")]
    public class ETCCheckoutModel : BaseEntity<Guid>
    {
        public Guid PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual PaymentModel? Payment { get; set; }

        [MaxLength(50)]
        public ETCServiceProviderEnum ServiceProvider { get; set; }

        [MaxLength(50)]
        public string TransactionId { get; set; }

        [MaxLength(50)]
        public TransactionStatusEnum TransactionStatus { get; set; }

        public double Amount { get; set; }

        [MaxLength(50)]
        public string? RFID { get; set; }

        [MaxLength(20)]
        public string? PlateNumber { get; set; }

        public string? TicketType { get; set; }
        public string? PlateType { get; set; }
        public string? VehicleType { get; set; }
        public int? BalanceStatus { get; set; }
    }
}
