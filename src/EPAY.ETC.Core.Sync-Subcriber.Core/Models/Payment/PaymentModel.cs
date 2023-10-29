using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models
{
    [Table("Payment")]
    public class PaymentModel : BaseEntity<Guid>
    {
        [MaxLength(10)]
        public string? LaneInId { get; set; }

        public Guid? FeeId { get; set; }
        [ForeignKey(nameof(FeeId))]
        public virtual FeeModel? Fee { get; set; }

        [MaxLength(10)]
        public string? LaneOutId { get; set; }
        public int Duration { get; set; }
        [MaxLength(50)]
        public string? RFID { get; set; }
        [MaxLength(150)]
        public string? Make { get; set; }
        [MaxLength(150)]
        public string? Model { get; set; }
        [MaxLength(20)]
        public string? PlateNumber { get; set; }
        [MaxLength(20)]
        public string? VehicleType { get; set; }
        public Guid? CustomVehicleTypeId { get; set; }
        [ForeignKey(nameof(CustomVehicleTypeId))]

        public double Amount { get; set; }

        public virtual ICollection<PaymentStatusModel>? PaymentStatuses { get; set; }
    }
}
