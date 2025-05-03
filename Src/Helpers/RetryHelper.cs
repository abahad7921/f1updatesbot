using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1UpdatesBot.Src.Helpers
{
    public static class RetryHelper
    {
        public static async Task<T> ExecuteWithRetryAsync<T>(
            Func<Task<T>> operation,
            int maxRetries = 3,
            int delayMilliseconds = 1000,
            Func<Exception, bool>? shouldRetryOnException = null)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (attempt < maxRetries && (shouldRetryOnException?.Invoke(ex) ?? true))
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delayMilliseconds}ms...");
                    await Task.Delay(delayMilliseconds);
                    delayMilliseconds *= 2; // Exponential backoff
                }
            }

            // If we exhausted all retries, run one last time and let any exception bubble up
            return await operation();
        }
    }
}
