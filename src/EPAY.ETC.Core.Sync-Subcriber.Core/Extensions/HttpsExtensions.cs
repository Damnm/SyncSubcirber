using Newtonsoft.Json;

#nullable disable
namespace EPAY.ETC.Core.Sync_Subcriber.Core.Extensions
{
    public static class HttpsExtensions
    {
        public static async Task<T> ReturnApiResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            string? jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync() ?? null;
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
    }
}
