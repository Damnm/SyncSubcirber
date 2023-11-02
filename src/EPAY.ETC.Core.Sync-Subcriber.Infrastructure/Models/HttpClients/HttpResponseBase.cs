namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients
{
    public class HttpResponseBase
    {
        public object? Data { get; set; }
        public bool Succeeded { get; set; } = false;
        public List<HttpError>? Errors { get; set; } = null; 
    }

    public class HttpError
    {
        public string? Message;
        public int Code;
    }
}
