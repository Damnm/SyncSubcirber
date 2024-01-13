using AutoMapper;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.Publisher.Interface;
using EPAY.ETC.Core.RabbitMQ.Common.Events;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IPublisherService _publisher;
        private readonly IOptions<List<EpayReportPublisherOption>> _publisherOptions;
        private readonly IMapper _mapper;

        public RabbitMQService(ILogger<RabbitMQService> logger, IPublisherService publisher, IOptions<List<EpayReportPublisherOption>> publisherOptions, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher)); ;
            _publisherOptions = publisherOptions ?? throw new ArgumentNullException(nameof(publisherOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public void PublishMessage<T>(T objectMessage, EpayReportPublisherTargetEnum publisherTarget, Dictionary<string, string>? arguments = null)
        {
            _logger.LogInformation($"Executing {nameof(PublishMessage)} method...");
            try
            {
                var message = new RabbitMessageOutbound { Message = JsonSerializer.Serialize(objectMessage) };
                var pubOption = _publisherOptions.Value.FirstOrDefault(x => x.PublisherTarget == publisherTarget);
                if (pubOption != null)
                {
                    if (arguments != null)
                    {
                        foreach (var item in arguments)
                        {
                            if (pubOption.BindArguments.ContainsKey(item.Key))
                                pubOption.BindArguments[item.Key] = item.Value;
                        }
                    }
                    _publisher.SendMessage(message, _mapper.Map<PublisherOptions>(pubOption));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(PublishMessage)} method. Error: {ex.Message}");
            }
        }
    }
}
