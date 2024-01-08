using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface ISyncProcessor
    {
        Task<VehicleLaneTransactionRequestModel> ProcessAsync(Guid paymentId);
    }
}
