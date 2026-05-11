export interface UserProfile {
  login: string;
  name: string | null;
  avatarUrl: string;
  bio: string | null;
  followers: number;
  publicRepos: number;
  htmlUrl: string;
}

export interface Repository {
  name: string;
  description: string | null;
  stargazersCount: number;
  forksCount: number;
  language: string | null;
  htmlUrl: string;
}

export interface PaginatedRepositories {
  items: Repository[];
  totalCount: number;
}

export interface ApiError {
  code: string;
  message: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: ApiError;
}
