namespace RestApp.Helper
{
    public class RetryHelper
    {
        public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> action,
        int maxRetries = 3,
        TimeSpan? retryDelay = null,
        Type exceptionTypeToHandle = null,
        ILogger logger = null)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            retryDelay ??= TimeSpan.FromSeconds(1);
            exceptionTypeToHandle ??= typeof(Exception);

            int retryCount = 0;
            while (true)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    if (retryCount >= maxRetries)
                    {
                        logger?.LogError(ex, "Operation failed after {MaxRetries} attempts.", maxRetries);
                        throw;
                    }

                    retryCount++;
                    logger?.LogWarning(ex, "Retry attempt {RetryCount} of {MaxRetries} failed. Retrying in {Delay}.", retryCount, maxRetries, retryDelay);
                    await Task.Delay(retryDelay.Value);
                }
            }
        }
    }
}