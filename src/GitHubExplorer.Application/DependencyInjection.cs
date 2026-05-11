using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GitHubExplorer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGitHubService, GitHubService>();
        return services;
    }
}
