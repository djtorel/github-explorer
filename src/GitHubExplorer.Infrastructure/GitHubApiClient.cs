using System.Net.Http.Json;
using System.Text.Json;
using GitHubExplorer.Domain.Interfaces;
using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;
using Microsoft.Extensions.Logging;

namespace GitHubExplorer.Infrastructure;

public sealed class GitHubApiClient : IGitHubClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubApiClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public GitHubApiClient(HttpClient httpClient, ILogger<GitHubApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<UserProfile>> GetUserAsync(string username, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(username)}", ct);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result<UserProfile>.Failure(GitHubError.NotFound);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return HandleForbidden<UserProfile>(response);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Unexpected status code {StatusCode} from GitHub API", response.StatusCode);
                return Result<UserProfile>.Failure(GitHubError.Unknown);
            }

            var profile = await response.Content.ReadFromJsonAsync<UserProfile>(JsonOptions, ct);

            if (profile is null)
            {
                return Result<UserProfile>.Failure(GitHubError.EmptyResult);
            }

            return Result<UserProfile>.Success(profile);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("GitHub API request timed out for user {Username}", username);
            return Result<UserProfile>.Failure(GitHubError.NetworkError);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error calling GitHub API for user {Username}", username);
            return Result<UserProfile>.Failure(GitHubError.NetworkError);
        }
    }

    public async Task<Result<IReadOnlyList<Repository>>> GetRepositoriesAsync(
        string username, int page, int perPage, CancellationToken ct = default)
    {
        try
        {
            var url = $"users/{Uri.EscapeDataString(username)}/repos?page={page}&per_page={perPage}&sort=stars&direction=desc";
            var response = await _httpClient.GetAsync(url, ct);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result<IReadOnlyList<Repository>>.Failure(GitHubError.NotFound);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return HandleForbidden<IReadOnlyList<Repository>>(response);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Unexpected status code {StatusCode} from GitHub API", response.StatusCode);
                return Result<IReadOnlyList<Repository>>.Failure(GitHubError.Unknown);
            }

            var repos = await response.Content.ReadFromJsonAsync<List<Repository>>(JsonOptions, ct);

            if (repos is null || repos.Count == 0)
            {
                return Result<IReadOnlyList<Repository>>.Failure(GitHubError.EmptyResult);
            }

            return Result<IReadOnlyList<Repository>>.Success(repos);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("GitHub API request timed out for repos of user {Username}", username);
            return Result<IReadOnlyList<Repository>>.Failure(GitHubError.NetworkError);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error calling GitHub API for repos of user {Username}", username);
            return Result<IReadOnlyList<Repository>>.Failure(GitHubError.NetworkError);
        }
    }

    private static Result<T> HandleForbidden<T>(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("x-ratelimit-remaining", out var values)
            && values.FirstOrDefault() == "0")
        {
            return Result<T>.Failure(GitHubError.RateLimited);
        }

        return Result<T>.Failure(GitHubError.Unknown);
    }
}
