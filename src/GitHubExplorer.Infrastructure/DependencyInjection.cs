using System.Net.Http.Headers;
using GitHubExplorer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace GitHubExplorer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGitHubInfrastructure(
        this IServiceCollection services,
        Action<GitHubApiOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IValidateOptions<GitHubApiOptions>, GitHubApiOptionsValidation>();

        services.AddHttpClient<IGitHubClient, GitHubApiClient>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<GitHubApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                client.DefaultRequestHeaders.Add("User-Agent", "GitHubExplorer");
                client.Timeout = TimeSpan.FromSeconds(10);

                if (!string.IsNullOrWhiteSpace(options.Token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Token", options.Token);
                }
            })
            .AddPolicyHandler((sp, _) => GetPolicy(sp, ResiliencePolicies.GetRetryPolicy))
            .AddPolicyHandler((sp, _) => GetPolicy(sp, ResiliencePolicies.GetCircuitBreakerPolicy));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetPolicy(
        IServiceProvider sp,
        Func<ILogger, IAsyncPolicy<HttpResponseMessage>> factory) =>
        factory(sp.GetRequiredService<ILogger<GitHubApiClient>>());
}
