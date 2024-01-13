using AutoMapper;
using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.RabbitMQ.Common.Events;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncSubcriberService : ISyncSubcriberService
    {
        private readonly ILogger<SyncSubcriberService> _logger;
        public readonly IEnumerable<ILaneProcesscor> _laneProcesscor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IMapper _mapper;
        private string AdminApiUrl;
        //private string epayReportApiHost = "http://10.10.74.67:8899/";
        private string epayReportApiHost = "https://localhost:7007/";

        public SyncSubcriberService(ILogger<SyncSubcriberService> logger,
            IEnumerable<ILaneProcesscor> laneProcesscor,
            HttpClient httpClient, IConfiguration configuration,
            IRabbitMQService rabbitMQService,
            IMapper mapper)
        {
            _logger = logger;
            _laneProcesscor = laneProcesscor ?? throw new ArgumentNullException(nameof(laneProcesscor));
            _httpClient = httpClient;
            _configuration = configuration;
            AdminApiUrl = Environment.GetEnvironmentVariable(CoreConstant.ENVIRONMENT_ADMIN_API_BASE) ?? _configuration["AdminApiUrl"];
            _rabbitMQService = rabbitMQService;
            _mapper = mapper;
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
                    {
                        result = await SyncDataToBackOffice(trans, direction, result);
                    }
                    else
                    {
                        string logMessage = $"Failed to run {nameof(SyncSubcriber)} method. Error: Transaction not found";
                        Console.WriteLine(logMessage);
                        _logger.LogError(logMessage);
                    }

                    // Publish message to Epay Report Queue
                    if (epayReportTrans != null)
                    {
                        //_rabbitMQService.PublishMessage(epayReportTrans, EpayReportPublisherTargetEnum.Transaction);
                        await SyncDataToEpayReportCenter(epayReportTrans);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                string logMessage = $"Failed to run {nameof(SyncSubcriber)} method. Error: {ex.Message}.\r\nStackTrace: {ex.StackTrace}";
                Console.WriteLine(logMessage);
                _logger.LogError(logMessage);
                return result;
            }
        }

        private async Task<bool> SyncDataToBackOffice(VehicleLaneTransactionRequestModel trans, string direction, bool result)
        {
            try
            {
                string logMessage = "";
                string url = $"{AdminApiUrl}LaneTransaction/Stations/{_configuration["StationId"]}/v1/lanes/{direction}";

                var responseMessage = await PostData(url, JsonConvert.SerializeObject(trans));
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                    if (response.Succeeded)
                    {
                        result = true;
                        logMessage = "Sync data success";
                    }
                    else
                        logMessage = $"Failed to run {nameof(SyncDataToBackOffice)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}";
                }
                else
                    logMessage = $"Failed to run {nameof(SyncDataToBackOffice)} method. Error: {responseMessage.StatusCode}";

                Console.WriteLine(logMessage);
                _logger.LogError(logMessage);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run {nameof(SyncDataToBackOffice)} method. Error: {ex.Message}\r\n");
                throw;
            }
        }

        private async Task SyncDataToEpayReportCenter(EpayReportTransactionModel trans)
        {
            try
            {
                string errMessage = "";
                string url = $"{epayReportApiHost}api/v1/transactions";

                var responseMessage = await PostData(url, JsonConvert.SerializeObject(trans));
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<HttpResponseBase>(responseMessage);
                    if (response.Succeeded)
                    {
                        string infoMessage = "Sync data to Epay Report Center successfully";
                        Console.WriteLine(infoMessage);
                        _logger.LogInformation(infoMessage);
                    }
                    else
                        errMessage = $"Failed to run {nameof(SyncDataToEpayReportCenter)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}";
                }
                else
                    errMessage = $"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: {responseMessage.StatusCode}";

                Console.WriteLine(errMessage);
                _logger.LogError(errMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run {nameof(SyncDataToEpayReportCenter)} method. Error: {ex.Message}\r\n");
            }
        }

        private async Task<HttpResponseMessage> PostData(string url, string data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(data, Encoding.UTF8, "application/json");

            Console.WriteLine($"Admin API Request: {request.RequestUri}\r\n{data}\r\n");

            return await _httpClient.SendAsync(request, CancellationToken.None);
        }
    }
}
