namespace GitHubExplorer.Application.DTOs;

public sealed record UserProfileDto(
    string Login,
    string? Name,
    string AvatarUrl,
    string? Bio,
    int Followers,
    int PublicRepos,
    string HtmlUrl);
