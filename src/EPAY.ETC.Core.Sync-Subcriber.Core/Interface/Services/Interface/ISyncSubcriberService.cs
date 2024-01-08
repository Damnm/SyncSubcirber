namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface
{
    public interface ISyncSubcriberService
    {
        Task<bool> SyncSubcriber(string message, string msgType);
    }
}
