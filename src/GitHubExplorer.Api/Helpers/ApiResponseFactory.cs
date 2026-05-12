using GitHubExplorer.Application.Constants;
using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace GitHubExplorer.Api.Helpers;

public static class ApiResponseFactory
{
    public static ApiErrorDto CreateError(string code, string message) =>
        new(code, message);

    public static IActionResult MapGitHubError(GitHubError error, ControllerBase controller) =>
        error switch
        {
            GitHubError.NotFound => controller.NotFound(WrapError(ApiErrorCodes.NotFound, ApiErrorMessages.UserNotFound)),
            GitHubError.RateLimited => controller.StatusCode(429, WrapError(ApiErrorCodes.RateLimited, ApiErrorMessages.RateLimitExceeded)),
            GitHubError.NetworkError => controller.StatusCode(503, WrapError(ApiErrorCodes.NetworkError, ApiErrorMessages.NetworkUnavailable)),
            GitHubError.EmptyResult => controller.Ok(WrapSuccess<object>(null)),
            _ => controller.StatusCode(500, WrapError(ApiErrorCodes.Unknown, ApiErrorMessages.UnexpectedError)),
        };

    public static ApiResponseDto<T> WrapSuccess<T>(T? data) =>
        new(true, data, null);

    public static ApiResponseDto<object> WrapError(string code, string message) =>
        new(false, null, new ApiErrorDto(code, message));
}
