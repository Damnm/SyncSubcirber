using AutoMapper;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Repositories;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Repositories
{
    public class FeeRepository : IFeeRepository
    {
        private readonly ILogger<FeeRepository> _logger;
        private readonly IMapper _mapper;
        private readonly CoreDbContext _dbContext;

        public FeeRepository(ILogger<FeeRepository> logger, IMapper mapper, CoreDbContext dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<FeeModel?> GetByIdAsync(Guid feeId)
        {
            try
            {
                _logger.LogInformation($"Executing {nameof(GetByIdAsync)} method...");

                var result = await _dbContext.Fees
                    .Where(f => f.Id == feeId)
                    .Include(f => f.Payments)
                        .ThenInclude(p => p.PaymentStatuses)
                    .Include(f => f.Payments)
                        .ThenInclude(p => p.ETCCheckOuts)
                    .Include(f => f.CustomVehicleType)
                    .Include(f => f.VehicleCategory)
                    .Include(f => f.TicketType)
                    .Include(f => f.ParkingLogs)
                    //.Select(f => new EpayReportTransactionModel()
                    //{
                    //    Fee = _mapper.Map<EpayReportFeeModel>(f),
                    //    Payment = f.Payments.Any() ? _mapper.Map<EpayReportPaymentModel>(f.Payments.First()) : null,
                    //    ParkingLog = f.ParkingLogs.Any() ? _mapper.Map<EpayReportParkingLogModel>(f.ParkingLogs.First()) : null
                    //})
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
