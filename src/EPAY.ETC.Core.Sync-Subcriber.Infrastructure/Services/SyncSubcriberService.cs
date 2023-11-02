using EPAY.ETC.Core.Models.Devices;
using EPAY.ETC.Core.Models.Enums;

using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.RabbitMQ.Common.Events;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
