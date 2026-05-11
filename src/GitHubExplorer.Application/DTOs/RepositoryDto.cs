namespace GitHubExplorer.Application.DTOs;

public sealed record RepositoryDto(
    string Name,
    string? Description,
    int StargazersCount,
    int ForksCount,
    string? Language,
    string HtmlUrl);
