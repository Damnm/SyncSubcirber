using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.PaymentStatus;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private const string msgTypeLaneOut = "PaymentStatus", msgTypeLaneIn = "In";
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
            string direction = msgType == msgTypeLaneIn ? msgTypeLaneIn : "out";
            bool result = false;
            
            try
            {
                if (!string.IsNullOrEmpty(msgType) && (msgType == msgTypeLaneIn || msgType == msgTypeLaneOut))
                {
                    if (Guid.TryParse(message, out Guid paymentId))
                    {
                        var transaction = await _syncService.GetLaneModelDetailsAsync(paymentId, msgType == msgTypeLaneIn);

                        if (transaction != null)
                        {
                            Console.WriteLine($": {transaction}");
                            var httpContent = new StringContent(JsonConvert.SerializeObject(transaction), Encoding.UTF8, "application/json");
                            var tesst = await _httpClient.PostAsync($"{_configuration["AdminApiUrl"]}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}",
                                httpContent);

                            var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(
                                await _httpClient.PostAsync($"{_configuration["AdminApiUrl"]}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}",
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
                    }
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
