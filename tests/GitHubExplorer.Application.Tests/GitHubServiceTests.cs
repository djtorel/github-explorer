using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Application.Services;
using GitHubExplorer.Domain.Interfaces;
using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;
using NSubstitute;
using Shouldly;

namespace GitHubExplorer.Application.Tests;

public sealed class GitHubServiceTests
{
    private readonly IGitHubClient _client = Substitute.For<IGitHubClient>();
    private readonly IGitHubService _service;

    public GitHubServiceTests() => _service = new GitHubService(_client);

    [Fact]
    public async Task GetUserAsync_ReturnsMappedDto_WhenUserFound()
    {
        var profile = new UserProfile
        {
            Login = "octocat",
            Name = "The Octocat",
            AvatarUrl = "https://avatars.githubusercontent.com/u/1?v=4",
            Bio = "GitHub mascot",
            Followers = 100,
            PublicRepos = 50,
            HtmlUrl = "https://github.com/octocat"
        };
        _client.GetUserAsync("octocat", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfile>.Success(profile));

        var result = await _service.GetUserAsync("octocat");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Login.ShouldBe("octocat");
        result.Value.Name.ShouldBe("The Octocat");
        result.Value.AvatarUrl.ShouldBe("https://avatars.githubusercontent.com/u/1?v=4");
        result.Value.Bio.ShouldBe("GitHub mascot");
        result.Value.Followers.ShouldBe(100);
        result.Value.PublicRepos.ShouldBe(50);
        result.Value.HtmlUrl.ShouldBe("https://github.com/octocat");
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsMappedDtos_WhenReposFound()
    {
        var repos = new List<Repository>
        {
            new() { Name = "Spoon-Knife", Description = "This repo is for demonstration purposes only.", StargazersCount = 12000, ForksCount = 140000, Language = "HTML", HtmlUrl = "https://github.com/octocat/Spoon-Knife" },
            new() { Name = "Hello-World", Description = "My first repository on GitHub!", StargazersCount = 3000, ForksCount = 2000, Language = null, HtmlUrl = "https://github.com/octocat/Hello-World" }
        };
        _client.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((repos, repos.Count)));

        var result = await _service.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Items[0].Name.ShouldBe("Spoon-Knife");
        result.Value.Items[0].Description.ShouldBe("This repo is for demonstration purposes only.");
        result.Value.Items[0].StargazersCount.ShouldBe(12000);
        result.Value.Items[0].ForksCount.ShouldBe(140000);
        result.Value.Items[0].Language.ShouldBe("HTML");
        result.Value.Items[0].HtmlUrl.ShouldBe("https://github.com/octocat/Spoon-Knife");
        result.Value.Items[1].Name.ShouldBe("Hello-World");
        result.Value.Items[1].Description.ShouldBe("My first repository on GitHub!");
        result.Value.Items[1].Language.ShouldBeNull();
        result.Value.TotalCount.ShouldBe(2);
    }

    [Fact]
    public async Task GetRepositoriesAsync_ReturnsEmptyList_WhenNoRepos()
    {
        _client.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((Array.Empty<Repository>(), 0)));

        var result = await _service.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetUserAsync_PassesThroughNotFoundError()
    {
        _client.GetUserAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfile>.Failure(GitHubError.NotFound));

        var result = await _service.GetUserAsync("nonexistent");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NotFound);
    }

    [Fact]
    public async Task GetUserAsync_PassesThroughRateLimitedError()
    {
        _client.GetUserAsync("any", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfile>.Failure(GitHubError.RateLimited));

        var result = await _service.GetUserAsync("any");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.RateLimited);
    }

    [Fact]
    public async Task GetRepositoriesAsync_PassesThroughNetworkError()
    {
        _client.GetRepositoriesAsync("any", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Failure(GitHubError.NetworkError));

        var result = await _service.GetRepositoriesAsync("any", 1, 30);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.NetworkError);
    }

    [Fact]
    public async Task GetRepositoriesAsync_PassesThroughEmptyResult()
    {
        _client.GetRepositoriesAsync("any", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Failure(GitHubError.EmptyResult));

        var result = await _service.GetRepositoriesAsync("any", 1, 30);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GitHubError.EmptyResult);
    }

    [Fact]
    public async Task GetUserAsync_NullName_PassesThroughNull()
    {
        var profile = new UserProfile
        {
            Login = "octocat",
            Name = null,
            AvatarUrl = "https://avatars.githubusercontent.com/u/1",
            Bio = "GitHub mascot",
            Followers = 100,
            PublicRepos = 50,
            HtmlUrl = "https://github.com/octocat"
        };
        _client.GetUserAsync("octocat", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfile>.Success(profile));

        var result = await _service.GetUserAsync("octocat");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBeNull();
        result.Value.Login.ShouldBe("octocat");
    }

    [Fact]
    public async Task GetUserAsync_NullBio_PassesThroughNull()
    {
        var profile = new UserProfile
        {
            Login = "octocat",
            Name = "The Octocat",
            AvatarUrl = "https://avatars.githubusercontent.com/u/1",
            Bio = null,
            Followers = 100,
            PublicRepos = 50,
            HtmlUrl = "https://github.com/octocat"
        };
        _client.GetUserAsync("octocat", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfile>.Success(profile));

        var result = await _service.GetUserAsync("octocat");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Bio.ShouldBeNull();
    }

    [Fact]
    public async Task GetRepositoriesAsync_PreservesOrderFromClient()
    {
        var repos = new List<Repository>
        {
            new() { Name = "low-stars", Description = "Low", StargazersCount = 10, ForksCount = 1, Language = "C#", HtmlUrl = "https://github.com/octocat/low-stars" },
            new() { Name = "high-stars", Description = "High", StargazersCount = 1000, ForksCount = 100, Language = "TypeScript", HtmlUrl = "https://github.com/octocat/high-stars" }
        };
        _client.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((repos, repos.Count)));

        var result = await _service.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items[0].Name.ShouldBe("low-stars");
        result.Value.Items[1].Name.ShouldBe("high-stars");
        result.Value.Items[0].StargazersCount.ShouldBe(10);
        result.Value.Items[1].StargazersCount.ShouldBe(1000);
    }

    [Fact]
    public async Task GetRepositoriesAsync_NullDescription_PassesThroughNull()
    {
        var repos = new List<Repository>
        {
            new() { Name = "no-desc", Description = null, StargazersCount = 5, ForksCount = 1, Language = "Python", HtmlUrl = "https://github.com/octocat/no-desc" }
        };
        _client.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((repos, repos.Count)));

        var result = await _service.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items[0].Description.ShouldBeNull();
    }

    [Fact]
    public async Task GetRepositoriesAsync_NullLanguage_PassesThroughNull()
    {
        var repos = new List<Repository>
        {
            new() { Name = "no-lang", Description = "No language", StargazersCount = 5, ForksCount = 1, Language = null, HtmlUrl = "https://github.com/octocat/no-lang" }
        };
        _client.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((repos, repos.Count)));

        var result = await _service.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items[0].Language.ShouldBeNull();
    }

    [Fact]
    public async Task GetRepositoriesAsync_EmptyList_HasZeroTotalCount()
    {
        _client.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((Array.Empty<Repository>(), 0)));

        var result = await _service.GetRepositoriesAsync("octocat", 1, 30);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
    }
}
