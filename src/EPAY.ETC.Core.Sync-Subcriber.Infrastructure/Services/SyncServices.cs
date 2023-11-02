using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncServices : ISyncService
    {
        private readonly CoreDbContext _dbContext;

        public SyncServices(CoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<VehicleLaneTransactionRequestModel> GetDetailsAsync(Guid paymentId)
        {
            var transaction = await _dbContext.PaymentStatuses.Where(x => x.PaymentId == paymentId)
                .Include(p => p.Payment)
                .ThenInclude(x => x.Fee)
           .Select(p => new VehicleLaneTransactionRequestModel
           {
               LaneInTransaction = null,
               LaneOutTransaction = new VehicleLaneOutTransactionRequestModel()
               {
                   TransactionId = p.TransactionId,
                   StationId = "",
                   LaneId = "",
                   EmployeeId = "",
                   LaneOutDate = DateTime.Now,
                   ShiftId = p.Payment.Fee.ShiftId.ToString(),
                   IsOCRSuccessful = false,
                   VehicleDetails = new VehicleLaneOutDetailRequestModel { },
                   Payment = new VehicleLaneOutPaymentRequestModel { },
                   TCPTransactions = null, //List<TCPTransactionRequestModel>? 
                   VETCRequest = new VETCLaneOutRequestModel { },
                   VETCResponse = new VETCLaneOutResponseModel { }
               },
           }).FirstOrDefaultAsync();

            return transaction;
        }
    }
}
