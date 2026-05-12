import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/svelte";
import ThemeToggle from "../ThemeToggle.svelte";
import { theme, setTheme } from "../../lib/state.svelte";

describe("ThemeToggle", () => {
	beforeEach(() => {
		setTheme("light");
	});

	it("renders toggle button", () => {
		render(ThemeToggle);
		expect(screen.getByRole("button")).toBeInTheDocument();
	});

	it("has aria-label for accessibility", () => {
		render(ThemeToggle);
		expect(screen.getByRole("button")).toHaveAttribute("aria-label");
	});

	it("toggles theme when clicked", async () => {
		render(ThemeToggle);
		const button = screen.getByRole("button");
		expect(theme.mode).toBe("light");
		await fireEvent.click(button);
		expect(theme.mode).toBe("dark");
		await fireEvent.click(button);
		expect(theme.mode).toBe("light");
	});
});
