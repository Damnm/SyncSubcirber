using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services
{
    public interface IRabbitMQService
    {
        void PublishMessage<T>(T objectMessage, EpayReportPublisherTargetEnum publisherTarget, Dictionary<string, string>? arguments = null);
    }
}
