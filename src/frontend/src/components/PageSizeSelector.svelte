<script lang="ts">
  interface Props {
    value: number;
    onChange: (size: number) => void;
  }

  let { value, onChange }: Props = $props();

  const options = [10, 30, 50];
</script>

<div class="flex items-center gap-2 text-sm text-gh-muted">
  <span>Show</span>
  <select
    {value}
    onchange={(e) => onChange(Number((e.target as HTMLSelectElement).value))}
    class="px-2 py-1 rounded-md border border-gh-border
           bg-gh-card text-gh-text
           focus:outline-none focus:ring-2 focus:ring-blue-500"
  >
    {#each options as opt}
      <option value={opt}>{opt}</option>
    {/each}
  </select>
  <span>per page</span>
</div>
