using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models;
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

        public async Task<bool> SyncSubcriber(string? message)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...");
            try
            {
                var data = JsonSerializer.Deserialize<PaymentStatusModel>(message);
                var paymentId = data.PaymentId;
                var transaction = await _syncService.GetDetailsAsync(paymentId);
                if (transaction != null)
                {
                    if(transaction.TransactionStatus == PaymentStatusEnum.Paid)
                    {
                      Console.WriteLine($": {transaction.PaymentId}");
                      return true;
                    }   
                }
                else
                {
                    return false;
                }
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
