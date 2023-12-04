using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        public  readonly IEnumerable<ILaneProcesscor> _laneProcesscor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string AdminApiUrl;
        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            IEnumerable<ILaneProcesscor> laneProcesscor,
            HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _laneProcesscor = laneProcesscor ?? throw new ArgumentNullException(nameof(laneProcesscor));
            _httpClient = httpClient;
            _configuration = configuration;
            AdminApiUrl = Environment.GetEnvironmentVariable(CoreConstant.ENVIRONMENT_ADMIN_API_BASE) ?? _configuration["AdminApiUrl"];
        }

        public async Task<bool> SyncSubcriber(string message, string msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...Message: {message}. MessageType: {msgType}");
            string direction = msgType == Constrant.MsgTypeIn ? "in" : "out";
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(msgType) && (msgType == Constrant.MsgTypeOut || msgType == Constrant.MsgTypeIn))
                {
                    FeeModel? feeModel = null;
                    LaneInVehicleModel? laneInModel = null;
                    VehicleLaneTransactionRequestModel vehicleLaneTransactionRequest = null;

                    var _laneService = _laneProcesscor.FirstOrDefault(x => x.IsSupported(msgType));
                    if (_laneService == null)
                    {
                        Console.WriteLine($": Message type {msgType} is not defined");
                        return false;
                    }

                    switch (msgType)
                    {
                        case "Fees":
                            feeModel = JsonConvert.DeserializeObject<FeeModel>(message);
                            if (feeModel != null && feeModel.Payment != null)
                            {
                                laneInModel = feeModel.LaneInVehicle;
                                vehicleLaneTransactionRequest = await _laneService.ProcessAsync(feeModel, laneInModel);
                            }
                            break;
                        case "In":
                            laneInModel = JsonConvert.DeserializeObject<LaneInVehicleModel>(message);
                            if (laneInModel != null)
                            {
                                vehicleLaneTransactionRequest = await _laneService.ProcessAsync(null, laneInModel);
                            }
                            break;
                        default: break;
                    }

                    if (vehicleLaneTransactionRequest != null)
                    {
                        //var httpContent = new StringContent(JsonConvert.SerializeObject(vehicleLaneTransactionRequest), Encoding.UTF8, "application/json");
                        //Console.WriteLine($"{JsonConvert.SerializeObject(vehicleLaneTransactionRequest)}\r\n");

                        //var responseMessage = await _httpClient.PostAsync($"{AdminApiUrl}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}",
                        //    httpContent);

                        //Console.WriteLine($"Url : {AdminApiUrl}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}");

                        string url = $"{AdminApiUrl}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}";
                        var responseMessage = await PostData(url, JsonConvert.SerializeObject(vehicleLaneTransactionRequest));

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

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}\r\n");
                _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}");
                return result;
            }
        }

        private async Task<HttpResponseMessage> PostData(string url, string data)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");

                return await _httpClient.SendAsync(request, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}\r\n");
                throw;
            }
        }
    }
}
