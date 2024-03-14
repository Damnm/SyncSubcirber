using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using FeeModel = EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities.FeeModel;
using FeeRequestModel = EPAY.ETC.Core.Models.Fees.FeeModel;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Processor
{
    public interface ILaneProcesscor
    {
        bool IsSupported(string msgType);
        Task<VehicleLaneTransactionRequestModel?> ProcessAsync(FeeRequestModel feeRequest, FeeModel feeEntity, LaneInVehicleModel laneInRequest);
        Task<EpayReportTransactionModel?> ProcessEpayReportAsync(FeeRequestModel feeRequest, FeeModel feeEntity);
    }
}
