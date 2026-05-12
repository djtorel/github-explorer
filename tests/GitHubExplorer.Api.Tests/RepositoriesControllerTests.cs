using System.Net;
using System.Text.Json;
using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Domain.Enums;
using GitHubExplorer.Domain.Results;
using NSubstitute;
using Shouldly;

namespace GitHubExplorer.Api.Tests;

public sealed class RepositoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public RepositoriesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRepositories_Returns200WithPaginatedRepos()
    {
        var repos = new List<RepositoryDto>
        {
            new("repo1", "First", 100, 10, "C#", "https://github.com/u/r1"),
            new("repo2", "Second", 50, 5, "TypeScript", "https://github.com/u/r2"),
            new("repo3", "Third", 25, 2, "Python", "https://github.com/u/r3"),
        };
        _factory.GitHubServiceMock.GetRepositoriesAsync("octocat", 1, 30, Arg.Any<SortBy>(), Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Success((repos, repos.Count)));

        var response = await _client.GetAsync("/api/users/octocat/repos?page=1&perPage=30");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await DeserializeAsync<ApiResponseDto<PaginatedResultDto<RepositoryDto>>>(response);
        body.Success.ShouldBeTrue();
        body.Data.ShouldNotBeNull();
        body.Data.Items.Count.ShouldBe(3);
        body.Data.Items[0].Name.ShouldBe("repo1");
        body.Data.Items[1].Name.ShouldBe("repo2");
        body.Data.Items[2].Name.ShouldBe("repo3");
        body.Data.TotalCount.ShouldBe(3);
    }

    [Fact]
    public async Task GetRepositories_InvalidPageSize_Returns400()
    {
        var response = await _client.GetAsync("/api/users/octocat/repos?page=1&perPage=25");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await DeserializeAsync<ApiErrorDto>(response);
        body.Code.ShouldBe("InvalidPageSize");
    }

    [Fact]
    public async Task GetRepositories_InvalidSortBy_Returns400()
    {
        var response = await _client.GetAsync("/api/users/octocat/repos?page=1&perPage=30&sortBy=invalid");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await DeserializeAsync<ApiErrorDto>(response);
        body.Code.ShouldBe("InvalidSortBy");
    }

    [Fact]
    public async Task GetRepositories_NameSort_PassesThroughToService()
    {
        var repos = new List<RepositoryDto>
        {
            new("alpha", "Alpha", 10, 1, "C#", "https://github.com/u/alpha"),
            new("beta", "Beta", 20, 2, "TypeScript", "https://github.com/u/beta"),
        };
        _factory.GitHubServiceMock.GetRepositoriesAsync("octocat", 1, 30, SortBy.NameAsc, Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Success((repos, repos.Count)));

        var response = await _client.GetAsync("/api/users/octocat/repos?page=1&perPage=30&sortBy=name_asc");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await DeserializeAsync<ApiResponseDto<PaginatedResultDto<RepositoryDto>>>(response);
        body.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task GetRepositories_UserNotFound_Returns404()
    {
        _factory.GitHubServiceMock.GetRepositoriesAsync("ghost", 1, 30, Arg.Any<SortBy>(), Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Failure(GitHubError.NotFound));

        var response = await _client.GetAsync("/api/users/ghost/repos?page=1&perPage=30");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("NotFound");
    }

    [Fact]
    public async Task GetRepositories_RateLimited_Returns429()
    {
        _factory.GitHubServiceMock.GetRepositoriesAsync("any", 1, 30, Arg.Any<SortBy>(), Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Failure(GitHubError.RateLimited));

        var response = await _client.GetAsync("/api/users/any/repos?page=1&perPage=30");

        response.StatusCode.ShouldBe((HttpStatusCode)429);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("RateLimited");
    }

    [Fact]
    public async Task GetRepositories_NetworkError_Returns503()
    {
        _factory.GitHubServiceMock.GetRepositoriesAsync("any", 1, 30, Arg.Any<SortBy>(), Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Failure(GitHubError.NetworkError));

        var response = await _client.GetAsync("/api/users/any/repos?page=1&perPage=30");

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("NetworkError");
    }

    [Fact]
    public async Task GetRepositories_EmptyList_Returns200WithEmptyItems()
    {
        _factory.GitHubServiceMock.GetRepositoriesAsync("empty", 1, 30, Arg.Any<SortBy>(), Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Success((Array.Empty<RepositoryDto>(), 0)));

        var response = await _client.GetAsync("/api/users/empty/repos?page=1&perPage=30");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await DeserializeAsync<ApiResponseDto<PaginatedResultDto<RepositoryDto>>>(response);
        body.Success.ShouldBeTrue();
        body.Data.ShouldNotBeNull();
        body.Data.Items.ShouldBeEmpty();
        body.Data.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetRepositories_EmptyResultError_Returns200WithEmptyArray()
    {
        _factory.GitHubServiceMock.GetRepositoriesAsync("any", 1, 30, Arg.Any<SortBy>(), Arg.Any<CancellationToken>())
            .Returns(Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>.Failure(GitHubError.EmptyResult));

        var response = await _client.GetAsync("/api/users/any/repos?page=1&perPage=30");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeTrue();
    }

    private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        })!;
    }
}
