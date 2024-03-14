using System.Runtime.Serialization;

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
