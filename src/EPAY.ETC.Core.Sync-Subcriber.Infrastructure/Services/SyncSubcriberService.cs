using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.UI;
using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.RabbitMQ.Common.Events;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        private readonly ISyncService _syncService;
        public SyncSubcriberService(ILogger<SyncSubcriberService> logger, ISyncService syncServices)
        {
            _logger = logger;
            _syncService = syncServices;
        }

        public async Task<bool> SyncSubcriber(string? message, string? msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...");
            try
            {
                var details = await _syncService.GetDetailsAsync(Id);

                var transactionModel = new TransactionSyncModel
                {
                };

                _adminDbContext.TransactionSyncModels.Add(transactionModel);
                await _adminDbContext.SaveChangesAsync();

                _logger.LogInformation($"Start received message and executing: {message}");
                return true;
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}");
                return false; 
            }
        }
    }
}
