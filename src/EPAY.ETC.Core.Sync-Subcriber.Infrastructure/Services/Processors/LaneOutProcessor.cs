using AutoMapper;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Extensions;
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
        private string _stationId = string.Empty;
        public LaneOutProcessor(ILogger<LaneOutProcessor> logger,
             IMapper mapper, IConfiguration configuration, CoreDbContext dbContext)
        {
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
            _stationId = _configuration["StationId"];
        }

        public bool IsSupported(string msgType)
        {
            return msgType == Constant.MsgTypeOut;
        }
        public async Task<VehicleLaneTransactionRequestModel?> ProcessAsync(FeeModel fee, LaneInVehicleModel? laneInVehicle)
        {
            if (fee.Payment == null)
                return null;

            // Parking transactions
            List<TCPTransactionRequestModel>? tCPTransactions = null;
            if (fee.Parking != null)
            {
                tCPTransactions = new List<TCPTransactionRequestModel>();
                if (fee.Parking.InEpoch != 0)
                {
                    tCPTransactions.Add(new TCPTransactionRequestModel
                    {
                        Direction = DirectionEnum.In,
                        TCPTransactionId = Guid.NewGuid().ToString(),
                        LaneId = fee.Parking.LaneInId ?? "",
                        Photos = new List<string> { fee.Parking.InVehiclePhotoUrl ?? "", fee.Parking.InPlateNumberPhotoUrl ?? "" },
                        DateTime = fee.Parking.InEpoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                    });
                }

                if (fee.Parking.OutEpoch != 0)
                {
                    tCPTransactions.Add(new TCPTransactionRequestModel
                    {
                        Direction = DirectionEnum.Out,
                        TCPTransactionId = Guid.NewGuid().ToString(),
                        LaneId = fee.Parking.LaneOutId ?? "",
                        Photos = new List<string> { fee.Parking.OutVehiclePhotoUrl ?? "", fee.Parking.OutPlateNumberPhotoUrl ?? "" },
                        DateTime = fee.Parking.OutEpoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                    });
                }
            }

            // Airport transaction
            Guid paymentId = fee.Payment.PaymentId;
            var transaction = await _dbContext.PaymentStatuses
            .Where(p => p.PaymentId == paymentId && p.Status == PaymentStatusEnum.Paid)
            .Include(p => p.Payment)
                .ThenInclude(p => p.Fee)
            .Select(p => new VehicleLaneTransactionRequestModel
            {
                LaneInTransaction = laneInVehicle == null ? null
                        : new VehicleLaneInTransactionRequestModel
                        {
                            TransactionId = $"Trans{laneInVehicle.Epoch}",
                            LaneInDate = laneInVehicle.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                        },
                LaneOutTransaction = new VehicleLaneOutTransactionRequestModel()
                {
                    TransactionId = p.TransactionId,
                    StationId = _stationId,
                    LaneId = $"{_stationId}{int.Parse(fee.LaneOutVehicle.LaneOutId ?? "01"):D2}",
                    EmployeeId = fee.EmployeeId,
                    LaneOutDate = fee.LaneOutVehicle.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName),
                    ShiftId = "030101",
                    IsOCRSuccessful = !string.IsNullOrEmpty(fee.LaneOutVehicle.VehicleInfo.PlateNumber) || !string.IsNullOrEmpty(fee.LaneOutVehicle.VehicleInfo.RearPlateNumber),
                    VehicleDetails = new VehicleLaneOutDetailRequestModel
                    {
                        RFID = fee.Payment.RFID,
                        VehicleTypeId = p.Payment.CustomVehicleType == null ? null
                                : p.Payment.CustomVehicleType.ExternalId.Substring(p.Payment.CustomVehicleType.ExternalId.Length - 2),
                        VehicleTypeName = p.Payment.CustomVehicleType == null ? null : p.Payment.CustomVehicleType.Desc,
                        FrontPlateColour = string.IsNullOrEmpty(fee.LaneOutVehicle.VehicleInfo.PlateColour) ? fee.LaneOutVehicle.VehicleInfo.RearPlateColour : fee.LaneOutVehicle.VehicleInfo.PlateColour,
                        FrontPlateNumber = string.IsNullOrEmpty(fee.LaneOutVehicle.VehicleInfo.PlateNumber) ? fee.LaneOutVehicle.VehicleInfo.RearPlateNumber : fee.LaneOutVehicle.VehicleInfo.PlateNumber,
                        FrontImage = string.IsNullOrEmpty(fee.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl) ? fee.LaneOutVehicle.VehicleInfo.VehicleRearPhotoUrl : fee.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl,
                        FrontPlateNumberImage = string.IsNullOrEmpty(fee.LaneOutVehicle.VehicleInfo.PlateNumberPhotoUrl) ? fee.LaneOutVehicle.VehicleInfo.PlateNumberRearPhotoUrl : fee.LaneOutVehicle.VehicleInfo.PlateNumberPhotoUrl,
                        VehicleChargeType = fee.LaneOutVehicle.VehicleChargeType
                    },
                    Payment = new VehicleLaneOutPaymentRequestModel
                    {
                        PaymentId = paymentId,
                        TicketType = fee.Payment.TicketType,
                        PeriodTicketType = p.Payment.Fee.VehicleCategory == null ? null
                                : (p.Payment.Fee.VehicleCategory.VehicleCategoryType == VehicleCategoryTypeEnum.Contract ? p.Payment.Fee.VehicleCategory.ExternalId : null),
                        ForceTicketType = p.Payment.Fee.VehicleCategory == null ? null
                                : (p.Payment.Fee.VehicleCategory.VehicleCategoryType == VehicleCategoryTypeEnum.Priority ? p.Payment.Fee.VehicleCategory.ExternalId : null),
                        ChargeAmount = (int?)fee.Payment.Amount,
                        DurationTime = (int)Math.Ceiling((decimal)fee.Payment.Duration / 60),
                        TicketId = fee.Payment.TicketId,
                        eTicket = null,
                        UseTcpParking = fee.Parking != null,
                        IsNonCash = false,
                        PaymentMethod = p.PaymentMethod,
                        IsManual = fee.LaneOutVehicle.IsManual,
                    },
                    TCPTransactions = tCPTransactions,
                    ParkingCode = fee.Parking == null ? null : fee.Parking.LocationId
                },
            }).FirstOrDefaultAsync();

            return transaction;
        }

        public async Task<EpayReportTransactionModel?> ProcessEpayReportAsync(FeeModel fee)
        {
            EpayReportTransactionModel? result = null;
            try
            {
                result = await _dbContext.Fees
                    .Where(f => f.Id == fee.FeeId)
                    .Include(f => f.Payments)
                        .ThenInclude(p => p.PaymentStatuses)
                    .Include(f => f.Payments)
                        .ThenInclude(p => p.ETCCheckOuts)
                    .Include(f => f.CustomVehicleType)
                    .Include(f => f.VehicleCategory)
                    .Include(f => f.TicketType)
                    .Include(f => f.ParkingLogs)
                    .Select(f => new EpayReportTransactionModel()
                    {
                        Fee = _mapper.Map<EpayReportFeeModel>(f),
                        Payment = f.Payments.Any() ? _mapper.Map<EpayReportPaymentModel>(f.Payments.First()) : null,
                        ParkingLog = f.ParkingLogs.Any() ? _mapper.Map<EpayReportParkingLogModel>(f.ParkingLogs.First()) : null
                    })
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    result.Fee.AirportId = fee.AirportId;
                    result.Fee.AirportName = fee.AirportName;
                    result.Fee.LaneInPlateNumberPhotoUrl = fee.LaneInVehicle?.VehicleInfo?.PlateNumberPhotoUrl;
                    result.Fee.LaneInVehiclePhotoUrl = fee.LaneInVehicle?.VehicleInfo?.VehiclePhotoUrl;
                    result.Fee.LaneInPlateNumberRearPhotoUrl = fee.LaneInVehicle?.VehicleInfo?.PlateNumberRearPhotoUrl;
                    result.Fee.LaneInVehicleRearPhotoUrl = fee.LaneInVehicle?.VehicleInfo?.VehicleRearPhotoUrl;
                    result.Fee.VehicleChargeType = fee.LaneOutVehicle.VehicleChargeType.ToString();
                    result.Fee.VehicleChargeTypeName = fee.LaneOutVehicle.VehicleChargeType.ToEnumMemberAttrValue();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(ProcessEpayReportAsync)} method. Error: {ex.Message}. StackTrace: {ex.StackTrace}");
                return result;
            }
        }
    }
}
