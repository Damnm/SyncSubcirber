using EPAY.ETC.Core.Models.Enums;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class EpayReportTransactionModel
    {
        public EpayReportFeeModel Fee { get; set; }
        public EpayReportPaymentModel? Payment { get; set; }
        public EpayReportParkingLogModel? ParkingLog { get; set; }
    }

    public class EpayReportFeeModel
    {
        public Guid FeeId { get; set; }
        public Guid ObjectId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AirportId { get; set; }
        public string AirportName { get; set; }
        public string? LaneInId { get; set; }
        public string? LaneInName { get; set; }
        public DateTime? LaneInDate { get; set; }
        public long? LaneInEpoch { get; set; }
        public string? LaneOutId { get; set; }
        public string? LaneOutName { get; set; }
        public DateTime? LaneOutDate { get; set; }
        public long? LaneOutEpoch { get; set; }
        public int Duration { get; set; }
        public string? RFID { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? PlateNumber { get; set; }
        public string? PlateColour { get; set; }
        public Guid? CustomVehicleTypeId { get; set; }
        public string? CustomVehicleTypeName { get; set; }
        public int? Seat { get; set; }
        public int? Weight { get; set; }
        public string? LaneInPlateNumberPhotoUrl { get; set; }
        public string? LaneInPlateNumberRearPhotoUrl { get; set; }
        public string? LaneInVehiclePhotoUrl { get; set; }
        public string? LaneInVehicleRearPhotoUrl { get; set; }
        public string? LaneOutPlateNumberPhotoUrl { get; set; }
        public string? LaneOutVehiclePhotoUrl { get; set; }
        public string? LaneOutImageEmbedInfoUrl { get; set; }
        public float? ConfidenceScore { get; set; }
        public double Amount { get; set; }
        public Guid? VehicleCategoryId { get; set; }
        public string? VehicleCategoryName { get; set; }
        public Guid? TicketTypeId { get; set; }
        public string? TicketTypeName { get; set; }
        public string? TicketId { get; set; }
        public string? TicketName { get; set; }
        public string? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public string? ExternalEmployeeId { get; set; }
        public string? ExternalEmployeeName { get; set; }
        public int BlockNumber { get; set; }
        public string? VehicleChargeType { get; set; }
        public string? VehicleChargeTypeName { get; set; }
    }

    public class EpayReportPaymentModel
    {
        public Guid PaymentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LaneInId { get; set; }
        public string? LaneOutId { get; set; }
        public int Duration { get; set; }
        public string? RFID { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? PlateNumber { get; set; }
        public Guid? CustomVehicleTypeId { get; set; }
        public double Amount { get; set; }
        public List<EpayReportETCCheckoutModel> Checkouts { get; set; }
        public List<EpayReportPaymentStatusModel> PaymentStatuses { get; set; }
    }

    public class EpayReportETCCheckoutModel
    {
        public DateTime CreatedDate { get; set; }
        public ETCServiceProviderEnum ServiceProvider { get; set; }
        public string TransactionId { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public double Amount { get; set; }
    }

    public class EpayReportPaymentStatusModel
    {
        public DateTime CreatedDate { get; set; }
        public double Amount { get; set; }
        public string? Currency { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public string? TransactionId { get; set; }
        public string? Reason { get; set; }
    }

    public class EpayReportParkingLogModel
    {
        public string LocationId { get; set; }
        public long? InEpoch { get; set; }
        public DateTime? LaneInDate { get; set; }
        public long? OutEpoch { get; set; }
        public DateTime? LaneOutDate { get; set; }
        public int DurationInSeconds { get; set; } = 0;
        public string? RFID { get; set; }
        public string? PlateNumber { get; set; }
        public string? InPlateNumberPhotoUrl { get; set; }
        public string? InVehiclePhotoUrl { get; set; }
        public string? OutPlateNumberPhotoUrl { get; set; }
        public string? OutVehiclePhotoUrl { get; set; }
        public string? LaneInId { get; set; }
        public string? LaneOutId { get; set; }
        public double? Amount { get; set; }
        public PaidStatusEnum? PaidStatus { get; set; }
    }
}
