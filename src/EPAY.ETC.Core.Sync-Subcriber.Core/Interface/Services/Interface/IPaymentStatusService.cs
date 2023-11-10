using EPAY.ETC.Core.Models.Fees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface IPaymentStatusService
    {
        Task<bool> ProcessAsync(PaymenStatusResponseModel paymenStatusResponseModel);
    }
}
