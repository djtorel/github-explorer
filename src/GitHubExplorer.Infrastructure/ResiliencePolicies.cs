using System.Net;
using Microsoft.Extensions.Logging;
using Polly;

namespace GitHubExplorer.Infrastructure;

public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(2),
                onRetry: (result, timeSpan, retryCount, ctx) =>
                {
                    logger.LogWarning(
                        "Retry {RetryCount}/3 after {Delay}s due to {Reason}",
                        retryCount,
                        timeSpan.TotalSeconds,
                        result.Exception?.GetType().Name ?? $"HTTP {(int)result.Result.StatusCode}");
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30),
                (result, duration) =>
                {
                    logger.LogWarning(
                        "Circuit breaker opened for {Duration}s due to {Reason}",
                        duration.TotalSeconds,
                        result.Exception?.GetType().Name ?? $"HTTP {(int)result.Result.StatusCode}");
                },
                () => logger.LogInformation("Circuit breaker reset"));
    }
}
