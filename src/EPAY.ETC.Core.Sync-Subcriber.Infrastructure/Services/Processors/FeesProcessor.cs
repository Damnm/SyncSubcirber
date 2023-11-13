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
    public class FeesProcessor : ISyncProcessor
    {
        public IFeesCalculationService _feesLogicBuilderService;
        private readonly ILogger<FeesProcessor> _logger;
        private readonly IMapper _mapper;
        public FeesProcessor(ILogger<FeesProcessor> logger,
            IFeesCalculationService feesLogicBuilderService, IMapper mapper)
        {
            _feesLogicBuilderService = feesLogicBuilderService;
            _mapper = mapper;
            _logger = logger;
        }
        public bool IsSupported(string msgType)
        {
            return msgType == FeeTypeEnum.FeeCalculation.ToString();
        }

        public Task<bool> ProcessAsync(string? message = null)
        {
            throw new NotImplementedException();
        }
    }
}
