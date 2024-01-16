using EPAY.ETC.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [Table("VehicleCategory")]
    [ExcludeFromCodeCoverage]
    public class VehicleCategoryModel : BaseEntity<Guid>
    {
        [MaxLength(100)]
        public string? VehicleCategoryName { get; set; }
        [MaxLength(255)]
        public string? Desc { get; set; }

        [MaxLength(6)]
        public string? ExternalId { get; set; }

        [MaxLength(20)]
        public VehicleCategoryTypeEnum? VehicleCategoryType { get; set; }
    }
}

