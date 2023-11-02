using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums
{
    public enum DirectionEnum
    {
        [EnumMember(Value = "In")]
        In,
        [EnumMember(Value = "Out")]
        Out
    }
}
