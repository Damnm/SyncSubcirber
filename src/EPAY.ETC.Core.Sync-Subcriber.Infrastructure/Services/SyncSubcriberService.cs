using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.UI;
using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.RabbitMQ.Common.Events;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        private readonly ICoreAPIService _coreAPIService;
        private readonly ISyncService _syncService;
        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,ICoreAPIService coreAPIService, ISyncService syncServices)
        {
            _logger = logger;
            _coreAPIService = coreAPIService;
            _syncService = syncServices;
        }

        public async Task<bool> SyncSubcriber(string? message, string? msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...");
            try
            {
                _logger.LogInformation($"Start received message and executing: {message}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}");
                return false;
            }
            
        }
    }
}
