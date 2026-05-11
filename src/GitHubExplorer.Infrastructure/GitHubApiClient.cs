using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GitHubExplorer.Domain.Interfaces;
using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;
using Microsoft.Extensions.Logging;

namespace GitHubExplorer.Infrastructure;

public sealed class GitHubApiClient(HttpClient httpClient, ILogger<GitHubApiClient> logger) : IGitHubClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public Task<Result<UserProfile>> GetUserAsync(string username, CancellationToken ct = default) =>
        SendRequestAsync(
            () => httpClient.GetAsync($"users/{Uri.EscapeDataString(username)}", ct),
            response => response.Content.ReadFromJsonAsync<UserProfile>(JsonOptions, ct),
            $"user {username}",
            ct);

    public async Task<Result<IReadOnlyList<Repository>>> GetRepositoriesAsync(
        string username, int page, int perPage, CancellationToken ct = default)
    {
        var url = $"users/{Uri.EscapeDataString(username)}/repos?page={page}&per_page={perPage}&sort=stars&direction=desc";

        var result = await SendRequestAsync(
            () => httpClient.GetAsync(url, ct),
            response => response.Content.ReadFromJsonAsync<List<Repository>>(JsonOptions, ct),
            $"repos of user {username}",
            ct);

        return result.Bind(repos =>
            repos.Count == 0
                ? Result<IReadOnlyList<Repository>>.Failure(GitHubError.EmptyResult)
                : Result<IReadOnlyList<Repository>>.Success(repos));
    }

    private async Task<Result<T>> SendRequestAsync<T>(
        Func<Task<HttpResponseMessage>> send,
        Func<HttpResponseMessage, Task<T?>> deserialize,
        string logContext,
        CancellationToken ct)
    {
        try
        {
            var response = await send();
            var error = MapToError(response);

            if (error.HasValue)
            {
                if (error.Value == GitHubError.Unknown)
                    logger.LogWarning("Unexpected status code {StatusCode} from GitHub API", response.StatusCode);
                return Result<T>.Failure(error.Value);
            }

            var data = await deserialize(response);
            return data is null
                ? Result<T>.Failure(GitHubError.EmptyResult)
                : Result<T>.Success(data);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("GitHub API request timed out for {Context}", logContext);
            return Result<T>.Failure(GitHubError.NetworkError);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Network error calling GitHub API for {Context}", logContext);
            return Result<T>.Failure(GitHubError.NetworkError);
        }
    }

    private static GitHubError? MapToError(HttpResponseMessage response) =>
        response.StatusCode switch
        {
            HttpStatusCode.NotFound => GitHubError.NotFound,
            HttpStatusCode.Forbidden => IsRateLimited(response) ? GitHubError.RateLimited : GitHubError.Unknown,
            _ when !response.IsSuccessStatusCode => GitHubError.Unknown,
            _ => null
        };

    private static bool IsRateLimited(HttpResponseMessage response) =>
        response.Headers.TryGetValues("x-ratelimit-remaining", out var values)
        && values.FirstOrDefault() == "0";
}
