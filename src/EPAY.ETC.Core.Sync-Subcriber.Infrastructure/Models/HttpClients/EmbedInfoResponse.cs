namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.HttpClients
{
    public class EmbedInfoResponse : ResponseBase
    {
        public ImageEmbedInfo? Data { get; set; }
    }

    public class ImageEmbedInfo
    {
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
