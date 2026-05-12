import type {
	UserProfile,
	PaginatedRepositories,
	ApiError,
	SortBy,
} from "./types.js";
import { ok, err, type Result } from "./result.js";
import { ErrorCodes } from "./error-codes.js";

const API_BASE = "/api";

export async function fetchUser(
	username: string,
): Promise<Result<UserProfile>> {
	try {
		const res = await fetch(
			`${API_BASE}/users/${encodeURIComponent(username)}`,
		);
		const json = await res.json();
		if (!json.success) return err(json.error as ApiError);
		return ok(json.data as UserProfile);
	} catch {
		return err({
			code: ErrorCodes.NetworkError,
			message: "Unable to reach the server.",
		});
	}
}

export async function fetchRepos(
	username: string,
	page: number,
	perPage: number,
	sortBy: SortBy = "stars_desc",
): Promise<Result<PaginatedRepositories>> {
	try {
		const params = new URLSearchParams({
			page: String(page),
			perPage: String(perPage),
			sortBy,
		});
		const res = await fetch(
			`${API_BASE}/users/${encodeURIComponent(username)}/repos?${params}`,
		);
		const json = await res.json();
		if (!json.success) return err(json.error as ApiError);
		return ok(json.data as PaginatedRepositories);
	} catch {
		return err({
			code: ErrorCodes.NetworkError,
			message: "Unable to reach the server.",
		});
	}
}
