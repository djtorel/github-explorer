namespace GitHubExplorer.Application.Constants;

public static class ApiErrorMessages
{
    public const string UserNotFound = "User not found.";
    public const string RateLimitExceeded = "GitHub API rate limit exceeded.";
    public const string NetworkUnavailable = "Unable to reach GitHub API.";
    public const string UnexpectedError = "An unexpected error occurred.";

    public const string UsernameCannotBeEmpty = "Username cannot be empty.";
    public const string PageMustBePositive = "Page must be 1 or greater.";
    public const string PageSizeInvalid = "Page size must be 10, 30, or 50.";
    public const string SortByInvalid = "Sort must be stars_desc, stars_asc, name_asc, or name_desc.";
}
