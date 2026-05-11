using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace GitHubExplorer.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IGitHubService service) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(CreateError("InvalidUsername", "Username cannot be empty."));

        var result = await service.GetUserAsync(username.Trim(), ct);
        return result.Match(
            user => Ok(new ApiResponseDto<UserProfileDto>(true, user, null)),
            error => MapErrorToActionResult(error));
    }

    private IActionResult MapErrorToActionResult(GitHubError error) => error switch
    {
        GitHubError.NotFound => NotFound(new ApiResponseDto<object>(false, null, new ApiErrorDto("NotFound", "User not found."))),
        GitHubError.RateLimited => StatusCode(429, new ApiResponseDto<object>(false, null, new ApiErrorDto("RateLimited", "GitHub API rate limit exceeded."))),
        GitHubError.NetworkError => StatusCode(503, new ApiResponseDto<object>(false, null, new ApiErrorDto("NetworkError", "Unable to reach GitHub API."))),
        GitHubError.EmptyResult => Ok(new ApiResponseDto<object>(true, null, null)),
        _ => StatusCode(500, new ApiResponseDto<object>(false, null, new ApiErrorDto("Unknown", "An unexpected error occurred.")))
    };

    private static ApiErrorDto CreateError(string code, string message) =>
        new(code, message);
}
