import { test, expect } from "@playwright/test";

const mockProfile = {
  success: true,
  data: {
    login: "octocat",
    name: "The Octocat",
    avatarUrl: "https://avatars.githubusercontent.com/u/1",
    bio: "GitHub mascot",
    followers: 1000,
    publicRepos: 50,
    htmlUrl: "https://github.com/octocat",
  },
  error: null,
};

const mockRepos = {
  success: true,
  data: {
    items: [
      { name: "repo1", description: "First repo", stargazersCount: 100, forksCount: 10, language: "C#", htmlUrl: "https://github.com/octocat/repo1" },
      { name: "repo2", description: "Second repo", stargazersCount: 50, forksCount: 5, language: "TypeScript", htmlUrl: "https://github.com/octocat/repo2" },
      { name: "repo3", description: "Third repo", stargazersCount: 25, forksCount: 2, language: "Python", htmlUrl: "https://github.com/octocat/repo3" },
    ],
    totalCount: 3,
  },
  error: null,
};

test.beforeEach(async ({ page }) => {
  await page.route("**/api/users/octocat", async (route) => {
    await route.fulfill({ json: mockProfile });
  });
  await page.route("**/api/users/octocat/repos**", async (route) => {
    await route.fulfill({ json: mockRepos });
  });
});

test.describe("Search Journey", () => {
  test("user can search for a GitHub profile and view repositories", async ({ page }) => {
    await page.goto("/#/");

    await expect(page.getByRole("heading", { name: /github explorer/i })).toBeVisible();

    await page.getByPlaceholder("Search GitHub user...").fill("octocat");
    await page.getByRole("button", { name: /search/i }).click();

    await expect(page.getByText("@octocat")).toBeVisible({ timeout: 10000 });
    await expect(page.getByText("The Octocat")).toBeVisible();
    await expect(page.getByText("GitHub mascot")).toBeVisible();
    await expect(page.getByText(/followers/)).toBeVisible();

    await expect(page.getByText("repo1")).toBeVisible();
    await expect(page.getByText("repo2")).toBeVisible();
    await expect(page.getByText("repo3")).toBeVisible();
  });

  test("direct navigation to user profile works", async ({ page }) => {
    await page.goto("/#/user/octocat");
    await expect(page.getByText("@octocat")).toBeVisible({ timeout: 10000 });
    await expect(page.getByText("The Octocat")).toBeVisible();
  });
});
