using System.Net;
using System.Text.Json;
using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Domain.Results;
using NSubstitute;
using Shouldly;

namespace GitHubExplorer.Api.Tests;

public sealed class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUserProfile_Returns200WithUserData()
    {
        var dto = new UserProfileDto(
            Login: "octocat",
            Name: "The Octocat",
            AvatarUrl: "https://avatars.githubusercontent.com/u/1",
            Bio: "GitHub mascot",
            Followers: 1000,
            PublicRepos: 50,
            HtmlUrl: "https://github.com/octocat"
        );
        _factory.GitHubServiceMock.GetUserAsync("octocat", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfileDto>.Success(dto));

        var response = await _client.GetAsync("/api/users/octocat");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await DeserializeAsync<ApiResponseDto<UserProfileDto>>(response);
        body.Success.ShouldBeTrue();
        body.Data.ShouldNotBeNull();
        body.Data.Login.ShouldBe("octocat");
        body.Data.Name.ShouldBe("The Octocat");
        body.Data.AvatarUrl.ShouldBe("https://avatars.githubusercontent.com/u/1");
        body.Data.Bio.ShouldBe("GitHub mascot");
        body.Data.Followers.ShouldBe(1000);
        body.Data.PublicRepos.ShouldBe(50);
        body.Data.HtmlUrl.ShouldBe("https://github.com/octocat");
    }

    [Fact]
    public async Task GetUserProfile_UserNotFound_Returns404()
    {
        _factory.GitHubServiceMock.GetUserAsync("ghost", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfileDto>.Failure(GitHubError.NotFound));

        var response = await _client.GetAsync("/api/users/ghost");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("NotFound");
    }

    [Fact]
    public async Task GetUserProfile_RateLimited_Returns429()
    {
        _factory.GitHubServiceMock.GetUserAsync("any", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfileDto>.Failure(GitHubError.RateLimited));

        var response = await _client.GetAsync("/api/users/any");

        response.StatusCode.ShouldBe((HttpStatusCode)429);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("RateLimited");
    }

    [Fact]
    public async Task GetUserProfile_NetworkError_Returns503()
    {
        _factory.GitHubServiceMock.GetUserAsync("any", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfileDto>.Failure(GitHubError.NetworkError));

        var response = await _client.GetAsync("/api/users/any");

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("NetworkError");
    }

    [Fact]
    public async Task GetUserProfile_ServerError_Returns500()
    {
        _factory.GitHubServiceMock.GetUserAsync("any", Arg.Any<CancellationToken>())
            .Returns(Result<UserProfileDto>.Failure(GitHubError.Unknown));

        var response = await _client.GetAsync("/api/users/any");

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        var body = await DeserializeAsync<ApiResponseDto<object>>(response);
        body.Success.ShouldBeFalse();
        body.Error.ShouldNotBeNull();
        body.Error.Code.ShouldBe("Unknown");
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
