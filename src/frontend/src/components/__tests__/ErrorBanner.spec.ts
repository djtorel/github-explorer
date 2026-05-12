import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/svelte";
import ErrorBanner from "../ErrorBanner.svelte";
import type { ApiError } from "../../lib/types";

describe("ErrorBanner", () => {
	it("renders NotFound error", () => {
		const error: ApiError = { code: "NotFound", message: "User not found" };
		render(ErrorBanner, { props: { error, onRetry: undefined } });
		expect(screen.getByText("User not found")).toBeInTheDocument();
		expect(screen.getByText("Try a different username")).toBeInTheDocument();
	});

	it("renders RateLimited error", () => {
		const error: ApiError = {
			code: "RateLimited",
			message: "Too many requests",
		};
		render(ErrorBanner, { props: { error } });
		expect(screen.getByText("Rate limit exceeded")).toBeInTheDocument();
	});

	it("shows retry button for NetworkError", () => {
		const error: ApiError = {
			code: "NetworkError",
			message: "Connection failed",
		};
		const onRetry = vi.fn();
		render(ErrorBanner, { props: { error, onRetry } });
		expect(screen.getByRole("button", { name: /retry/i })).toBeInTheDocument();
	});

	it("does not show retry button for NetworkError without onRetry", () => {
		const error: ApiError = {
			code: "NetworkError",
			message: "Connection failed",
		};
		render(ErrorBanner, { props: { error } });
		expect(
			screen.queryByRole("button", { name: /retry/i }),
		).not.toBeInTheDocument();
	});

	it("calls onRetry when retry button clicked", async () => {
		const error: ApiError = {
			code: "NetworkError",
			message: "Connection failed",
		};
		const onRetry = vi.fn();
		render(ErrorBanner, { props: { error, onRetry } });
		await fireEvent.click(screen.getByRole("button", { name: /retry/i }));
		expect(onRetry).toHaveBeenCalledOnce();
	});

	it("renders generic error for unknown code", () => {
		const error: ApiError = { code: "Unknown", message: "Something broke" };
		render(ErrorBanner, { props: { error } });
		expect(screen.getByText("Something went wrong")).toBeInTheDocument();
	});
});
