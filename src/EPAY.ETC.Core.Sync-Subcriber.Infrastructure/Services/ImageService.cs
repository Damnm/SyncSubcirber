using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services;
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

        public async Task<string?> GetImageEmbedInfoUrl(HttpClient _httpClient, string apiUrl, ImageEmbedInfoRequest request)
        {
            _logger.LogInformation($"Executing {nameof(GetImageEmbedInfoUrl)} method...");
            string? result = null;
            string errMessage = string.Empty;
            try
            {
                string requestData = JsonConvert.SerializeObject(request);
                _logger.LogInformation($"{nameof(GetImageEmbedInfoUrl)} Request: {apiUrl}\r\n{requestData}\r\n");
                var responseMessage = await HttpClientUtil.PostData(_httpClient, $"{apiUrl}", requestData);

                _logger.LogInformation($"{nameof(GetImageEmbedInfoUrl)} Response: {responseMessage}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await HttpsExtensions.ReturnApiResponse<EmbedInfoResponse>(responseMessage);
                    if (response.Succeeded)
                    {
                        result = response.Data?.PhotoUrl;
                    }
                    else
                    {
                        errMessage = $"Failed to {nameof(GetImageEmbedInfoUrl)} method message: {response.Errors.FirstOrDefault().Message}, errorCode: {response.Errors.FirstOrDefault().Code}";
                    }
                }
                else
                {
                    errMessage = $"Failed to {nameof(GetImageEmbedInfoUrl)} method. Error: {responseMessage.StatusCode}";
                }

                Console.WriteLine(errMessage);
                _logger.LogError(errMessage);

                return result;
            }
            catch (Exception ex)
            {
                errMessage = $"Failed to run {nameof(GetImageEmbedInfoUrl)} method. Error: {ex.Message}";
                Console.WriteLine(errMessage);
                _logger.LogError(errMessage);
                return result;
            }
        }
    }
}
