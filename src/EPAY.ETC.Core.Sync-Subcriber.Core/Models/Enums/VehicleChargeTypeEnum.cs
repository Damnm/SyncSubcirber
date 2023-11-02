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
    public enum VehicleChargeTypeEnum
    {
        [EnumMember(Value = "Tất cả")]
        All = 90,
        [EnumMember(Value = "Xe có phí")]
        ChargedVehicle = 91,
        [EnumMember(Value = "Xe miễn phí")]
        FreeOfChargeVehicle = 92,
        [EnumMember(Value = "Xe tháng")]
        MonthlyChargedVehicle = 93,
        [EnumMember(Value = "Xe ưu tiên có đăng ký")]
        RegisteredPriorityVehicle = 2,
        [EnumMember(Value = "Xe ưu tiên 1 lượt")]
        OneOffPriorityVehicle = 0,
        [EnumMember(Value = "Xe ưu tiên đoàn")]
        PriorityFleetVehicle = 1,
        [EnumMember(Value = "Xe ưu tiên 1 lượt sử dụng barcode")]
        OneOffBarcodePriorityVehicle = 3,
        [EnumMember(Value = "Xe ưu tiên đoàn sử dụng barcode")]
        PriorityFleetBarcodeVehicle = 4
    }
}
