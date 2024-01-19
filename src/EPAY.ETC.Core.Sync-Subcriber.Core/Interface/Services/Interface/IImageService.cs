using EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface IImageService
    {
        public Task<string?> GetUrlImageEmbedInfoUrl(HttpClient _httpClient, string apiUrl, ImageEmbedInfoRequest request);
    }
}
