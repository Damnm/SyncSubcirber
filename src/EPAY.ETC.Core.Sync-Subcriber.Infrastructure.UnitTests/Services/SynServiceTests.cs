using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.UnitTests.Services
{
    public class SynServiceTests
    {
        #region Variables
        private Mock<CoreDbContext> _dbContextMock = new Mock<CoreDbContext>();
        private Mock<ILogger<SyncServices>> _loggerMock = new Mock<ILogger<SyncServices>>();
        private Mock<Microsoft.Extensions.Configuration.IConfiguration> _iconfigurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        private Mock<DbSet<PaymentStatusModel>> _dbPaymentStatusModelSetMock = new Mock<DbSet<PaymentStatusModel>>();

        private List<PaymentStatusModel> statusModels = new List<PaymentStatusModel>()
        {
            new PaymentStatusModel()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Status = PaymentStatusEnum.Paid,
                PaymentId =Guid.Parse( "f3353ae8-2fba-4446-9a5e-0ef607cc7469"),
                Amount = 10000,
                Currency = "VND",
                PaymentDate =  DateTime.Now,
                PaymentMethod = PaymentMethodEnum.RFID,
                TransactionId = "TransactionId",
                Reason = "Reason",
            },

        };
        private bool isLaneIn = false;
        private Guid paymentId = Guid.Parse("f3353ae8-2fba-4446-9a5e-0ef607cc7469");
        #endregion

        [Fact]
        public async Task GivenRequestPaidIsValidPaymentId_WhenSynServiceisCalles_ThenSuccess()
        {
            // Arrange
            _dbPaymentStatusModelSetMock = EFTestHelper.GetMockDbSet(statusModels);
            _dbContextMock.Setup(x => x.PaymentStatuses).Returns(_dbPaymentStatusModelSetMock.Object);
            //Act
            SyncServices syncServices = new SyncServices(_dbContextMock.Object, _iconfigurationMock.Object);
            var result = await syncServices.GetLaneModelDetailsAsync(paymentId, isLaneIn);
            //_iconfigurationMock.SetupGet(x => x[It.Is<string>(s => s == "Key")]).Returns("test123");
            // Assert          
            Assert.NotNull(result);
            Assert.Null(result.LaneInTransaction);
            Assert.NotNull(result.LaneOutTransaction);           
        }
    }
}