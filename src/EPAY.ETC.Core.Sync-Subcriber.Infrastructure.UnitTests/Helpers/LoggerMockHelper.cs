using Microsoft.Extensions.Logging;
using Moq;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.UnitTests.Helpers
{
    public static class LoggerMockHelper
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel expectedLogLevel,
            string expectedMessage, Func<Times> times, Exception expectedException) where T : class
        {
            loggerMock.Verify(
                m => m.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(expectedMessage)),
                    It.Is<Exception>(e => e == expectedException),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }
    }
}
