<script lang="ts">
  interface Props {
    value: number;
    onChange: (size: number) => void;
  }

  let { value, onChange }: Props = $props();

  const options = [10, 30, 50];
</script>

<div class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
  <span>Show</span>
  <select
    {value}
    onchange={(e) => onChange(Number((e.target as HTMLSelectElement).value))}
    class="px-2 py-1 rounded-md border border-gray-200 dark:border-gray-700
           bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100
           focus:outline-none focus:ring-2 focus:ring-blue-500"
  >
    {#each options as opt}
      <option value={opt}>{opt}</option>
    {/each}
  </select>
  <span>per page</span>
</div>
