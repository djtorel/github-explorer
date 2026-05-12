import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/svelte";
import ProfileCard from "../ProfileCard.svelte";
import type { UserProfile } from "../../lib/types";

const mockProfile: UserProfile = {
	login: "octocat",
	name: "The Octocat",
	bio: "GitHub mascot",
	avatarUrl: "https://avatars.githubusercontent.com/u/1",
	htmlUrl: "https://github.com/octocat",
	followers: 1000,
	publicRepos: 50,
};

describe("ProfileCard", () => {
	it("renders profile data", () => {
		render(ProfileCard, { props: { profile: mockProfile, loading: false } });
		expect(screen.getByText("The Octocat")).toBeInTheDocument();
		expect(screen.getByText("@octocat")).toBeInTheDocument();
		expect(screen.getByText("GitHub mascot")).toBeInTheDocument();
	});

	it("renders login as name when name is null", () => {
		render(ProfileCard, {
			props: { profile: { ...mockProfile, name: null }, loading: false },
		});
		expect(screen.getByText("octocat")).toBeInTheDocument();
	});

	it("shows skeleton when loading", () => {
		render(ProfileCard, { props: { profile: null, loading: true } });
		expect(screen.queryByText("The Octocat")).not.toBeInTheDocument();
		expect(document.querySelector(".animate-pulse")).toBeInTheDocument();
	});

	it("renders followers count", () => {
		render(ProfileCard, { props: { profile: mockProfile, loading: false } });
		expect(screen.getByText(/1,000 followers/)).toBeInTheDocument();
	});

	it("renders public repos count", () => {
		render(ProfileCard, { props: { profile: mockProfile, loading: false } });
		expect(screen.getByText(/50 public repos/)).toBeInTheDocument();
	});

	it("links to GitHub profile", () => {
		render(ProfileCard, { props: { profile: mockProfile, loading: false } });
		const link = screen.getByRole("link", { name: /view on github/i });
		expect(link).toHaveAttribute("href", "https://github.com/octocat");
		expect(link).toHaveAttribute("target", "_blank");
	});
});
