using GitHubExplorer.Application.Constants;
using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Interfaces;
using GitHubExplorer.Api.Helpers;
using GitHubExplorer.Domain.Enums;
using GitHubExplorer.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace GitHubExplorer.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class RepositoriesController(IGitHubService service) : ControllerBase
{
    private static readonly int[] ValidPageSizes = [10, 30, 50];

    private static readonly string[] ValidSortValues = ["stars_desc", "stars_asc", "name_asc", "name_desc"];

    [HttpGet("{username}/repos")]
    public async Task<IActionResult> GetRepositories(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 30,
        [FromQuery] string sortBy = "stars_desc",
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(ApiResponseFactory.CreateError(ApiErrorCodes.InvalidUsername, ApiErrorMessages.UsernameCannotBeEmpty));

        if (page < 1)
            return BadRequest(ApiResponseFactory.CreateError(ApiErrorCodes.InvalidPage, ApiErrorMessages.PageMustBePositive));

        if (!ValidPageSizes.Contains(perPage))
            return BadRequest(ApiResponseFactory.CreateError(ApiErrorCodes.InvalidPageSize, ApiErrorMessages.PageSizeInvalid));

        if (!ValidSortValues.Contains(sortBy))
            return BadRequest(ApiResponseFactory.CreateError(ApiErrorCodes.InvalidSortBy, ApiErrorMessages.SortByInvalid));

        var sort = sortBy switch
        {
            "stars_asc" => SortBy.StarsAsc,
            "name_asc" => SortBy.NameAsc,
            "name_desc" => SortBy.NameDesc,
            _ => SortBy.StarsDesc,
        };

        var result = await service.GetRepositoriesAsync(username.Trim(), page, perPage, sort, ct);
        return result.Match(
            result => Ok(ApiResponseFactory.WrapSuccess(new PaginatedResultDto<RepositoryDto>(result.Items, result.TotalCount))),
            error => ApiResponseFactory.MapGitHubError(error, this));
    }
}
