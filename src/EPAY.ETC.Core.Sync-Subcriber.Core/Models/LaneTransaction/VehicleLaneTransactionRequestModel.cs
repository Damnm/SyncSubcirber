namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class VehicleLaneTransactionRequestModel
    {
        public VehicleLaneInTransactionRequestModel? LaneInTransaction { get; set; }
        public VehicleLaneOutTransactionRequestModel? LaneOutTransaction { get; set; }
    }
}
