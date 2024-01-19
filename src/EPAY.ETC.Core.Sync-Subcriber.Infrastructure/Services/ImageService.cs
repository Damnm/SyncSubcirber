using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;
        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> GetUrlImageEmbedInfoUrl(HttpClient _httpClient, string apiUrl, ImageEmbedInfoRequest request)
        {
            _logger.LogInformation($"Executing {nameof(GetUrlImageEmbedInfoUrl)} method...");
            string result = string.Empty;
            try
            {
                string requestData = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"{nameof(GetUrlImageEmbedInfoUrl)} Request: {apiUrl}\r\n{requestData}\r\n");
                var responseMessage = await HttpClientUtil.PostData(_httpClient, $"{apiUrl}", requestData);

                _logger.LogInformation($"{nameof(GetUrlImageEmbedInfoUrl)} Response: {responseMessage}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<EmbedInfoResponse>(responseMessage);
                    if (response.Succeeded)
                    {
                        result = response.Data?.PhotoUrl;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error. Get embed info image failed");
                        Console.ResetColor();
                        string logMessage = $"Failed to {nameof(GetUrlImageEmbedInfoUrl)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}";
                        _logger.LogError(logMessage);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error. Get embed info image failed");
                    Console.ResetColor();
                    string logMessage = $"Failed to {nameof(GetUrlImageEmbedInfoUrl)} method. Error: {responseMessage.StatusCode}";
                    _logger.LogError(logMessage);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error. Get embed info image failed");
                _logger.LogError($"Failed to run {nameof(GetUrlImageEmbedInfoUrl)} method. Error: {ex.Message}");
                return result;
            }
        }
    }
}
