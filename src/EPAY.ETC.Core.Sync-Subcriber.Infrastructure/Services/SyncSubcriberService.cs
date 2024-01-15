using AutoMapper;
using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
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
        private string _adminApiUrl, _imageApiUrl, _epayReportApiUrl;
        private readonly IMapper _mapper;

        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            IEnumerable<ILaneProcesscor> laneProcesscor,
            HttpClient httpClient, IConfiguration configuration, IImageService imageService, IMapper mapper)
        {
            _logger = logger;
            _laneProcesscor = laneProcesscor ?? throw new ArgumentNullException(nameof(laneProcesscor));
            _httpClient = httpClient;
            _configuration = configuration;
            _adminApiUrl = Environment.GetEnvironmentVariable(CoreConstant.ENVIRONMENT_ADMIN_API_BASE) ?? _configuration["AdminApiUrl"];
            _imageApiUrl = Environment.GetEnvironmentVariable("AI_IMAGE_API_BASE_ENVIRONMENT") ?? _configuration["EmbedInfoImageApiUrl"];
            _epayReportApiUrl = Environment.GetEnvironmentVariable("EPAY_REPORT_API_BASE_ENVIRONMENT") ?? _configuration["EpayReportApiUrl"];
            _mapper = mapper;
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
                    FeeModel? fee = null;
                    LaneInVehicleModel? laneIn = null;
                    VehicleLaneTransactionRequestModel? trans = null;
                    EpayReportTransactionModel? epayReportTrans = null;

                    var _laneService = _laneProcesscor.FirstOrDefault(x => x.IsSupported(msgType));
                    if (_laneService == null)
                    {
                        Console.WriteLine($"Message type {msgType} is not defined");
                        _logger.LogError($"Message type {msgType} is not defined");
                        return false;
                    }

                    switch (msgType)
                    {
                        case "Fees":
                            fee = JsonConvert.DeserializeObject<FeeModel>(message);
                            if (fee != null && fee.Payment != null)
                            {
                                laneIn = fee.LaneInVehicle;
                                trans = await _laneService.ProcessAsync(fee, laneIn);
                                epayReportTrans = await _laneService.ProcessEpayReportAsync(fee);
                                //Get embed info image url
                                //string url = $"{_imageApiUrl}Media/v1/embed-info";
                                //vehicleLaneTransactionRequest.LaneOutTransaction.VehicleDetails.LaneOutImageEmbedInfoURL
                                //    = await _imageService.GetUrlImageEmbedInfoUrl(_httpClient, url, new ImageEmbedInfoRequest
                                //    {
                                //        ReferenceId = feeModel.Payment.PaymentId,
                                //        AirportId = "",
                                //        TerminalId = "",
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
                            laneIn = JsonConvert.DeserializeObject<LaneInVehicleModel>(message);
                            if (laneIn != null)
                            {
                                trans = await _laneService.ProcessAsync(null, laneIn);
                            }
                            break;
                        default: break;
                    }

                    // Sync data to BackOffice
                    if (trans != null)
                        result = await SyncDataToBackOffice(trans, direction, result);
                    else
                        _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: Transaction not found");

                    // Sync data to Epay Report Queue
                    if (epayReportTrans != null)
                        await SyncDataToEpayReportCenter(epayReportTrans);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}.\r\nStackTrace: {ex.StackTrace}");
                return result;
            }
        }

        private async Task<bool> SyncDataToBackOffice(VehicleLaneTransactionRequestModel trans, string direction, bool result)
        {
            try
            {
                string url = $"{_adminApiUrl}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}";
                var responseMessage = await HttpClientUtil.PostData(_httpClient, url, JsonConvert.SerializeObject(trans));
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                    if (response.Succeeded)
                    {
                        result = true;
                        Console.WriteLine("Sync data to Back Office successfully");
                    }
                    else
                        _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}");
                }
                else
                    _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method. Error: {responseMessage.StatusCode}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method. Error: {ex.Message}\r\n");
                throw;
            }
        }

        private async Task SyncDataToEpayReportCenter(EpayReportTransactionModel trans)
        {
            try
            {
                string url = $"{_epayReportApiUrl}v1/transactions";
                var responseMessage = await HttpClientUtil.PostData(_httpClient, url, JsonConvert.SerializeObject(trans));
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                    if (response.Succeeded)
                    {
                        string infoMessage = "Sync data to Epay Report successfully";
                        Console.WriteLine(infoMessage);
                        _logger.LogInformation(infoMessage);
                    }
                    else
                        _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}");
                }
                else
                    _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: {responseMessage.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: {ex.Message}\r\n. Stack trace: {ex.StackTrace}");
            }
        }
    }
}
