using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Options
{
    public class EpayReportPublisherOption : PublisherOptions
    {
        public EpayReportPublisherTargetEnum PublisherTarget { get; set; }
    }
}
