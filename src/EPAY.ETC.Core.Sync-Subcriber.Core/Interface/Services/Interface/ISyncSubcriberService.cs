using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface ISyncSubcriberService
    {
        Task<bool> SyncSubcriber(string? message = null, string? msgType = "");
    }
}
