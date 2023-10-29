using System.Text.Json;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Commons
{
    public class BaseProperties
    {
        public JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }
}
