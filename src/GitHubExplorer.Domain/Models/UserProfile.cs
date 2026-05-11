namespace GitHubExplorer.Domain.Models;

public sealed class UserProfile
{
    public required string Login { get; init; }
    public string? Name { get; init; }
    public required string AvatarUrl { get; init; }
    public string? Bio { get; init; }
    public required int Followers { get; init; }
    public required int PublicRepos { get; init; }
    public required string HtmlUrl { get; init; }
}
