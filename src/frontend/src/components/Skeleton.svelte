<script lang="ts">
  interface Props {
    variant: 'avatar' | 'card' | 'list' | 'text';
    count?: number;
  }

  let { variant, count = 1 }: Props = $props();
</script>

{#each Array(count) as _}
  {#if variant === 'avatar'}
    <div class="w-24 h-24 rounded-full bg-gray-200 dark:bg-gray-700 animate-pulse"></div>
  {:else if variant === 'card'}
    <div class="w-full h-32 rounded-lg bg-gray-200 dark:bg-gray-700 animate-pulse mb-4"></div>
  {:else if variant === 'list'}
    <div class="w-full h-20 rounded-lg bg-gray-200 dark:bg-gray-700 animate-pulse mb-3"></div>
  {:else if variant === 'text'}
    <div class="w-3/4 h-4 rounded bg-gray-200 dark:bg-gray-700 animate-pulse mb-2"></div>
  {/if}
{/each}
