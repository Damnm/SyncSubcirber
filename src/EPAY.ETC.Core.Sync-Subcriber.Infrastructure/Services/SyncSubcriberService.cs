using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        private readonly IEnumerable<ILaneProcesscor> _laneProcesscor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            IEnumerable<ILaneProcesscor> laneProcesscor,
            HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _laneProcesscor = laneProcesscor;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> SyncSubcriber(string message, string msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...");
            string direction = msgType == Constrant.MsgTypeIn ? "in" : "out";
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(msgType) && (msgType == Constrant.MsgTypeOut || msgType == Constrant.MsgTypeIn))
                {
                    FeeModel feeModel = null;
                    LaneInVehicleModel laneInModel = null;
                    VehicleLaneTransactionRequestModel vehicleLaneTransactionRequest = null;

                    var _laneService = _laneProcesscor.FirstOrDefault(x => x.IsSupported(msgType ?? string.Empty));
                    if (_laneService == null)
                    {
                        Console.WriteLine($": Message type {msgType} is not defined");
                        return false;
                    }

                    if (msgType == Constrant.MsgTypeOut) feeModel = JsonConvert.DeserializeObject<FeeModel>(message);
                    else laneInModel = JsonConvert.DeserializeObject<LaneInVehicleModel>(message);

                    if (msgType == Constrant.MsgTypeOut && feeModel != null && feeModel.Payment != null)
                    {
                        vehicleLaneTransactionRequest = await _laneService.ProcessAsync(feeModel.Payment.PaymentId, null);
                    }
                    else if(msgType == Constrant.MsgTypeIn && laneInModel != null)
                    {
                        vehicleLaneTransactionRequest = await _laneService.ProcessAsync(null, laneInModel);
                    }


                    if (vehicleLaneTransactionRequest != null)
                    {
                        var httpContent = new StringContent(JsonConvert.SerializeObject(vehicleLaneTransactionRequest), Encoding.UTF8, "application/json");
                        Console.WriteLine($": {JsonConvert.SerializeObject(vehicleLaneTransactionRequest)}");

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
