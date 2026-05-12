using GitHubExplorer.Application.Constants;
using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Api.Helpers;
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
            return BadRequest(ApiResponseFactory.CreateError(ApiErrorCodes.InvalidUsername, ApiErrorMessages.UsernameCannotBeEmpty));

        var result = await service.GetUserAsync(username.Trim(), ct);
        return result.Match(
            user => Ok(ApiResponseFactory.WrapSuccess(user)),
            error => ApiResponseFactory.MapGitHubError(error, this));
    }
}
