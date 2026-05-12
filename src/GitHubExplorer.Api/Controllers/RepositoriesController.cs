using GitHubExplorer.Application.DTOs;
using GitHubExplorer.Application.Interfaces;
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
            return BadRequest(CreateError("InvalidUsername", "Username cannot be empty."));

        if (page < 1)
            return BadRequest(CreateError("InvalidPage", "Page must be 1 or greater."));

        if (!ValidPageSizes.Contains(perPage))
            return BadRequest(CreateError("InvalidPageSize", "Page size must be 10, 30, or 50."));

        if (!ValidSortValues.Contains(sortBy))
            return BadRequest(CreateError("InvalidSortBy", "Sort must be stars_desc, stars_asc, name_asc, or name_desc."));

        var sort = sortBy switch
        {
            "stars_asc" => SortBy.StarsAsc,
            "name_asc" => SortBy.NameAsc,
            "name_desc" => SortBy.NameDesc,
            _ => SortBy.StarsDesc,
        };

        var result = await service.GetRepositoriesAsync(username.Trim(), page, perPage, sort, ct);
        return result.Match(
            result => Ok(new ApiResponseDto<PaginatedResultDto<RepositoryDto>>(true,
                new PaginatedResultDto<RepositoryDto>(result.Items, result.TotalCount), null)),
            error => MapErrorToActionResult(error));
    }

    private IActionResult MapErrorToActionResult(GitHubError error) => error switch
    {
        GitHubError.NotFound => NotFound(new ApiResponseDto<object>(false, null, new ApiErrorDto("NotFound", "User not found."))),
        GitHubError.RateLimited => StatusCode(429, new ApiResponseDto<object>(false, null, new ApiErrorDto("RateLimited", "GitHub API rate limit exceeded."))),
        GitHubError.NetworkError => StatusCode(503, new ApiResponseDto<object>(false, null, new ApiErrorDto("NetworkError", "Unable to reach GitHub API."))),
        GitHubError.EmptyResult => Ok(new ApiResponseDto<object>(true, Array.Empty<object>(), null)),
        _ => StatusCode(500, new ApiResponseDto<object>(false, null, new ApiErrorDto("Unknown", "An unexpected error occurred.")))
    };

    private static ApiErrorDto CreateError(string code, string message) =>
        new(code, message);
}
