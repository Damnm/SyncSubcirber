using System.ComponentModel.DataAnnotations;
#nullable disable
namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class VehicleLaneInTransactionRequestModel
    {
        [Required(ErrorMessage = "TransactionId is required"), MaxLength(50)]
        public string TransactionId { set; get; }
        [MaxLength(2)]
        public string StationId { set; get; }
        public string LaneId { set; get; }
        public string ShiftId { set; get; }
        public DateTime? LaneInDate { set; get; }
        public string? TCPCheckPoint { set; get; }
        public VehicleLaneInDetailRequestModel VehicleDetails { set; get; }
        public VETCLaneInRequestModel? VETCRequest { set; get; }
        public VETCLaneInResponseModel? VETCResponse { set; get; }
    }

    public class VETCLaneInResponseModel
    {
        public string? PlateNumber { set; get; }
        public string? VehicleType { set; get; }
        public string? Status { set; get; }
        public string? MinBalanceStatus { set; get; }
        public string? TicketId { set; get; }
    }
    public class VETCLaneInRequestModel
    {
        public string? FETransactionId { set; get; }
        public string? Etag { set; get; }
        public string? Tid { set; get; }
        public string CheckinDatetime { set; get; }
        public string? TollInId { set; get; }
        public string? LaneInId { set; get; }
        public string? MinBalance { set; get; }
    }

    public class VehicleLaneInDetailRequestModel
    {
        [MaxLength(150)]
        public string? RFID { set; get; }
        [MaxLength(50)]
        public string? FrontPlateColour { set; get; }
        [MaxLength(50)]
        public string? RearPlateColour { set; get; }
        [MaxLength(15)]
        public string? FrontPlateNumber { set; get; }
        [MaxLength(15)]
        public string? RearPlateNumber { set; get; }
        public string? FrontImage { set; get; }
        public string? FrontPlateNumberImage { set; get; }
        public string? RearImage { set; get; }
        public string? RearPlateNumberImage { set; get; }
        [MaxLength(5)]
        public string? ImageExtension { set; get; }
        [MaxLength(6)]
        public string? VehicleTypeId { set; get; }
    }
}
