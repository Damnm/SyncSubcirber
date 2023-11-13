using AutoMapper;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Publisher.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors
{
    public class PaymentCommitmentProcessor : ISyncProcessor
    {
        private IPaymentCommitmentService _paymentCommitmentService;
        private readonly ILogger<PaymentCommitmentProcessor> _logger;
        private readonly IMapper _mapper;
        public PaymentCommitmentProcessor(ILogger<PaymentCommitmentProcessor> logger,
            IPaymentCommitmentService paymentCommitmentService, IMapper mapper)
        {
            _paymentCommitmentService = paymentCommitmentService;
            _mapper = mapper;
            _logger = logger;
        }
        bool ISyncProcessor.IsSupported(string msgType)
        {
            return msgType == FeeTypeEnum.FeeCommitment.ToString();
        }

        Task<bool> ISyncProcessor.ProcessAsync(string? message)
        {
            throw new NotImplementedException();
        }
    }
}
