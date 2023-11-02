using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReconciliationActionEnum
    {
        [EnumMember(Value = "Tất cả")]
        All,
        [EnumMember(Value = "Hợp lệ")]
        Valid,
        [EnumMember(Value = "Không hợp lệ")]
        Invalid,
        [EnumMember(Value = "Xem xét")]
        Consider
    }
}
