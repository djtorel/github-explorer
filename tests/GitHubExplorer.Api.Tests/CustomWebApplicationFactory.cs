using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Domain.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace GitHubExplorer.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IGitHubService GitHubServiceMock { get; } = Substitute.For<IGitHubService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IGitHubService));
            if (descriptor != null) services.Remove(descriptor);

            services.AddSingleton(GitHubServiceMock);
        });
    }
}
