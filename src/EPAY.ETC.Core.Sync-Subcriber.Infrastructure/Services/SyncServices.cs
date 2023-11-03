using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncServices : ISyncService
    {
        private readonly CoreDbContext _dbContext;

        public SyncServices(CoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<VehicleLaneTransactionRequestModel> GetLaneModelDetailsAsync(Guid paymentId, bool isLaneIn)
        {
            var transaction = await _dbContext.PaymentStatuses.Where(x => x.PaymentId == paymentId)
                .Include(p => p.Payment)
                .ThenInclude(x => x.Fee)
           .Select(p => new VehicleLaneTransactionRequestModel
           {
               LaneInTransaction = isLaneIn ? new VehicleLaneInTransactionRequestModel() { } : null,
               LaneOutTransaction = !isLaneIn ? new VehicleLaneOutTransactionRequestModel()
               {
                   TransactionId = p.TransactionId,
                   StationId = "03",
                   LaneId = p.Payment.LaneInId,
                   EmployeeId = p.Payment.Fee.EmployeeId,
                   LaneOutDate = (DateTime)p.Payment.Fee.LaneOutDate,
                   ShiftId = p.Payment.Fee.ShiftId.ToString(),
                   IsOCRSuccessful = false,
                   VehicleDetails = new VehicleLaneOutDetailRequestModel
                   {
                       RFID = p.Payment.RFID,
                       VehicleTypeId = p.Payment.CustomVehicleTypeId.ToString(),
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
                       //TransactionType = ,
                       IsManual = false,
                       IsUseBarcode = false,
                       TicketId = p.Payment.Fee.TicketId,
                       eTicket = null,
                       //UseTcpParking = null,
                       IsNonCash = false,
                       PriorityType = null,
                       ForceTicketType = null,
                       PaymentMethod = p.PaymentMethod
                   },
                   TCPTransactions = null, //List<TCPTransactionRequestModel>? 
                   VETCRequest = new VETCLaneOutRequestModel { },
                   VETCResponse = new VETCLaneOutResponseModel { }
               } : null,
           }).FirstOrDefaultAsync();

            return transaction;
        }
    }
}
