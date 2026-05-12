import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/svelte";
import userEvent from "@testing-library/user-event";
import SortSelector from "../SortSelector.svelte";

describe("SortSelector", () => {
	const options = [
		{ value: "stars_desc" as const, label: "★ Stars (high → low)" },
		{ value: "stars_asc" as const, label: "★ Stars (low → high)" },
		{ value: "name_asc" as const, label: "A → Z Name" },
		{ value: "name_desc" as const, label: "Z → A Name" },
	];

	it("renders all sort options", () => {
		render(SortSelector, { props: { value: "stars_desc", onChange: vi.fn() } });

		const select = screen.getByLabelText("Sort repositories");
		expect(select).toBeInTheDocument();

		options.forEach((opt) => {
			expect(screen.getByText(opt.label)).toBeInTheDocument();
		});
	});

	it("reflects the current value", () => {
		render(SortSelector, { props: { value: "name_asc", onChange: vi.fn() } });

		const select = screen.getByLabelText(
			"Sort repositories",
		) as HTMLSelectElement;
		expect(select.value).toBe("name_asc");
	});

	it("calls onChange when selection changes", async () => {
		const handleChange = vi.fn();
		render(SortSelector, {
			props: { value: "stars_desc", onChange: handleChange },
		});

		const select = screen.getByLabelText("Sort repositories");
		await userEvent.selectOptions(select, "name_desc");

		expect(handleChange).toHaveBeenCalledTimes(1);
		expect(handleChange).toHaveBeenCalledWith("name_desc");
	});
});
