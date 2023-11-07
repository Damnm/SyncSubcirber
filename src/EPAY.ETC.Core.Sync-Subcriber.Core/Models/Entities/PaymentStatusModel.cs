using EPAY.ETC.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [Table("PaymentStatus")]
    public class PaymentStatusModel : BaseEntity<Guid>
    {
        [ForeignKey("PaymentId")]
        public Guid PaymentId { get; set; }
        public virtual PaymentModel? Payment { get; set; }
        public double Amount { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentStatusEnum Status { get; set; }

        [MaxLength(50)]
        public string? TransactionId { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

    }
}
