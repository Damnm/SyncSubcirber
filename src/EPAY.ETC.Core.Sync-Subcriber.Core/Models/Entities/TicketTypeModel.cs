using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("TicketType")]
    public class TicketTypeModel : BaseEntity<Guid>
    {
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }

        public virtual ICollection<FeeModel> Fees { get; set; }
    }
}
