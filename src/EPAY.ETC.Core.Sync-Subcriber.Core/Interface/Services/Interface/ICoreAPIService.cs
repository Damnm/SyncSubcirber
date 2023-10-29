using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Request;
using EPAY.ETC.Core.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface ICoreAPIService
    {
        Task<ValidationResult<PaymentStatusModel>?> GetByIdAsync(Guid id);
    }
}
