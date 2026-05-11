import type { UserProfile, Repository, ApiError } from "./types.js";

export const theme = $state({ mode: "dark" as "light" | "dark" });

export const currentUser = $state({
  username: "",
  profile: null as UserProfile | null,
  repos: [] as Repository[],
  totalCount: 0,
  page: 1,
  perPage: 30,
  loading: false,
  error: null as ApiError | null,
});
