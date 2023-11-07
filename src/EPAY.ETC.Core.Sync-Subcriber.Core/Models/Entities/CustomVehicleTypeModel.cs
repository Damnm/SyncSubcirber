using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [Table("CustomVehicleType")]
    public class CustomVehicleTypeModel : BaseEntity<Guid>
    {
        [MaxLength(6)]
        public string ExternalId { get; set; }
    }
}
