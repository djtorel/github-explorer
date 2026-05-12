import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/svelte";
import SearchBar from "../SearchBar.svelte";

vi.mock("svelte-spa-router", () => ({
	push: vi.fn(),
}));

import { push } from "svelte-spa-router";

describe("SearchBar", () => {
	it("renders input and button", () => {
		render(SearchBar, { props: { value: "", loading: false } });
		expect(
			screen.getByPlaceholderText("Search GitHub user..."),
		).toBeInTheDocument();
		expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
	});

	it("button is disabled when input is empty", () => {
		render(SearchBar, { props: { value: "", loading: false } });
		expect(screen.getByRole("button")).toBeDisabled();
	});

	it("button is disabled when loading", () => {
		render(SearchBar, { props: { value: "octocat", loading: true } });
		expect(screen.getByRole("button")).toBeDisabled();
	});

	it("pre-fills input with value prop", () => {
		render(SearchBar, { props: { value: "octocat", loading: false } });
		expect(screen.getByDisplayValue("octocat")).toBeInTheDocument();
	});

	it("calls push on submit with trimmed username", async () => {
		vi.mocked(push).mockClear();
		render(SearchBar, { props: { value: "", loading: false } });
		const input = screen.getByPlaceholderText("Search GitHub user...");
		await fireEvent.input(input, { target: { value: "  octocat  " } });
		await fireEvent.submit(input.closest("form")!);
		expect(push).toHaveBeenCalledWith("/user/octocat");
	});
});
