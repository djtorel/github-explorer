using System.Net;
using GitHubExplorer.Domain.Enums;
using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;
using Shouldly;

namespace GitHubExplorer.Infrastructure.Tests;

public class GitHubApiClientTests
{
    [Fact]
    public async Task GetUserAsync_ReturnsSuccess_WhenUserExists()
    {
        var result = await RunUserTest(_ => TestData.OkJson(TestData.SampleUserJson), "octocat");

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
        var result = await RunUserTest(_ => TestData.Status(HttpStatusCode.NotFound), "not-a-real-user-12345");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsRateLimited_WhenRateLimitExceeded()
    {
        var result = await RunUserTest(_ => TestData.RateLimited(), "octocat");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.RateLimited);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsNetworkError_WhenRequestFails()
    {
        var result = await RunUserTest(_ => throw new HttpRequestException("Connection refused"), "octocat");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NetworkError);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsSuccess_WithNullOptionalFields()
    {
        var result = await RunUserTest(_ => TestData.OkJson(TestData.SampleUserNullFieldsJson), "minimal");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBeNull();
        result.Value.Bio.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserAsync_ReturnsUnknown_WhenUnexpectedStatusCode()
    {
        var result = await RunUserTest(_ => TestData.Status(HttpStatusCode.Unauthorized), "octocat");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.Unknown);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsSuccess_WhenReposExist()
    {
        var result = await RunReposTest(_ => TestData.OkJson(TestData.SampleReposJson), "octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Items[0].Name.ShouldBe("repo1");
        result.Value.Items[0].StargazersCount.ShouldBe(100);
        result.Value.Items[1].Name.ShouldBe("repo2");
        result.Value.Items[1].Description.ShouldBeNull();
        result.Value.TotalCount.ShouldBe(2);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsEmptyResult_WhenNoRepos()
    {
        var result = await RunReposTest(_ => TestData.OkJson("[]"), "octocat", 1, 30);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.EmptyResult);
    }

    [Fact]
    public async Task GetRepositoriesAsync_StarSort_FetchesAllWithMaxPerPage()
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
        capturedUrl.ShouldContain("page=1");
        capturedUrl.ShouldContain("per_page=100");
        capturedUrl.ShouldNotContain("sort=stars");
    }

    [Fact]
    public async Task GetRepositoriesAsync_NameAsc_PassesFullNameSort()
    {
        string? capturedUrl = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return TestData.OkJson("[]");
        });
        var client = TestData.CreateClient(handler);

        await client.GetRepositoriesAsync("octocat", 1, 30, GitHubExplorer.Domain.Enums.SortBy.NameAsc);

        capturedUrl.ShouldNotBeNull();
        capturedUrl.ShouldContain("sort=full_name");
        capturedUrl.ShouldContain("direction=asc");
    }

    [Fact]
    public async Task GetRepositoriesAsync_NameDesc_PassesFullNameSortDesc()
    {
        string? capturedUrl = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            capturedUrl = req.RequestUri?.ToString();
            return TestData.OkJson("[]");
        });
        var client = TestData.CreateClient(handler);

        await client.GetRepositoriesAsync("octocat", 1, 30, GitHubExplorer.Domain.Enums.SortBy.NameDesc);

        capturedUrl.ShouldNotBeNull();
        capturedUrl.ShouldContain("sort=full_name");
        capturedUrl.ShouldContain("direction=desc");
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsRateLimited_WhenRateLimitHit()
    {
        var result = await RunReposTest(_ => TestData.RateLimited(), "octocat", 1, 30);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.RateLimited);
    }

    private static async Task<Result<UserProfile>> RunUserTest(
        Func<HttpRequestMessage, HttpResponseMessage> handler,
        string username)
    {
        var client = TestData.CreateClient(new FakeHttpMessageHandler(handler));
        return await client.GetUserAsync(username);
    }

    private static async Task<Result<(IReadOnlyList<Repository> Items, int TotalCount)>> RunReposTest(
        Func<HttpRequestMessage, HttpResponseMessage> handler,
        string username,
        int page,
        int perPage)
    {
        var client = TestData.CreateClient(new FakeHttpMessageHandler(handler));
        return await client.GetRepositoriesAsync(username, page, perPage);
    }
}
