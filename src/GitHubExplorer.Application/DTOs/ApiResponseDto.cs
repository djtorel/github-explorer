namespace GitHubExplorer.Application.DTOs;

public sealed record ApiResponseDto<T>(bool Success, T? Data, ApiErrorDto? Error);

public sealed record ApiErrorDto(string Code, string Message);

public sealed record PaginatedResultDto<T>(IReadOnlyList<T> Items, int TotalCount);
