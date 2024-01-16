using EPAY.ETC.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [Table("CustomVehicleType")]
    public class CustomVehicleTypeModel : BaseEntity<Guid>
    {
        public CustomVehicleTypeEnum Name { get; set; }
        [MaxLength(255)]
        public string? Desc { get; set; }
        [MaxLength(6)]
        public string? ExternalId { get; set; }
        public virtual ICollection<FeeModel>? Fees { get; set; }
        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
