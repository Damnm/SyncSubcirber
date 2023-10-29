using EPAY.ETC.Core.RabbitMQ.Common.Enums;
using EPAY.ETC.Core.Subscriber.Common.Options;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Configs
{ 
    public class SubscriberOptionModel
    {
        #region Common options
        public bool CreateExchangeQueue { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public ExchangeOrQueueEnum ExchangeOrQueue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AutoAck { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public bool RequeueNack { get; set; } = false;
        /// <summary>
        /// if set to true then exchange or queue will be deleted and created again if PRECONDITION fail
        /// </summary>
        public bool RecreateIfFailed { get; set; } = false;
        /// <summary>
        /// If yes, clients cannot publish to this exchange directly. It can only be used with exchange to exchange bindings.
        /// </summary>
        public bool Internal { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public uint PrefetchSize { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public ushort PrefetchCount { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? MessageHandlingPeriod { get; set; } = TimeSpan.FromMilliseconds(1000);
        #endregion

        #region Exchange options
        public ExchangeOption? ExchangeOption { get; set; } = new ExchangeOption();
        #endregion

        #region Queue options
        public List<QueueOption> QueueOptions { get; set; } = new List<QueueOption>();
        #endregion
    }
}
