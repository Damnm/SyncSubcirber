using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Repositories
{
    public interface IFeeRepository
    {
        Task<FeeModel?> GetByIdAsync(Guid feeId);
    }
}
