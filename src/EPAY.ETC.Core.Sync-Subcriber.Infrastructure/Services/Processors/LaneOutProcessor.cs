using AutoMapper;
using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FeeModel = EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities.FeeModel;
using FeeRequestModel = EPAY.ETC.Core.Models.Fees.FeeModel;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors
{
    public class LaneOutProcessor : ILaneProcesscor
    {
        private readonly ILogger<LaneOutProcessor> _logger;
        private readonly IMapper _mapper;
        private readonly CoreDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _stationId = string.Empty;
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

        public async Task<VehicleLaneTransactionRequestModel?> ProcessAsync(FeeRequestModel feeRequest, FeeModel feeEntity, LaneInVehicleModel laneInRequest)
        {
            try
            {
                var paymentStatus = feeEntity.Payments.FirstOrDefault()?.PaymentStatuses?.FirstOrDefault(x => x.Status == PaymentStatusEnum.Paid);
                if (paymentStatus == null)
                    return null;

                // Parking transactions
                List<TCPTransactionRequestModel>? tCPTransactions = null;
                if (feeRequest.Parking != null)
                {
                    tCPTransactions = new List<TCPTransactionRequestModel>();
                    if (feeRequest.Parking.InEpoch != 0)
                    {
                        tCPTransactions.Add(new TCPTransactionRequestModel
                        {
                            Direction = DirectionEnum.In,
                            TCPTransactionId = Guid.NewGuid().ToString(),
                            LaneId = feeRequest.Parking.LaneInId ?? "",
                            Photos = new List<string> { feeRequest.Parking.InVehiclePhotoUrl ?? "", feeRequest.Parking.InPlateNumberPhotoUrl ?? "" },
                            DateTime = feeRequest.Parking.InEpoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                        });
                    }

                    if (feeRequest.Parking.OutEpoch != 0)
                    {
                        tCPTransactions.Add(new TCPTransactionRequestModel
                        {
                            Direction = DirectionEnum.Out,
                            TCPTransactionId = Guid.NewGuid().ToString(),
                            LaneId = feeRequest.Parking.LaneOutId ?? "",
                            Photos = new List<string> { feeRequest.Parking.OutVehiclePhotoUrl ?? "", feeRequest.Parking.OutPlateNumberPhotoUrl ?? "" },
                            DateTime = feeRequest.Parking.OutEpoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                        });
                    }
                }

                // Airport transaction
                var transaction = new VehicleLaneTransactionRequestModel
                {
                    LaneInTransaction = laneInRequest == null ? null
                            : new VehicleLaneInTransactionRequestModel
                            {
                                TransactionId = $"Trans{laneInRequest.Epoch}",
                                LaneInDate = laneInRequest.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName)
                            },
                    LaneOutTransaction = new VehicleLaneOutTransactionRequestModel()
                    {
                        TransactionId = paymentStatus.TransactionId,
                        StationId = _stationId,
                        LaneId = $"{_stationId}{int.Parse(feeRequest.LaneOutVehicle.LaneOutId ?? "01"):D2}",
                        EmployeeId = feeRequest.EmployeeId,
                        LaneOutDate = feeRequest.LaneOutVehicle.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName),
                        ShiftId = "030101",
                        IsOCRSuccessful = !string.IsNullOrEmpty(feeRequest.LaneOutVehicle.VehicleInfo.PlateNumber) || !string.IsNullOrEmpty(feeRequest.LaneOutVehicle.VehicleInfo.RearPlateNumber),
                        VehicleDetails = new VehicleLaneOutDetailRequestModel
                        {
                            RFID = feeRequest.Payment.RFID,
                            VehicleTypeId = paymentStatus.Payment.CustomVehicleType == null ? null
                                    : paymentStatus.Payment.CustomVehicleType.ExternalId.Substring(paymentStatus.Payment.CustomVehicleType.ExternalId.Length - 2),
                            VehicleTypeName = paymentStatus.Payment.CustomVehicleType == null ? null : paymentStatus.Payment.CustomVehicleType.Desc,
                            FrontPlateColour = string.IsNullOrEmpty(feeRequest.LaneOutVehicle.VehicleInfo.PlateColour) ? feeRequest.LaneOutVehicle.VehicleInfo.RearPlateColour : feeRequest.LaneOutVehicle.VehicleInfo.PlateColour,
                            FrontPlateNumber = string.IsNullOrEmpty(feeRequest.LaneOutVehicle.VehicleInfo.PlateNumber) ? feeRequest.LaneOutVehicle.VehicleInfo.RearPlateNumber : feeRequest.LaneOutVehicle.VehicleInfo.PlateNumber,
                            FrontImage = string.IsNullOrEmpty(feeRequest.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl) ? feeRequest.LaneOutVehicle.VehicleInfo.VehicleRearPhotoUrl : feeRequest.LaneOutVehicle.VehicleInfo.VehiclePhotoUrl,
                            FrontPlateNumberImage = string.IsNullOrEmpty(feeRequest.LaneOutVehicle.VehicleInfo.PlateNumberPhotoUrl) ? feeRequest.LaneOutVehicle.VehicleInfo.PlateNumberRearPhotoUrl : feeRequest.LaneOutVehicle.VehicleInfo.PlateNumberPhotoUrl,
                            VehicleChargeType = feeRequest.LaneOutVehicle.VehicleChargeType
                        },
                        Payment = new VehicleLaneOutPaymentRequestModel
                        {
                            PaymentId = paymentStatus.Payment.Id,
                            TicketType = feeRequest.Payment.TicketType,
                            TicketTypeName = feeRequest.Payment.TicketTypeName,
                            PeriodTicketType = feeEntity.VehicleCategory == null ? null
                                    : (feeEntity.VehicleCategory.VehicleCategoryType == VehicleCategoryTypeEnum.Contract ? feeEntity.VehicleCategory.ExternalId : null),
                            ForceTicketType = feeEntity.VehicleCategory == null ? null
                                    : (feeEntity.VehicleCategory.VehicleCategoryType == VehicleCategoryTypeEnum.Priority ? feeEntity.VehicleCategory.ExternalId : null),
                            ChargeAmount = (int?)feeRequest.Payment.Amount,
                            DurationTime = (int)Math.Ceiling((decimal)feeRequest.Payment.Duration / 60),
                            TicketId = feeRequest.Payment.TicketId,
                            eTicket = null,
                            UseTcpParking = feeRequest.Parking != null,
                            IsNonCash = false,
                            PaymentMethod = paymentStatus.PaymentMethod,
                            IsManual = feeRequest.LaneOutVehicle.IsManual,
                        },
                        TCPTransactions = tCPTransactions,
                        ParkingCode = feeRequest.Parking == null ? null : feeRequest.Parking.LocationId
                    },
                };

                return await Task.FromResult(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(ProcessAsync)} method. Error: {ex.Message}. StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<EpayReportTransactionModel?> ProcessEpayReportAsync(FeeRequestModel feeRequest, FeeModel feeEntity)
        {
            try
            {
                var transaction = new EpayReportTransactionModel()
                {
                    Fee = _mapper.Map<EpayReportFeeModel>(feeEntity),
                    Payment = feeEntity.Payments.Any() ? _mapper.Map<EpayReportPaymentModel>(feeEntity.Payments.First()) : null,
                    ParkingLog = feeEntity.ParkingLogs.Any() ? _mapper.Map<EpayReportParkingLogModel>(feeEntity.ParkingLogs.First()) : null
                };

                if (transaction != null)
                {
                    transaction.Fee.AirportId = feeRequest.AirportId;
                    transaction.Fee.AirportName = feeRequest.AirportName;
                    transaction.Fee.LaneInPlateNumberPhotoUrl = feeRequest.LaneInVehicle?.VehicleInfo?.PlateNumberPhotoUrl;
                    transaction.Fee.LaneInVehiclePhotoUrl = feeRequest.LaneInVehicle?.VehicleInfo?.VehiclePhotoUrl;
                    transaction.Fee.LaneInPlateNumberRearPhotoUrl = feeRequest.LaneInVehicle?.VehicleInfo?.PlateNumberRearPhotoUrl;
                    transaction.Fee.LaneInVehicleRearPhotoUrl = feeRequest.LaneInVehicle?.VehicleInfo?.VehicleRearPhotoUrl;
                    transaction.Fee.VehicleChargeType = feeRequest.LaneOutVehicle?.VehicleChargeType.ToString();
                    transaction.Fee.VehicleChargeTypeName = feeRequest.LaneOutVehicle?.VehicleChargeType.ToEnumMemberAttrValue();
                }

                return await Task.FromResult(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to run {nameof(ProcessEpayReportAsync)} method. Error: {ex.Message}. StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
