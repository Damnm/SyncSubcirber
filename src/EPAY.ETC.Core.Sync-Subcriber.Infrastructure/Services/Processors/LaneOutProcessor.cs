using AutoMapper;
using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
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
            return msgType == Constrant.MsgTypeOut;
        }
        public async Task<VehicleLaneTransactionRequestModel> ProcessAsync(FeeModel feeModel, LaneInVehicleModel? laneInVehicleModel)
        {
            Guid paymentId = feeModel.Payment.PaymentId;    
            using (var  context = new CoreDbContext()){

                var transaction = await _dbContext.PaymentStatuses
                .Where(x => x.PaymentId == paymentId && x.Status == ETC.Core.Models.Enums.PaymentStatusEnum.Paid)
                .Include(p => p.Payment)
                .ThenInclude(x => x.Fee)
                .Select(p => new VehicleLaneTransactionRequestModel
                {
                    LaneInTransaction = laneInVehicleModel == null ? null 
                        : new VehicleLaneInTransactionRequestModel
                        {
                            TransactionId = p.TransactionId,
                            LaneInDate = laneInVehicleModel.Epoch.ToSpecificDateTime("SE Asia Standard Time")
                        },
                    LaneOutTransaction = new VehicleLaneOutTransactionRequestModel()
                    {
                        TransactionId = p.TransactionId,
                        StationId = stationId,
                        LaneId = $"{stationId}{int.Parse(p.Payment.LaneOutId ?? "01"):D2}",
                        EmployeeId = p.Payment.Fee.EmployeeId ?? "030002",
                        LaneOutDate = p.Payment.Fee.LaneOutDate ?? DateTime.Now,
                        ShiftId = p.Payment.Fee.LaneOutDate.Value.Hour <12 ? "030101":"030102",
                        IsOCRSuccessful = false,
                        VehicleDetails = new VehicleLaneOutDetailRequestModel
                        {
                            RFID = p.Payment.RFID,
                            VehicleTypeId = p.Payment.CustomVehicleType == null ? null : p.Payment.CustomVehicleType.ExternalId,
                            FrontPlateColour = p.Payment.Fee.PlateColour,
                            FrontPlateNumber = p.Payment.Fee.PlateNumber,
                            FrontImage = p.Payment.Fee.LaneOutVehiclePhotoUrl,
                            FrontPlateNumberImage = p.Payment.Fee.LaneOutPlateNumberPhotoUrl,
                            ImageExtension = null,
                            VehicleChargeType = feeModel.LaneOutVehicle.VehicleChargeType
                        },
                        Payment = new VehicleLaneOutPaymentRequestModel
                        {
                            //TicketType = p.Payment.Fee.TicketTypeId,
                            PeriodTicketType = p.Payment.Fee.VehicleCategory == null ? null 
                                : (p.Payment.Fee.VehicleCategory.VehicleCategoryType == "Contract" ? p.Payment.Fee.VehicleCategory.ExternalId : null),
                            ChargeAmount = (int?)p.Payment.Fee.Amount,
                            DurationTime = p.Payment.Duration,
                            TicketId = p.Payment.Fee.TicketId,
                            eTicket = null,
                            UseTcpParking = false,
                            IsNonCash = false,
                            ForceTicketType = p.Payment.Fee.VehicleCategory == null ? null 
                                : (p.Payment.Fee.VehicleCategory.VehicleCategoryType == "Priority" ? p.Payment.Fee.VehicleCategory.ExternalId : null),
                            PaymentMethod = p.PaymentMethod,
                            IsManual = feeModel.LaneOutVehicle.IsManual
                        },
                        TCPTransactions = null,
                        VETCRequest = null,
                        VETCResponse = null
                    },
               }).FirstOrDefaultAsync();
                return transaction;
            }
            
        }
    }
}
