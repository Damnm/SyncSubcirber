using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Options
{
    public class EpayReportPublisherOption : PublisherOptions
    {
        public EpayReportPublisherTargetEnum PublisherTarget { get; set; }
    }
}
