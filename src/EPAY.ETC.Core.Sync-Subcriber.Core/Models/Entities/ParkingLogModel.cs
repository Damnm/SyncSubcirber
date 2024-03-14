using EPAY.ETC.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [Table("ParkingLog")]
    public class ParkingLogModel : BaseEntity<Guid>
    {
        [MaxLength(50)]
        public string LocationId { get; set; }
        public long? InEpoch { get; set; }
        public DateTime? LaneInDate { get; set; }
        public long? OutEpoch { get; set; }
        public DateTime? LaneOutDate { get; set; }
        public int DurationInSeconds { get; set; } = 0;
        [MaxLength(50)]
        public string? RFID { get; set; }
        [MaxLength(20)]
        public string? PlateNumber { get; set; }
        [MaxLength(255)]
        public string? InPlateNumberPhotoUrl { get; set; }
        [MaxLength(255)]
        public string? InVehiclePhotoUrl { get; set; }
        [MaxLength(255)]
        public string? OutPlateNumberPhotoUrl { get; set; }
        [MaxLength(255)]
        public string? OutVehiclePhotoUrl { get; set; }
        [MaxLength(10)]
        public string? LaneInId { get; set; }
        [MaxLength(10)]
        public string? LaneOutId { get; set; }
        public double? Amount { get; set; }
        public PaidStatusEnum? PaidStatus { get; set; }

        [ForeignKey("FeeId")]
        public Guid? FeeId { get; set; }
        public virtual FeeModel? Fee { get; set; }
    }
}
