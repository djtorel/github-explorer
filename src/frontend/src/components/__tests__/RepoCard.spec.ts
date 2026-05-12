import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/svelte";
import RepoCard from "../RepoCard.svelte";
import type { Repository } from "../../lib/types";

const mockRepo: Repository = {
  name: "hello-world",
  description: "A test repository",
  language: "TypeScript",
  stargazersCount: 42,
  forksCount: 7,
  htmlUrl: "https://github.com/octocat/hello-world",
};

describe("RepoCard", () => {
  it("renders repo name and description", () => {
    render(RepoCard, { props: { repo: mockRepo } });
    expect(screen.getByText("hello-world")).toBeInTheDocument();
    expect(screen.getByText("A test repository")).toBeInTheDocument();
  });

  it("renders star and fork counts", () => {
    render(RepoCard, { props: { repo: mockRepo } });
    expect(screen.getByText(/42/)).toBeInTheDocument();
    expect(screen.getByText(/7/)).toBeInTheDocument();
  });

  it("renders language badge", () => {
    render(RepoCard, { props: { repo: mockRepo } });
    expect(screen.getByText("TypeScript")).toBeInTheDocument();
  });

  it("links to GitHub", () => {
    render(RepoCard, { props: { repo: mockRepo } });
    const link = screen.getByRole("link");
    expect(link).toHaveAttribute(
      "href",
      "https://github.com/octocat/hello-world",
    );
    expect(link).toHaveAttribute("target", "_blank");
  });

  it("renders without description", () => {
    const repoWithoutDesc = { ...mockRepo, description: null };
    render(RepoCard, { props: { repo: repoWithoutDesc } });
    expect(screen.getByText("hello-world")).toBeInTheDocument();
    expect(screen.queryByText("A test repository")).not.toBeInTheDocument();
  });
});
