namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients
{
    public class HttpResponseBase : ResponseBase
    {
        public object? Data { get; set; }
    }

    public class ResponseBase
    {
        public bool Succeeded { get; set; } = false;
        public List<HttpError>? Errors { get; set; } = null;
    }

    public class HttpError
    {
        public string? Message { set; get; }
        public int Code { set; get; } = 0;
    }
}
