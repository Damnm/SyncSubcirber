using AutoMapper;
using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Repositories;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Configs;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IFeeRepository _feeRepository;
        private readonly IOptions<ApiEndpointConfig> _apiEndpointConfig;
        private readonly string _stationId = string.Empty;

        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            IEnumerable<ILaneProcesscor> laneProcesscor,
            HttpClient httpClient, IConfiguration configuration, IImageService imageService, IMapper mapper,
            IFeeRepository feeRepository, IOptions<ApiEndpointConfig> apiEndpointConfig)
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
            _feeRepository = feeRepository;
            _apiEndpointConfig = apiEndpointConfig;
            _stationId = _configuration["StationId"];
        }

        public async Task<bool> SyncSubcriber(string message, string msgType)
        {
            _logger.LogInformation($"Executing {nameof(SyncSubcriber)} method...Message: {message}. MessageType: {msgType}");
            string direction = msgType == Constant.MsgTypeIn ? "in" : "out";
            bool result = false;
            try
            {
                var _laneService = _laneProcesscor.FirstOrDefault(x => x.IsSupported(msgType));
                if (_laneService == null)
                {
                    Console.WriteLine($"Message type {msgType} is not defined");
                    _logger.LogError($"Message type {msgType} is not defined");
                    return false;
                }

                FeeModel? feeRequest = null;
                LaneInVehicleModel? laneInRequest = null;
                VehicleLaneTransactionRequestModel? trans = null;
                EpayReportTransactionModel? epayReportTrans = null;

                switch (msgType)
                {
                    case "Fees":
                        feeRequest = JsonConvert.DeserializeObject<FeeModel>(message);
                        if (feeRequest != null && feeRequest.FeeId != null && feeRequest.Payment != null)
                        {
                            // Prepare datas
                            var feeEntity = await _feeRepository.GetByIdAsync(feeRequest.FeeId.Value);
                            if (feeEntity != null)
                            {
                                laneInRequest = feeRequest.LaneInVehicle;
                                Task<VehicleLaneTransactionRequestModel?> transT1 = _laneService.ProcessAsync(feeRequest, feeEntity, laneInRequest);
                                Task<EpayReportTransactionModel?> transT2 = _laneService.ProcessEpayReportAsync(feeRequest, feeEntity);

                                await Task.WhenAll(transT1, transT2);
                                trans = transT1.Result;
                                epayReportTrans = transT2.Result;

                                //Get embed info image url
                                string? imageEmbedInfoUrl = null;
                                if (trans != null && trans.LaneOutTransaction != null && trans.LaneOutTransaction.VehicleDetails != null)
                                {
                                    string url = $"{_imageApiUrl}{_apiEndpointConfig.Value.GetImageEmbedInfo}";
                                    imageEmbedInfoUrl = await _imageService.GetImageEmbedInfoUrl(_httpClient, url, new ImageEmbedInfoRequest
                                    {
                                        ReferenceId = feeRequest.Payment.PaymentId,
                                        AirportId = feeRequest.AirportId,
                                        TerminalId = feeRequest.TerminalId,
                                        LaneOutId = feeRequest.LaneOutVehicle.LaneOutId,
                                        LaneOutDateTime = trans.LaneOutTransaction.LaneOutDate,
                                        VehicleType = trans.LaneOutTransaction.VehicleDetails.VehicleTypeName,
                                        PlateNumber = trans.LaneOutTransaction.VehicleDetails.FrontPlateNumber,
                                        TicketType = trans.LaneOutTransaction.Payment?.TicketTypeName,
                                        Amount = (decimal)trans.LaneOutTransaction.Payment?.ChargeAmount,
                                        RFID = trans.LaneOutTransaction.VehicleDetails.RFID,
                                        ImageId = trans.LaneOutTransaction.VehicleDetails.FrontImage
                                    });

                                    trans.LaneOutTransaction.VehicleDetails.LaneOutImageEmbedInfoURL = imageEmbedInfoUrl;

                                    if (epayReportTrans != null)
                                        epayReportTrans.Fee.LaneOutImageEmbedInfoUrl = imageEmbedInfoUrl;
                                }
                            }
                        }
                        break;
                    case "In":
                        laneInRequest = JsonConvert.DeserializeObject<LaneInVehicleModel>(message);
                        if (laneInRequest != null)
                        {
                            trans = await _laneService.ProcessAsync(null, null, laneInRequest);
                        }
                        break;
                    default: break;
                }

                // Sync data to BackOffice
                Task<bool> syncT1 = SyncDataToBackOffice(trans, direction);

                // Sync data to Epay Report Center
                Task<bool> syncT2 = SyncDataToEpayReportCenter(epayReportTrans);

                await Task.WhenAll(syncT1, syncT2);
                result = syncT1.Result;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}.\r\nStackTrace: {ex.StackTrace}");
                return result;
            }
        }

        private async Task<bool> SyncDataToBackOffice(VehicleLaneTransactionRequestModel? trans, string direction)
        {
            bool result = false;
            try
            {
                if (trans == null)
                {
                    _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method. Error: Transaction not found");
                    return result;
                }

                string url = $"{_adminApiUrl}{string.Format(_apiEndpointConfig.Value.BackOfficeTransactionSync, _stationId, direction)}";
                string data = JsonConvert.SerializeObject(trans);
                Console.WriteLine($"{nameof(SyncDataToBackOffice)} Request: {url}\r\n{data}");
                _logger.LogInformation($"{nameof(SyncDataToBackOffice)} Request: {data}");

                var responseMessage = await HttpClientUtil.PostData(_httpClient, url, data);
                _logger.LogInformation($"{nameof(SyncDataToBackOffice)} Response: {responseMessage}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                    if (response.Succeeded)
                    {
                        result = true;
                        Console.WriteLine("Sync data to BackOffice successfully\r\n");
                        _logger.LogInformation("Sync data to BackOffice successfully");
                    }
                    else
                    {
                        _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}");
                    }
                }
                else
                    _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method. Error: {responseMessage.StatusCode}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncDataToBackOffice)} method. Error: {ex.Message}\r\n");
                return result;
            }
        }

        private async Task<bool> SyncDataToEpayReportCenter(EpayReportTransactionModel? trans)
        {
            bool result = false;
            try
            {
                if (trans == null)
                {
                    _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: Transaction not found");
                    return result;
                }

                string url = $"{_epayReportApiUrl}{_apiEndpointConfig.Value.EpayReportTransactionSync}";
                string data = JsonConvert.SerializeObject(trans);
                Console.WriteLine($"{nameof(SyncDataToEpayReportCenter)} Request: {url}\r\n{data}");
                _logger.LogInformation($"{nameof(SyncDataToEpayReportCenter)} Request: {data}");

                var responseMessage = await HttpClientUtil.PostData(_httpClient, url, data);
                _logger.LogInformation($"{nameof(SyncDataToEpayReportCenter)} Response: {responseMessage}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                    if (response.Succeeded)
                    {
                        result = true;
                        string infoMessage = "Sync data to Epay Report successfully\r\n";
                        Console.WriteLine(infoMessage);
                        _logger.LogInformation(infoMessage);
                    }
                    else
                        _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}");
                }
                else
                    _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: {responseMessage.StatusCode}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: {ex.Message}\r\n. Stack trace: {ex.StackTrace}");
                return result;
            }
        }
    }
}
