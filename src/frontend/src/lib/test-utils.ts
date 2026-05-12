import { render } from "@testing-library/svelte";
import type { Component } from "svelte";

export function renderComponent<T extends Record<string, unknown>>(
  Component: Component<T>,
  props: T,
) {
  return render(Component, { props } as any);
}
