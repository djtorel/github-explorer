using System.Net;
using GitHubExplorer.Domain.Results;
using Shouldly;

namespace GitHubExplorer.Infrastructure.Tests;

public class GitHubApiClientTests
{
    [Fact]
    public async Task GetUserAsync_ReturnsSuccess_WhenUserExists()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.OkJson(TestData.SampleUserJson));
        var client = TestData.CreateClient(handler);

        var result = await client.GetUserAsync("octocat");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Login.ShouldBe("octocat");
        result.Value.Name.ShouldBe("The Octocat");
        result.Value.AvatarUrl.ShouldBe("https://avatars.githubusercontent.com/u/1?v=4");
        result.Value.Bio.ShouldBe("Hi, I'm the Octocat!");
        result.Value.Followers.ShouldBe(1000);
        result.Value.PublicRepos.ShouldBe(50);
        result.Value.HtmlUrl.ShouldBe("https://github.com/octocat");
    }

    [Fact]
    public async Task GetUserAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.Status(HttpStatusCode.NotFound));
        var client = TestData.CreateClient(handler);

        var result = await client.GetUserAsync("not-a-real-user-12345");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsRateLimited_WhenRateLimitExceeded()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.RateLimited());
        var client = TestData.CreateClient(handler);

        var result = await client.GetUserAsync("octocat");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.RateLimited);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsNetworkError_WhenRequestFails()
    {
        var handler = new FakeHttpMessageHandler(_ => throw new HttpRequestException("Connection refused"));
        var client = TestData.CreateClient(handler);

        var result = await client.GetUserAsync("octocat");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NetworkError);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsSuccess_WithNullOptionalFields()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.OkJson(TestData.SampleUserNullFieldsJson));
        var client = TestData.CreateClient(handler);

        var result = await client.GetUserAsync("minimal");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBeNull();
        result.Value.Bio.ShouldBeNull();
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsSuccess_WhenReposExist()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.OkJson(TestData.SampleReposJson));
        var client = TestData.CreateClient(handler);

        var result = await client.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Name.ShouldBe("repo1");
        result.Value[0].StargazersCount.ShouldBe(100);
        result.Value[1].Name.ShouldBe("repo2");
        result.Value[1].Description.ShouldBeNull();
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsEmptyResult_WhenNoRepos()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.OkJson("[]"));
        var client = TestData.CreateClient(handler);

        var result = await client.GetRepositoriesAsync("octocat", 1, 30);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.EmptyResult);
    }

    [Fact]
    public async Task GetRepositoriesAsync_PassesPageAndPerPageInQueryString()
    {
        string? capturedUrl = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return TestData.OkJson("[]");
        });
        var client = TestData.CreateClient(handler);

        await client.GetRepositoriesAsync("octocat", 2, 50);

        capturedUrl.ShouldNotBeNull();
        capturedUrl.ShouldContain("page=2");
        capturedUrl.ShouldContain("per_page=50");
        capturedUrl.ShouldContain("sort=stars");
        capturedUrl.ShouldContain("direction=desc");
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsRateLimited_WhenRateLimitHit()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.RateLimited());
        var client = TestData.CreateClient(handler);

        var result = await client.GetRepositoriesAsync("octocat", 1, 30);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.RateLimited);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsUnknown_WhenUnexpectedStatusCode()
    {
        var handler = new FakeHttpMessageHandler(_ => TestData.Status(HttpStatusCode.Unauthorized));
        var client = TestData.CreateClient(handler);

        var result = await client.GetUserAsync("octocat");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.Unknown);
    }
}
