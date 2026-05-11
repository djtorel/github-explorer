using System.Net;

namespace GitHubExplorer.Infrastructure;

public class GitHubApiException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
