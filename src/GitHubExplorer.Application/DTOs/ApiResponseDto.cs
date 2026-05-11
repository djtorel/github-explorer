namespace GitHubExplorer.Application.DTOs;

public sealed record ApiResponseDto<T>(bool Success, T? Data, ApiErrorDto? Error);

public sealed record ApiErrorDto(string Code, string Message);
