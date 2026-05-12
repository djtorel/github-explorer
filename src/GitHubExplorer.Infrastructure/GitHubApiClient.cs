using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using GitHubExplorer.Domain.Enums;
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

    public async Task<Result<(IReadOnlyList<Repository> Items, int TotalCount)>> GetRepositoriesAsync(
        string username, int page, int perPage, SortBy sortBy = SortBy.StarsDesc, CancellationToken ct = default)
    {
        if (sortBy is SortBy.NameAsc or SortBy.NameDesc)
        {
            var direction = sortBy == SortBy.NameAsc ? "asc" : "desc";
            var url = $"users/{Uri.EscapeDataString(username)}/repos?page={page}&per_page={perPage}&sort=full_name&direction={direction}";
            return await FetchSinglePageAsync(url, perPage, ct);
        }

        return await FetchAllAndSortAsync(username, page, perPage, sortBy, ct);
    }

    private async Task<Result<(IReadOnlyList<Repository> Items, int TotalCount)>> FetchAllAndSortAsync(
        string username, int page, int perPage, SortBy sortBy, CancellationToken ct)
    {
        const int fetchPerPage = 100;
        const int maxPages = 10;

        var allRepos = new List<Repository>();
        var currentPage = 1;
        string? nextUrl = $"users/{Uri.EscapeDataString(username)}/repos?page=1&per_page={fetchPerPage}";

        while (!string.IsNullOrEmpty(nextUrl) && currentPage <= maxPages)
        {
            var result = await FetchPageWithMetadataAsync(nextUrl, fetchPerPage, ct, isRelative: true);
            if (!result.IsSuccess)
                return Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Failure(result.Error);

            allRepos.AddRange(result.Value.Items);
            nextUrl = result.Value.NextUrl;
            currentPage++;
        }

        if (allRepos.Count == 0)
            return Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Failure(GitHubError.EmptyResult);

        var sorted = sortBy == SortBy.StarsDesc
            ? allRepos.OrderByDescending(r => r.StargazersCount).ToList()
            : allRepos.OrderBy(r => r.StargazersCount).ToList();

        var totalCount = sorted.Count;
        var skip = (page - 1) * perPage;
        var paged = skip >= totalCount
            ? new List<Repository>()
            : sorted.Skip(skip).Take(perPage).ToList();

        return Result<(IReadOnlyList<Repository> Items, int TotalCount)>.Success((paged, totalCount));
    }

    private sealed record FetchPageResult(IReadOnlyList<Repository> Items, int TotalCount, string? NextUrl);

    private async Task<Result<FetchPageResult>> FetchPageWithMetadataAsync(
        string url, int perPage, CancellationToken ct, bool isRelative = false)
    {
        try
        {
            var requestUrl = isRelative ? url : $"{httpClient.BaseAddress}{url}";
            var response = await httpClient.GetAsync(requestUrl, ct);
            var error = MapToError(response);

            if (error.HasValue)
            {
                if (error.Value == GitHubError.Unknown)
                    logger.LogWarning("Unexpected status code {StatusCode} from GitHub API", response.StatusCode);
                return Result<FetchPageResult>.Failure(error.Value);
            }

            var repos = await response.Content.ReadFromJsonAsync<List<Repository>>(JsonOptions, ct);

            if (repos is null || repos.Count == 0)
                return Result<FetchPageResult>.Failure(GitHubError.EmptyResult);

            var linkHeader = response.Headers.TryGetValues("Link", out var linkValues)
                ? linkValues.FirstOrDefault()
                : null;

            var totalCount = ExtractTotalCountFromLinkHeader(linkHeader, perPage);
            if (totalCount == 0) totalCount = repos.Count;

            var nextUrl = ExtractNextUrl(linkHeader);

            return Result<FetchPageResult>.Success(new FetchPageResult(repos, totalCount, nextUrl));
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning("GitHub API request timed out for repos");
            return Result<FetchPageResult>.Failure(GitHubError.NetworkError);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Network error calling GitHub API for repos");
            return Result<FetchPageResult>.Failure(GitHubError.NetworkError);
        }
    }

    private async Task<Result<(IReadOnlyList<Repository> Items, int TotalCount)>> FetchSinglePageAsync(
        string url, int perPage, CancellationToken ct, bool isRelative = false) =>
        (await FetchPageWithMetadataAsync(url, perPage, ct, isRelative))
            .Map(r => (r.Items, r.TotalCount));

    private static string? ExtractNextUrl(string? linkHeader)
    {
        if (string.IsNullOrWhiteSpace(linkHeader))
            return null;

        var nextMatch = Regex.Match(linkHeader, @"<([^>]+)>;\s*rel=""next""");
        return nextMatch.Success ? nextMatch.Groups[1].Value : null;
    }

    private static int ExtractTotalCountFromLinkHeader(string? linkHeader, int perPage)
    {
        if (string.IsNullOrWhiteSpace(linkHeader)) return 0;

        var lastMatch = Regex.Match(
            linkHeader,
            @"<[^>]+[?&]page=(\d+)[^>]*>;\s*rel=""last""");

        if (lastMatch.Success && int.TryParse(lastMatch.Groups[1].Value, out var lastPage))
            return lastPage * perPage;

        return 0;
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
