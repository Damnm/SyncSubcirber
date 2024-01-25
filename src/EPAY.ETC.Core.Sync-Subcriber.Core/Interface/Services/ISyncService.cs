using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services
{
    public interface ISyncService
    {
        Task<VehicleLaneTransactionRequestModel> GetLaneModelDetailsAsync(Guid paymentId, bool isLaneIn);
        Task<bool> ProcessAsync(string? message = null, string? msgType = "");
    }
}
