import { test, expect } from "@playwright/test";

test.describe("Error States", () => {
	test("shows user not found error", async ({ page }) => {
		await page.route(
			"**/api/users/this-user-definitely-does-not-exist-12345**",
			async (route) => {
				await route.fulfill({
					status: 404,
					json: {
						success: false,
						data: null,
						error: { code: "NotFound", message: "User not found." },
					},
				});
			},
		);

		await page.goto("/#/user/this-user-definitely-does-not-exist-12345");
		await expect(page.getByText(/user not found/i)).toBeVisible({
			timeout: 10000,
		});
		await expect(page.getByText(/try a different username/i)).toBeVisible();
	});

	test("shows 404 page for unknown routes", async ({ page }) => {
		await page.goto("/#/some-random-route");
		await expect(page.getByText("404")).toBeVisible();
		await expect(page.getByText(/page not found/i)).toBeVisible();
	});

	test("home page has working theme toggle", async ({ page }) => {
		await page.goto("/#/");

		const toggle = page.getByRole("button", { name: /switch to/i });
		await expect(toggle).toBeVisible();

		await toggle.click();

		const html = page.locator("html");
		await expect(html).toHaveClass(/dark/);

		await toggle.click();
		await expect(html).not.toHaveClass(/dark/);
	});
});
