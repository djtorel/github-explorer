<script lang="ts">
  import { push } from 'svelte-spa-router';

  interface Props {
    value?: string;
    loading?: boolean;
  }

  let { value = '', loading = false }: Props = $props();
  let inputValue = $state('');

  $effect(() => {
    inputValue = value;
  });

  $effect(() => {
    inputValue = value;
  });

  function handleSubmit(e: Event) {
    e.preventDefault();
    const username = inputValue.trim();
    if (username) {
      push(`/user/${encodeURIComponent(username)}`);
    }
  }
</script>

<form onsubmit={handleSubmit} class="w-full max-w-lg">
  <div class="flex gap-2">
    <input
      type="text"
      bind:value={inputValue}
      placeholder="Search GitHub user..."
      disabled={loading}
      class="flex-1 px-4 py-3 rounded-lg border border-gray-200 dark:border-gray-700
             bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100
             focus:outline-none focus:ring-2 focus:ring-blue-500
             disabled:opacity-50"
    />
    <button
      type="submit"
      disabled={loading || !inputValue.trim()}
      class="px-6 py-3 rounded-lg bg-blue-600 text-white font-medium
             hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
    >
      {#if loading}
        <span class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
      {:else}
        Search
      {/if}
    </button>
  </div>
</form>
