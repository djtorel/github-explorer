using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Domain.Enums;
using GitHubExplorer.Domain.Interfaces;
using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;

namespace GitHubExplorer.Application.Services;

public sealed class GitHubService(IGitHubClient client) : IGitHubService
{
    public Task<Result<UserProfileDto>> GetUserAsync(string username, CancellationToken ct = default) =>
        client.GetUserAsync(username, ct).MapAsync(MapToDto);

    public Task<Result<(IReadOnlyList<RepositoryDto> Items, int TotalCount)>> GetRepositoriesAsync(
        string username, int page, int perPage, SortBy sortBy = SortBy.StarsDesc, CancellationToken ct = default) =>
        client.GetRepositoriesAsync(username, page, perPage, sortBy, ct)
            .MapAsync(r => (MapToDtos(r.Items), r.TotalCount));

    private static UserProfileDto MapToDto(UserProfile user) => new(
        user.Login, user.Name, user.AvatarUrl, user.Bio,
        user.Followers, user.PublicRepos, user.HtmlUrl);

    private static IReadOnlyList<RepositoryDto> MapToDtos(IReadOnlyList<Repository> repos) =>
        [.. repos.Select(r => new RepositoryDto(
            r.Name, r.Description, r.StargazersCount,
            r.ForksCount, r.Language, r.HtmlUrl))];
}
