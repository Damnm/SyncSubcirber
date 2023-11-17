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
        public async Task<VehicleLaneTransactionRequestModel> ProcessAsync(Guid? paymentId, LaneInVehicleModel laneInVehicleModel)
        {
            using (var  context = new CoreDbContext()){
                var transaction = await _dbContext.PaymentStatuses
                .Where(x => x.PaymentId == paymentId.Value)
                .Include(p => p.Payment)
                .ThenInclude(x => x.Fee)
                .Select(p => new VehicleLaneTransactionRequestModel
                {
                    LaneInTransaction = new VehicleLaneInTransactionRequestModel
                    {
                        TransactionId = p.TransactionId,
                        LaneInDate = laneInVehicleModel.Epoch.ToSpecificDateTime("SE Asia Standard Time")
                    },
                    LaneOutTransaction = new VehicleLaneOutTransactionRequestModel()
                    {
                        TransactionId = p.TransactionId,
                        StationId = stationId,
                        LaneId = p.Payment.LaneOutId ?? "0301",
                        EmployeeId = p.Payment.Fee.EmployeeId ?? "030002",
                        LaneOutDate = p.Payment.Fee.LaneOutDate ?? DateTime.Now,
                        ShiftId = "030101", // p.Payment.Fee.ShiftId ??
                        IsOCRSuccessful = false,
                        VehicleDetails = new VehicleLaneOutDetailRequestModel
                        {
                            RFID = p.Payment.RFID,
                            VehicleTypeId = p.Payment.CustomVehicleType.ExternalId,
                            FrontPlateColour = p.Payment.Fee.PlateColour,
                            FrontPlateNumber = p.Payment.Fee.PlateNumber,
                            FrontImage = p.Payment.Fee.LaneOutVehiclePhotoUrl,
                            FrontPlateNumberImage = p.Payment.Fee.LaneOutPlateNumberPhotoUrl,
                            ImageExtension = null,
                        },
                        Payment = new VehicleLaneOutPaymentRequestModel
                        {
                            TicketType = p.Payment.Fee.TicketTypeId,
                            PeriodTicketType = null,
                            ChargeAmount = (int?)p.Payment.Fee.Amount,
                            DurationTime = p.Payment.Duration,
                            IsManual = false,
                            IsUseBarcode = false,
                            TicketId = p.Payment.Fee.TicketId,
                            eTicket = null,
                            UseTcpParking = false,
                            IsNonCash = false,
                            PriorityType = null,
                            ForceTicketType = null,
                            PaymentMethod = p.PaymentMethod
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
