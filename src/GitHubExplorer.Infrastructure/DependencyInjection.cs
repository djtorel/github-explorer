using System.Net.Http.Headers;
using GitHubExplorer.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GitHubExplorer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGitHubInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GitHubApiOptions>(
            configuration.GetSection(GitHubApiOptions.SectionName));

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
            .AddPolicyHandler((sp, _) =>
            {
                var logger = sp.GetRequiredService<ILogger<GitHubApiClient>>();
                return ResiliencePolicies.GetRetryPolicy(logger);
            })
            .AddPolicyHandler((sp, _) =>
            {
                var logger = sp.GetRequiredService<ILogger<GitHubApiClient>>();
                return ResiliencePolicies.GetCircuitBreakerPolicy(logger);
            });

        return services;
    }
}
