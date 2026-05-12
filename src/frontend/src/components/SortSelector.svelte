<script lang="ts">
  interface Props {
    value: "stars_desc" | "stars_asc" | "name_asc" | "name_desc";
    onChange: (value: "stars_desc" | "stars_asc" | "name_asc" | "name_desc") => void;
  }

  let { value, onChange }: Props = $props();

  const options = [
    { value: "stars_desc" as const, label: "★ Stars (high → low)" },
    { value: "stars_asc" as const, label: "★ Stars (low → high)" },
    { value: "name_asc" as const, label: "A → Z Name" },
    { value: "name_desc" as const, label: "Z → A Name" },
  ];
</script>

<div class="relative">
  <select
    value={value}
    onchange={(e) => onChange(e.currentTarget.value as typeof value)}
    aria-label="Sort repositories"
    class="appearance-none rounded-lg border border-gh-border bg-gh-card px-3 py-1.5 pr-8 text-sm text-gh-text focus:outline-none focus:ring-2 focus:ring-blue-500 cursor-pointer"
  >
    {#each options as opt}
      <option value={opt.value}>{opt.label}</option>
    {/each}
  </select>
  <svg
    class="pointer-events-none absolute right-2 top-1/2 h-4 w-4 -translate-y-1/2 text-gh-muted"
    fill="none"
    stroke="currentColor"
    viewBox="0 0 24 24"
  >
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
  </svg>
</div>
