using System.Net.Http.Headers;
using System.Text;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils
{
    public class HttpClientUtil
    {
        public static async Task<HttpResponseMessage> PostData(HttpClient _httpClient, string url, string data)
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
                Console.WriteLine($"Failed to run {nameof(PostData)} method. Error: {ex.Message}\r\n");
                throw;
            }
        }
    }
}
