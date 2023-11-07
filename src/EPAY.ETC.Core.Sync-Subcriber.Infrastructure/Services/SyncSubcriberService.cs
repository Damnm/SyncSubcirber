using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

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
                            var httpContent = new StringContent(JsonConvert.SerializeObject(transaction), Encoding.UTF8, "application/json");
                            Console.WriteLine($": {JsonConvert.SerializeObject(transaction)}");

                            var responseMessage = await _httpClient.PostAsync($"{_configuration["AdminApiUrl"]}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}",
                                httpContent);

                            if (responseMessage.IsSuccessStatusCode)
                            {
                                var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
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
                            _logger.LogError($"Failed to sync data to Admin API {nameof(SyncSubcriber)} method. Error: {responseMessage.StatusCode}");
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
