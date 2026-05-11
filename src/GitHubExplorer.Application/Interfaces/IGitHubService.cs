using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Domain.Results;

namespace GitHubExplorer.Application.Interfaces;

public interface IGitHubService
{
    Task<Result<UserProfileDto>> GetUserAsync(string username, CancellationToken ct = default);

    Task<Result<IReadOnlyList<RepositoryDto>>> GetRepositoriesAsync(
        string username, int page, int perPage, CancellationToken ct = default);
}
