using AutoMapper;
using EPAY.ETC.Core.Publisher.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors
{
    public class PaymentStatusProcessor : ISyncProcessor
    {
        private IPaymentStatusService _paymentStatusService;
        private readonly ILogger<PaymentStatusProcessor> _logger;
        private readonly IMapper _mapper;
        public PaymentStatusProcessor(ILogger<PaymentStatusProcessor> logger,
            IPaymentStatusService paymentStatusService, IMapper mapper)
        {
            _paymentStatusService = paymentStatusService;
            _mapper = mapper;
            _logger = logger;
        }

        public bool IsSupported(string msgType)
        {
            return msgType == MessageTypeEnum.PaymentStatus.ToString();
        }

        public Task<bool> ProcessAsync(string? message = null)
        {
            throw new NotImplementedException();
        }
    }
}
