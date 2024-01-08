using AutoMapper;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors
{
    public class LaneOutProcessor : ILaneProcesscor
    {
        private readonly ILogger<LaneOutProcessor> _logger;
        private readonly IMapper _mapper;
        private readonly CoreDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private string stationId = string.Empty;
        public LaneOutProcessor(ILogger<LaneOutProcessor> logger,
             IMapper mapper, IConfiguration configuration, CoreDbContext dbContext)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
            stationId = _configuration["StationId"];
        }

        public bool IsSupported(string msgType)
        {
            return msgType == Constant.MsgTypeOut;
        }
        public async Task<VehicleLaneTransactionRequestModel?> ProcessAsync(FeeModel feeModel, LaneInVehicleModel? laneInVehicleModel)
        {
            if (feeModel.Payment == null)
                return null;

            // Parking transactions
            List<TCPTransactionRequestModel>? tCPTransactions = null;
            if (feeModel.Parking != null)
            {
                tCPTransactions = new List<TCPTransactionRequestModel>();
                if (feeModel.Parking.InEpoch != 0)
                {
                    tCPTransactions.Add(new TCPTransactionRequestModel
                    {
                        Direction = DirectionEnum.In,
                        TCPTransactionId = Guid.NewGuid().ToString(),
                        LaneId = feeModel.Parking.LaneInId ?? "",
                        Photos = new List<string> { feeModel.Parking.InVehiclePhotoUrl ?? "", feeModel.Parking.InPlateNumberPhotoUrl ?? "" },
                        DateTime = feeModel.Parking.InEpoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                    });
                }

                if (feeModel.Parking.OutEpoch != 0)
                {
                    tCPTransactions.Add(new TCPTransactionRequestModel
                    {
                        Direction = DirectionEnum.Out,
                        TCPTransactionId = Guid.NewGuid().ToString(),
                        LaneId = feeModel.Parking.LaneOutId ?? "",
                        Photos = new List<string> { feeModel.Parking.OutVehiclePhotoUrl ?? "", feeModel.Parking.OutPlateNumberPhotoUrl ?? "" },
                        DateTime = feeModel.Parking.OutEpoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                    });
                }
            }

            // Airport transaction
            Guid paymentId = feeModel.Payment.PaymentId;
            var transaction = await _dbContext.PaymentStatuses
            .Where(x => x.PaymentId == paymentId && x.Status == ETC.Core.Models.Enums.PaymentStatusEnum.Paid)
            .Include(p => p.Payment)
            .Select(p => new VehicleLaneTransactionRequestModel
            {
                LaneInTransaction = laneInVehicleModel == null ? null
                        : new VehicleLaneInTransactionRequestModel
                        {
                            TransactionId = $"TRANS{laneInVehicleModel.Epoch}",
                            LaneInDate = laneInVehicleModel.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                        },
                LaneOutTransaction = new VehicleLaneOutTransactionRequestModel()
                {
                    TransactionId = p.TransactionId,
                    StationId = stationId,
                    LaneId = $"{stationId}{int.Parse(feeModel.LaneOutVehicle.LaneOutId ?? "01"):D2}",
                    EmployeeId = feeModel.EmployeeId,
                    LaneOutDate = p.Payment.Fee.LaneOutDate ?? DateTime.Now.ConvertToTimeZone(DateTimeKind.Local, Constant.AsianTimeZoneName),
                    ShiftId = p.Payment.Fee.LaneOutDate.Value.Hour < 12 ? "030101" : "030102",
                    IsOCRSuccessful = !string.IsNullOrEmpty(feeModel.LaneOutVehicle.VehicleInfo.PlateNumber) || !string.IsNullOrEmpty(feeModel.LaneOutVehicle.VehicleInfo.RearPlateNumber),
                    VehicleDetails = new VehicleLaneOutDetailRequestModel
                    {
                        RFID = feeModel.Payment.RFID,
                        VehicleTypeId = p.Payment.CustomVehicleType == null ? null
                                : p.Payment.CustomVehicleType.ExternalId.Substring(p.Payment.CustomVehicleType.ExternalId.Length - 2),
                        FrontPlateColour = string.IsNullOrEmpty(feeModel.LaneOutVehicle.VehicleInfo.PlateColour) ? feeModel.LaneOutVehicle.VehicleInfo.RearPlateColour : feeModel.LaneOutVehicle.VehicleInfo.PlateColour,
                        FrontPlateNumber = string.IsNullOrEmpty(feeModel.LaneOutVehicle.VehicleInfo.PlateNumber) ? feeModel.LaneOutVehicle.VehicleInfo.RearPlateNumber : feeModel.LaneOutVehicle.VehicleInfo.PlateNumber,
                        FrontImage = string.IsNullOrEmpty(feeModel.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl) ? feeModel.LaneOutVehicle.VehicleInfo.VehicleRearPhotoUrl : feeModel.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl,
                        FrontPlateNumberImage = string.IsNullOrEmpty(feeModel.LaneOutVehicle.VehicleInfo.PlateNumberPhotoUrl) ? feeModel.LaneOutVehicle.VehicleInfo.PlateNumberRearPhotoUrl : feeModel.LaneOutVehicle.VehicleInfo.PlateNumberPhotoUrl,
                        VehicleChargeType = feeModel.LaneOutVehicle.VehicleChargeType
                    },
                    Payment = new VehicleLaneOutPaymentRequestModel
                    {
                        PeriodTicketType = p.Payment.Fee.VehicleCategory == null ? null
                                : (p.Payment.Fee.VehicleCategory.VehicleCategoryType == VehicleCategoryTypeEnum.Contract.ToString() ? p.Payment.Fee.VehicleCategory.ExternalId : null),
                        ChargeAmount = (int?)feeModel.Payment.Amount,
                        DurationTime = (int)Math.Ceiling((decimal)feeModel.Payment.Duration / 60),
                        TicketId = feeModel.Payment.TicketId,
                        eTicket = null,
                        UseTcpParking = feeModel.Parking != null,
                        IsNonCash = false,
                        ForceTicketType = p.Payment.Fee.VehicleCategory == null ? null
                                : (p.Payment.Fee.VehicleCategory.VehicleCategoryType == VehicleCategoryTypeEnum.Priority.ToString() ? p.Payment.Fee.VehicleCategory.ExternalId : null),
                        PaymentMethod = p.PaymentMethod,
                        IsManual = feeModel.LaneOutVehicle.IsManual,
                    },
                    TCPTransactions = tCPTransactions,
                    ParkingCode = feeModel.Parking == null ? null : feeModel.Parking.LocationId
                },
            }).FirstOrDefaultAsync();
            return transaction;
        }
    }
}
