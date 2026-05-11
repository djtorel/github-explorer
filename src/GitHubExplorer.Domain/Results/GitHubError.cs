namespace GitHubExplorer.Domain.Results;

public enum GitHubError
{
    NotFound,
    RateLimited,
    NetworkError,
    EmptyResult,
    Unknown
}
