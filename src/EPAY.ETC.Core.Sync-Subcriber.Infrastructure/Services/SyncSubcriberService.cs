using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        private readonly ISyncService _syncService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string direction = "out";
        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            ISyncService syncServices,
            HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _syncService = syncServices;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> SyncSubcriber(string message, string msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...");
            bool result = false, isLaneIn = msgType == "In";
            try
            {
                var data = JsonSerializer.Deserialize<PaymentStatusModel>(message);
                var paymentId = data.PaymentId;
                var transaction = await _syncService.GetLaneModelDetailsAsync(paymentId, isLaneIn); // should be return LaneTransactionRequestModel
                if (transaction != null)
                {
                    Console.WriteLine($": {transaction}");
                    //call admin api /LaneTransaction/Stations/{stationId}/v1/lanes/{direction}
                    var httpContent = new StringContent(JsonSerializer.Serialize(transaction), Encoding.UTF8, "application/json");
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(
                        await _httpClient.PostAsync($"{_configuration["AdminApiUrl"]}/LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}", 
                        httpContent));

                    if (response.Succeeded)
                    {
                        result = true;
                        _logger.LogError("Sync data success");
                    }
                    else
                    {
                        _logger.LogError($"Failed to sync data {nameof(SyncSubcriber)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}");
                    }
                }
                else
                {
                    _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: transaction not found");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}");
                return result;
            }
        }
    }
}
