using System.Net;

namespace GitHubExplorer.Infrastructure;

public class GitHubApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public GitHubApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
