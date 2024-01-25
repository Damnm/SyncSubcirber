using EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services
{
    public interface IImageService
    {
        public Task<string?> GetImageEmbedInfoUrl(HttpClient _httpClient, string apiUrl, ImageEmbedInfoRequest request);
    }
}
