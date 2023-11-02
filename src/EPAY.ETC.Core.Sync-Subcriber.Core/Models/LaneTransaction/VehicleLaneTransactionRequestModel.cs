using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class VehicleLaneTransactionRequestModel
    {
        public VehicleLaneInTransactionRequestModel? LaneInTransaction { get; set; }
        public VehicleLaneOutTransactionRequestModel? LaneOutTransaction { get; set; }
        [Required, NotMapped, Range(1, 1, ErrorMessage = "LaneInTransaction or LaneOutTransaction must be required")]
        public int RequiredVehicleLaneTransactionRequest
        {
            get
            {
                return LaneInTransaction != null || LaneOutTransaction != null ? 1 : 0;
            }
        }
    }
}
