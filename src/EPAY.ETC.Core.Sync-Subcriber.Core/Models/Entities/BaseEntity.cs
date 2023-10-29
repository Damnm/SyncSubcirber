using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class BaseEntity<TId>
    {
        [Key]
        public TId? Id { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
