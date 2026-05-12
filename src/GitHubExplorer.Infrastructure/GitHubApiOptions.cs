namespace GitHubExplorer.Infrastructure;

public class GitHubApiOptions
{
    public const string SectionName = "GitHubApi";

    public string BaseUrl { get; init; } = "https://api.github.com";
    public string? Token { get; init; }
}
