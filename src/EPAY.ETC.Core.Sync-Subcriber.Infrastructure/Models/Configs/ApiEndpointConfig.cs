using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Configs
{
    public class ApiEndpointConfig
    {
        public string BackOfficeTransactionSync { get; set; } = string.Empty;
        public string EpayReportTransactionSync { get; set; } = string.Empty;
        public string GetImageEmbedInfo { get; set; } = string.Empty;
    }
}
