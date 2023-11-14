using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor
{
    public interface ILaneOutProcesscor
    {
        Task<VehicleLaneTransactionRequestModel> ProcessAsync(Guid paymentId);
    }
}
