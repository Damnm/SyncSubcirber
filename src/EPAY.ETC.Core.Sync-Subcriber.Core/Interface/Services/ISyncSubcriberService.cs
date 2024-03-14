namespace EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services
{
    public interface ISyncSubcriberService
    {
        Task<bool> SyncSubcriber(string message, string msgType);
    }
}
