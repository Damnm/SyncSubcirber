using EPAY.ETC.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class VehicleLaneOutTransactionRequestModel
    {
        public string TransactionId { set; get; }
        public string StationId { set; get; }
        public string LaneId { set; get; }
        public string EmployeeId { set; get; }
        public DateTime LaneOutDate { set; get; }
        public string ShiftId { set; get; }
        public bool IsOCRSuccessful { set; get; }
        public string? ParkingCode { set; get; }
        public VehicleLaneOutDetailRequestModel VehicleDetails { set; get; }
        public VehicleLaneOutPaymentRequestModel Payment { set; get; }
        public List<TCPTransactionRequestModel>? TCPTransactions { get; set; }
        public VETCLaneOutRequestModel? VETCRequest { set; get; }
        public VETCLaneOutResponseModel? VETCResponse { set; get; }
    }


    public class VETCLaneOutResponseModel
    {
        public string? PlateNumber { set; get; }
        public string? VehicleType { set; get; }
        public string? Status { set; get; }
        public decimal? Amount { set; get; } = 0;
        public string? TicketId { set; get; }
        public string? TicketType { set; get; }
        public string? TicketSubType { set; get; }
        public string? TicketSubTypeDesc { set; get; }
    }
    public class VETCLaneOutRequestModel
    {
        public string? FETransactionId { set; get; }
        public string? Etag { set; get; }
        public string? Tid { set; get; }
        public DateTime? CheckinDatetime { set; get; }
        public string? TollInId { set; get; }
        public string? LaneInId { set; get; }
        public DateTime? CheckoutDatetime { set; get; }
        public string? TollOutId { set; get; }
        public string? LaneOutId { set; get; }
        public bool? UseTcp { set; get; }
    }

    public class VehicleLaneOutDetailRequestModel
    {
        [MaxLength(150)]
        public string? RFID { set; get; }
        [MaxLength(6)]
        public string? VehicleTypeId { set; get; }
        public string? VehicleTypeName { set; get; }
        [MaxLength(50)]
        public string? FrontPlateColour { set; get; }
        [MaxLength(15)]
        public string? FrontPlateNumber { set; get; }
        public string? FrontImage { set; get; }
        public string? LaneOutImageEmbedInfoURL { set; get; }
        public string? FrontPlateNumberImage { set; get; }
        [MaxLength(5)]
        public string? ImageExtension { set; get; }
        public VehicleChargeTypeEnum VehicleChargeType { set; get; }
    }

    public class VehicleLaneOutPaymentRequestModel
    {
        public string? TicketType { set; get; }
        [MaxLength(6)]
        public string? PeriodTicketType { set; get; }
        public int? ChargeAmount { set; get; }
        public int? DurationTime { set; get; }
        public int TransactionType { set; get; }
        [Required(ErrorMessage = "IsManual is required")]
        public bool IsManual { set; get; }
        public bool? IsUseBarcode { set; get; }
        [MaxLength(20)]
        public string? TicketId { set; get; }
        [MaxLength(50)]
        public string? eTicket { set; get; }
        [Required(ErrorMessage = "IsParking is required")]
        public bool UseTcpParking { set; get; }
        public bool? IsNonCash { set; get; }
        [MaxLength(6)]
        public string? PriorityType { set; get; }
        [MaxLength(6)]
        public string? ForceTicketType { set; get; }
        public PaymentMethodEnum PaymentMethod { set; get; }
        public Guid PaymentId { set; get; }
        public string? TicketTypeName { set; get; }
    }
}
