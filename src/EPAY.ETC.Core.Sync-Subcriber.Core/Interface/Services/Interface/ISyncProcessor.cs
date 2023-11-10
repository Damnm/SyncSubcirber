using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface ISyncProcessor
    {
        bool IsSupported(string msgType);
        Task<bool> ProcessAsync(string? message = null);
    }
}
