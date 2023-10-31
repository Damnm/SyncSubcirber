using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services
{
    public class SyncServices : ISyncService
    {
        private readonly CoreDbContext _dbContext;

        public async Task<TransactionSyncModel> GetDetailsAsync(Guid paymentStatusId)
        {
            var transaction = await _dbContext.PaymentStatuses
                .Include(p => p.Payment)
                .ThenInclude(x => x.Fee)
           .Where(p => p.Id == paymentStatusId)
           .Select(p => new TransactionSyncModel
           {
               EmployeeId = p.Payment.Fee.EmployeeId,
               LaneInId = p.Payment.Fee.LaneInId,
               LaneOutId = p.Payment.Fee.LaneOutId,
               LaneInDate = p.Payment.Fee.LaneInDate,
               LaneOutDate = p.Payment.Fee.LaneOutDate,
               ShiftId = p.Payment.Fee.ShiftId,
               RFID = p.Payment.Fee.RFID,
               CustomVehicleTypeId = p.Payment.Fee.CustomVehicleTypeId,
               VehicleType = p.Payment.VehicleType,
               PlateNumber = p.Payment.Fee.PlateNumber,
               PlateColour = p.Payment.Fee.PlateColour,
               LaneInPlateNumberPhotoUrl = p.Payment.Fee.LaneInPlateNumberPhotoUrl,
               LaneInVehiclePhotoUrl = p.Payment.Fee.LaneInVehiclePhotoUrl,
               LaneOutPlateNumberPhotoUrl = p.Payment.Fee.LaneOutPlateNumberPhotoUrl,
               LaneOutVehiclePhotoUrl = p.Payment.Fee.LaneOutVehiclePhotoUrl,
               TicketId = p.Payment.Fee.TicketId,
               TicketTypeId = p.Payment.Fee.TicketTypeId,
               Amount = p.Payment.Amount,
               Duration = p.Payment.Duration,
               PaymentMethod = p.PaymentMethod.ToString(),
               PaymentDate = p.PaymentDate,
               TransactionId = p.TransactionId,
               TransactionStatus = p.Status.ToString(),
           })
            .FirstOrDefaultAsync();

            return transaction;
        }
    }
}
