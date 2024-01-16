using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface IRabbitMQService
    {
        void PublishMessage<T>(T objectMessage, EpayReportPublisherTargetEnum publisherTarget, Dictionary<string, string>? arguments = null);
    }
}
