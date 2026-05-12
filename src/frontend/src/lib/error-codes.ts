export const ErrorCodes = {
	NotFound: "NotFound",
	RateLimited: "RateLimited",
	NetworkError: "NetworkError",
	EmptyResult: "EmptyResult",
	Unknown: "Unknown",
} as const;

export type ErrorCode = (typeof ErrorCodes)[keyof typeof ErrorCodes];

export const ErrorMessages = {
	[ErrorCodes.NotFound]: "User not found.",
	[ErrorCodes.RateLimited]: "GitHub API rate limit exceeded.",
	[ErrorCodes.NetworkError]: "Unable to reach the server.",
	[ErrorCodes.Unknown]: "An unexpected error occurred.",
} as const;
