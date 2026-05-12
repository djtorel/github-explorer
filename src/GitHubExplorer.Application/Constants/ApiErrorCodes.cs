namespace GitHubExplorer.Application.Constants;

public static class ApiErrorCodes
{
    public const string NotFound = nameof(NotFound);
    public const string RateLimited = nameof(RateLimited);
    public const string NetworkError = nameof(NetworkError);
    public const string EmptyResult = nameof(EmptyResult);
    public const string Unknown = nameof(Unknown);

    public const string InvalidUsername = nameof(InvalidUsername);
    public const string InvalidPage = nameof(InvalidPage);
    public const string InvalidPageSize = nameof(InvalidPageSize);
    public const string InvalidSortBy = nameof(InvalidSortBy);
}
