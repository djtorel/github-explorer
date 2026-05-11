import type {
  UserProfile,
  PaginatedRepositories,
  ApiError,
  ApiResponse,
} from "./types.js";

const API_BASE = "/api";

export async function fetchUser(
  username: string,
): Promise<{ data?: UserProfile; error?: ApiError }> {
  try {
    const res = await fetch(
      `${API_BASE}/users/${encodeURIComponent(username)}`,
    );
    const json: ApiResponse<UserProfile> = await res.json();
    if (!json.success) return { error: json.error };
    return { data: json.data };
  } catch {
    return {
      error: { code: "NetworkError", message: "Unable to reach the server." },
    };
  }
}

export async function fetchRepos(
  username: string,
  page: number,
  perPage: number,
): Promise<{ data?: PaginatedRepositories; error?: ApiError }> {
  try {
    const params = new URLSearchParams({
      page: String(page),
      perPage: String(perPage),
    });
    const res = await fetch(
      `${API_BASE}/users/${encodeURIComponent(username)}/repos?${params}`,
    );
    const json: ApiResponse<PaginatedRepositories> = await res.json();
    if (!json.success) return { error: json.error };
    return { data: json.data };
  } catch {
    return {
      error: { code: "NetworkError", message: "Unable to reach the server." },
    };
  }
}
