using GitHubExplorer.Domain.Models;
using GitHubExplorer.Domain.Results;

namespace GitHubExplorer.Domain.Interfaces;

public interface IGitHubClient
{
    Task<Result<UserProfile>> GetUserAsync(string username, CancellationToken ct = default);

    Task<Result<(IReadOnlyList<Repository> Items, int TotalCount)>> GetRepositoriesAsync(
        string username, int page, int perPage, CancellationToken ct = default);
}
