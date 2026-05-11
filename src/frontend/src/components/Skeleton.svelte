<script lang="ts">
  interface Props {
    variant: 'avatar' | 'card' | 'list' | 'text' | 'profile' | 'repo';
    count?: number;
  }

  let { variant, count = 1 }: Props = $props();
</script>

{#each Array(count) as _}
  {#if variant === 'avatar'}
    <div class="w-20 h-20 sm:w-24 sm:h-24 rounded-full bg-gray-200 dark:bg-gray-700 animate-pulse"></div>
  {:else if variant === 'card'}
    <div class="w-full h-32 rounded-lg bg-gray-200 dark:bg-gray-700 animate-pulse mb-4"></div>
  {:else if variant === 'list'}
    <div class="w-full h-20 rounded-lg bg-gray-200 dark:bg-gray-700 animate-pulse mb-3"></div>
  {:else if variant === 'text'}
    <div class="w-3/4 h-4 rounded bg-gray-200 dark:bg-gray-700 animate-pulse mb-2"></div>
  {:else if variant === 'profile'}
    <div class="flex flex-col sm:flex-row items-center sm:items-start gap-4 sm:gap-6 w-full">
      <div class="w-20 h-20 sm:w-24 sm:h-24 rounded-full bg-gray-200 dark:bg-gray-700 animate-pulse flex-shrink-0"></div>
      <div class="flex-1 w-full space-y-2 text-center sm:text-left">
        <div class="w-1/2 h-6 rounded bg-gray-200 dark:bg-gray-700 animate-pulse mx-auto sm:mx-0"></div>
        <div class="w-3/4 h-4 rounded bg-gray-200 dark:bg-gray-700 animate-pulse mx-auto sm:mx-0"></div>
        <div class="w-1/3 h-4 rounded bg-gray-200 dark:bg-gray-700 animate-pulse mx-auto sm:mx-0"></div>
      </div>
    </div>
  {:else if variant === 'repo'}
    <div class="w-full p-4 rounded-lg border border-gray-200 dark:border-gray-700 bg-gray-200 dark:bg-gray-700 animate-pulse mb-3">
      <div class="w-1/3 h-5 rounded bg-gray-300 dark:bg-gray-600 mb-2"></div>
      <div class="w-2/3 h-4 rounded bg-gray-300 dark:bg-gray-600 mb-2"></div>
      <div class="w-16 h-4 rounded bg-gray-300 dark:bg-gray-600"></div>
    </div>
  {/if}
{/each}
