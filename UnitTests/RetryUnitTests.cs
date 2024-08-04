using Microsoft.Extensions.Logging;
using Moq;
using RestApp.Helper;

namespace UnitTests
{
    public class RetryUnitTests
    {
        [Fact]
        public async Task ExecuteRetry_SuccessOnFirstAttempt()
        {
            var loggerMock = new Mock<ILogger>();
            var expectedResult = "Success";
            Func<Task<string>> action = async () => await Task.FromResult(expectedResult);

            var result = await RetryHelper.ExecuteWithRetryAsync(
                action,
                maxRetries: 3,
                retryDelay: TimeSpan.FromSeconds(1),
                logger: loggerMock.Object
            );

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ExecuteRetry_FailsAfterMaxRetries()
        {
            var loggerMock = new Mock<ILogger>();
            Func<Task<string>> action = async () =>
            {
                throw new InvalidOperationException("Temporary error");
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                RetryHelper.ExecuteWithRetryAsync(
                    action,
                    maxRetries: 3,
                    retryDelay: TimeSpan.FromMilliseconds(100),
                    exceptionTypeToHandle: typeof(InvalidOperationException),
                    logger: loggerMock.Object
                )
            );
        }

        [Fact]
        public async Task ExecuteRetry_RetriesOnFailure()
        {
            var loggerMock = new Mock<ILogger>();
            int attempt = 0;
            Func<Task<string>> action = async () =>
            {
                attempt++;
                if (attempt < 3)
                {
                    throw new InvalidOperationException("Temporary error");
                }
                return "Success";
            };

            var result = await RetryHelper.ExecuteWithRetryAsync(
                action,
                maxRetries: 3,
                retryDelay: TimeSpan.FromMilliseconds(100),
                exceptionTypeToHandle: typeof(InvalidOperationException),
                logger: loggerMock.Object
            );

            Assert.Equal("Success", result);
        }
    }
}