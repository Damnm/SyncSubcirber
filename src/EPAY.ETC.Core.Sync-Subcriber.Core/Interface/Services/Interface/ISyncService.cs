using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface ISyncService
    {
        Task<TransactionSyncModel> GetDetailsAsync(Guid paymentStatusId);
    }
}
