using Microsoft.Extensions.Options;

namespace GitHubExplorer.Infrastructure;

public class GitHubApiOptionsValidation : IValidateOptions<GitHubApiOptions>
{
    public ValidateOptionsResult Validate(string? name, GitHubApiOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add("BaseUrl is required.");
        }
        else if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            errors.Add("BaseUrl must be a valid HTTPS URI.");
        }

        if (!string.IsNullOrWhiteSpace(options.Token))
        {
            if (!options.Token.StartsWith("ghp_") && !options.Token.StartsWith("github_pat_"))
            {
                errors.Add("Token must start with 'ghp_' or 'github_pat_'.");
            }
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}
