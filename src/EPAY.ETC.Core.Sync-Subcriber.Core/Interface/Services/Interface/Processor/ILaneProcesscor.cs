using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor
{
    public interface ILaneProcesscor
    {
        bool IsSupported(string msgType);
        Task<VehicleLaneTransactionRequestModel?> ProcessAsync(FeeModel fee, LaneInVehicleModel? laneInVehicle);
        Task<EpayReportTransactionModel?> ProcessEpayReportAsync(FeeModel fee);
    }
}
