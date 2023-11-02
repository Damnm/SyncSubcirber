using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities
{
    public class AdminBaseEntity<TId>
    {
        public TId? Id { get; set; }

        [MaxLength(255)]
        public string? Name { get; set; }
        [StringLength(2)]
        public string? StationId { set; get; }
    }
}
