namespace GitHubExplorer.Domain.Models;

public sealed class Repository
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required int StargazersCount { get; init; }
    public required int ForksCount { get; init; }
    public string? Language { get; init; }
    public required string HtmlUrl { get; init; }
}
