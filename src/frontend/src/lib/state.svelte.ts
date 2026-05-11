import type { UserProfile, Repository, ApiError } from "./types.js";

function getInitialTheme(): "light" | "dark" {
	const stored = localStorage.getItem("github-explorer-theme");
	if (stored === "light" || stored === "dark") return stored;
	return window.matchMedia("(prefers-color-scheme: dark)").matches
		? "dark"
		: "light";
}

export const theme = $state({ mode: getInitialTheme() });

export function setTheme(mode: "light" | "dark") {
	theme.mode = mode;
	document.documentElement.classList.toggle("dark", mode === "dark");
	localStorage.setItem("github-explorer-theme", mode);
}

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
