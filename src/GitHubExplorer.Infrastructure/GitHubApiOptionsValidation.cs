using Microsoft.Extensions.Options;

namespace GitHubExplorer.Infrastructure;

public class GitHubApiOptionsValidation : IValidateOptions<GitHubApiOptions>
{
    public ValidateOptionsResult Validate(string? name, GitHubApiOptions options) =>
        ValidateBaseUrl(options)
            .Concat(ValidateToken(options))
            .ToArray() is { Length: > 0 } errors
                ? ValidateOptionsResult.Fail(errors)
                : ValidateOptionsResult.Success;

    private static IEnumerable<string> ValidateBaseUrl(GitHubApiOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            yield return "BaseUrl is required.";
            yield break;
        }

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            yield return "BaseUrl must be a valid HTTPS URI.";
        }
    }

    private static IEnumerable<string> ValidateToken(GitHubApiOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Token))
            yield break;

        if (!options.Token.StartsWith("ghp_") && !options.Token.StartsWith("github_pat_"))
        {
            yield return "Token must start with 'ghp_' or 'github_pat_'.";
        }
    }
}
