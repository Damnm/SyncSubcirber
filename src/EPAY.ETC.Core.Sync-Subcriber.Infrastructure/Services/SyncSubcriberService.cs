using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        public readonly IEnumerable<ILaneProcesscor> _laneProcesscor;
        public readonly IImageService _imageService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _adminApiUrl, _imageApiUrl;
        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            IEnumerable<ILaneProcesscor> laneProcesscor,
            HttpClient httpClient, IConfiguration configuration, IImageService imageService)
        {
            _logger = logger;
            _laneProcesscor = laneProcesscor ?? throw new ArgumentNullException(nameof(laneProcesscor));
            _httpClient = httpClient;
            _configuration = configuration;
            _adminApiUrl = Environment.GetEnvironmentVariable(CoreConstant.ENVIRONMENT_ADMIN_API_BASE) ?? _configuration["AdminApiUrl"];
            _imageApiUrl = Environment.GetEnvironmentVariable("AI_IMAGE_API_BASE_ENVIRONMENT") ?? _configuration["EmbedInfoImageApiUrl"];
            _imageService = imageService;
        }

        public async Task<bool> SyncSubcriber(string message, string msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...Message: {message}. MessageType: {msgType}");
            string direction = msgType == Constant.MsgTypeIn ? "in" : "out";
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(msgType) && (msgType == Constant.MsgTypeOut || msgType == Constant.MsgTypeIn))
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
                                //Get embed info image url
                                //string url = $"{_imageApiUrl}Media/v1/embed-info";
                                //vehicleLaneTransactionRequest.LaneOutTransaction.VehicleDetails.LaneOutImageEmbedInfoURL
                                //    = await _imageService.GetUrlImageEmbedInfoUrl(_httpClient, url, new ImageEmbedInfoRequest
                                //    {
                                //        ReferenceId = feeModel.Payment.PaymentId,
                                //        AirportId = feeModel.AirportId,
                                //        TerminalId = feeModel.TerminalId,
                                //        LaneOutDateTime = vehicleLaneTransactionRequest.LaneOutTransaction.LaneOutDate,
                                //        VehicleType = vehicleLaneTransactionRequest.LaneOutTransaction.VehicleDetails.VehicleTypeId,
                                //        PlateNumber = vehicleLaneTransactionRequest.LaneOutTransaction.VehicleDetails.FrontPlateNumber,
                                //        TicketType = vehicleLaneTransactionRequest.LaneOutTransaction.Payment.TicketType,
                                //        Amount = (decimal)vehicleLaneTransactionRequest.LaneOutTransaction.Payment.ChargeAmount,
                                //        RFID = vehicleLaneTransactionRequest.LaneOutTransaction.VehicleDetails.RFID,
                                //        ImageId = feeModel.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl,
                                //    });
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
                        string url = $"{_adminApiUrl}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}";
                        var responseMessage = await HttpClientUtil.PostData(_httpClient, url, JsonConvert.SerializeObject(vehicleLaneTransactionRequest));
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                            if (response.Succeeded)
                            {
                                result = true;
                                string logMessage = "Sync data success";
                                _logger.LogInformation(logMessage);
                            }
                            else
                            {
                                string logMessage = $"Failed to sync data {nameof(SyncSubcriber)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}";
                                _logger.LogError(logMessage);
                            }
                        }
                        else
                        {
                            string logMessage = $"Failed to sync data to Admin API {nameof(SyncSubcriber)} method. Error: {responseMessage.StatusCode}";
                            _logger.LogError(logMessage);
                        }
                    }
                    else
                    {
                        string logMessage = $"Failed to run {nameof(SyncSubcriber)} method. Error: transaction not found";
                        _logger.LogError(logMessage);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                string logMessage = $"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}";
                _logger.LogError(logMessage);
                return result;
            }
        }
    }
}
