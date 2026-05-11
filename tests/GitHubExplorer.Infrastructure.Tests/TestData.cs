using System.Net;
using GitHubExplorer.Infrastructure;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GitHubExplorer.Infrastructure.Tests;

public sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> sendAsync) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(sendAsync(request));
}

public static class TestData
{
    public const string SampleUserJson = """
        {
            "login": "octocat",
            "name": "The Octocat",
            "avatar_url": "https://avatars.githubusercontent.com/u/1?v=4",
            "bio": "Hi, I'm the Octocat!",
            "followers": 1000,
            "public_repos": 50,
            "html_url": "https://github.com/octocat"
        }
        """;

    public const string SampleUserNullFieldsJson = """
        {
            "login": "minimal",
            "name": null,
            "avatar_url": "https://example.com/avatar.png",
            "bio": null,
            "followers": 0,
            "public_repos": 0,
            "html_url": "https://github.com/minimal"
        }
        """;

    public const string SampleReposJson = """
        [
            {
                "name": "repo1",
                "description": "First repo",
                "stargazers_count": 100,
                "forks_count": 10,
                "language": "C#",
                "html_url": "https://github.com/octocat/repo1"
            },
            {
                "name": "repo2",
                "description": null,
                "stargazers_count": 50,
                "forks_count": 5,
                "language": null,
                "html_url": "https://github.com/octocat/repo2"
            }
        ]
        """;

    public static GitHubApiClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.github.com") };
        var logger = Substitute.For<ILogger<GitHubApiClient>>();
        return new GitHubApiClient(httpClient, logger);
    }

    public static HttpResponseMessage OkJson(string json) =>
        new(HttpStatusCode.OK) { Content = new StringContent(json) };

    public static HttpResponseMessage Status(HttpStatusCode status) =>
        new(status);

    public static HttpResponseMessage RateLimited() =>
        new(HttpStatusCode.Forbidden)
        {
            Headers = { { "x-ratelimit-remaining", "0" } }
        };
}
