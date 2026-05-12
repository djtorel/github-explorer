import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/svelte";
import Pagination from "../Pagination.svelte";

describe("Pagination", () => {
	it("renders current page and total", () => {
		render(Pagination, {
			props: { page: 2, perPage: 10, totalCount: 25, onPageChange: vi.fn() },
		});
		expect(screen.getByText("Page 2 of 3")).toBeInTheDocument();
	});

	it("disables Prev on first page", () => {
		render(Pagination, {
			props: { page: 1, perPage: 10, totalCount: 25, onPageChange: vi.fn() },
		});
		expect(screen.getByRole("button", { name: /prev/i })).toBeDisabled();
	});

	it("disables Next on last page", () => {
		render(Pagination, {
			props: { page: 3, perPage: 10, totalCount: 25, onPageChange: vi.fn() },
		});
		expect(screen.getByRole("button", { name: /next/i })).toBeDisabled();
	});

	it("calls onPageChange when Next clicked", async () => {
		const onPageChange = vi.fn();
		render(Pagination, {
			props: { page: 1, perPage: 10, totalCount: 25, onPageChange },
		});
		await fireEvent.click(screen.getByRole("button", { name: /next/i }));
		expect(onPageChange).toHaveBeenCalledWith(2);
	});

	it("calls onPageChange when Prev clicked", async () => {
		const onPageChange = vi.fn();
		render(Pagination, {
			props: { page: 2, perPage: 10, totalCount: 25, onPageChange },
		});
		await fireEvent.click(screen.getByRole("button", { name: /prev/i }));
		expect(onPageChange).toHaveBeenCalledWith(1);
	});

	it("calls onPageChange when page number clicked", async () => {
		const onPageChange = vi.fn();
		render(Pagination, {
			props: { page: 1, perPage: 10, totalCount: 50, onPageChange },
		});
		await fireEvent.click(screen.getByRole("button", { name: "3" }));
		expect(onPageChange).toHaveBeenCalledWith(3);
	});
});
