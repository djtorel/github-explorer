using System.Net;
using Microsoft.Extensions.Logging;
using Polly;

namespace GitHubExplorer.Infrastructure;

public static class ResiliencePolicies
{
    private const int RetryCount = 3;
    private const int RetryDelaySeconds = 2;
    private const int CircuitBreakerFailures = 5;
    private const int CircuitBreakerDurationSeconds = 30;

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger) =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: RetryCount,
                sleepDurationProvider: _ => TimeSpan.FromSeconds(RetryDelaySeconds),
                onRetry: (result, timeSpan, retryCount, _) =>
                    logger.LogWarning(
                        "Retry {RetryCount}/{MaxRetries} after {Delay}s due to {Reason}",
                        retryCount,
                        RetryCount,
                        timeSpan.TotalSeconds,
                        result.Exception?.GetType().Name ?? $"HTTP {(int)result.Result.StatusCode}"));

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger) =>
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .CircuitBreakerAsync(
                CircuitBreakerFailures,
                TimeSpan.FromSeconds(CircuitBreakerDurationSeconds),
                (result, duration) =>
                    logger.LogWarning(
                        "Circuit breaker opened for {Duration}s due to {Reason}",
                        duration.TotalSeconds,
                        result.Exception?.GetType().Name ?? $"HTTP {(int)result.Result.StatusCode}"),
                () => logger.LogInformation("Circuit breaker reset"));
}
