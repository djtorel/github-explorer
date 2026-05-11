<script lang="ts">
  import type { ApiError } from '../lib/types.js';

  interface Props {
    error: ApiError;
    onRetry?: () => void;
  }

  let { error, onRetry }: Props = $props();

  const config = $derived.by(() => {
    switch (error.code) {
      case 'NotFound':
        return {
          icon: QuestionMarkIcon,
          title: 'User not found',
          message: 'Try a different username',
          style: 'border-orange-200 dark:border-orange-800 bg-orange-50 dark:bg-orange-900/20 text-orange-700 dark:text-orange-300',
        };
      case 'RateLimited':
        return {
          icon: ClockIcon,
          title: 'Rate limit exceeded',
          message: 'Try again in a few minutes',
          style: 'border-yellow-200 dark:border-yellow-800 bg-yellow-50 dark:bg-yellow-900/20 text-yellow-700 dark:text-yellow-300',
        };
      case 'NetworkError':
        return {
          icon: WifiOffIcon,
          title: 'Connection error',
          message: error.message || 'Unable to reach the server',
          style: 'border-red-200 dark:border-red-800 bg-red-50 dark:bg-red-900/20 text-red-700 dark:text-red-300',
        };
      default:
        return {
          icon: AlertIcon,
          title: 'Something went wrong',
          message: error.message || 'Please try again',
          style: 'border-gh-border bg-gh-card/50 text-gh-muted',
        };
    }
  });

  const showRetry = $derived(error.code === 'NetworkError' && onRetry);
</script>

{#snippet QuestionMarkIcon()}
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <circle cx="12" cy="12" r="10"/>
    <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"/>
    <path d="M12 17h.01"/>
  </svg>
{/snippet}

{#snippet ClockIcon()}
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <circle cx="12" cy="12" r="10"/>
    <polyline points="12 6 12 12 16 14"/>
  </svg>
{/snippet}

{#snippet WifiOffIcon()}
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <line x1="2" y1="2" x2="22" y2="22"/>
    <path d="M8.5 16.5a7 7 0 0 1 10 0"/>
    <path d="M2 12a15.3 15.3 0 0 1 4-3"/>
    <path d="M22 12a15.3 15.3 0 0 0-4-3"/>
    <path d="M5 8a11.7 11.7 0 0 1 14 0"/>
  </svg>
{/snippet}

{#snippet AlertIcon()}
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <circle cx="12" cy="12" r="10"/>
    <line x1="12" y1="8" x2="12" y2="12"/>
    <line x1="12" y1="16" x2="12.01" y2="16"/>
  </svg>
{/snippet}

<div class="p-4 rounded-lg border flex items-start gap-3 {config.style}">
  <div class="flex-shrink-0 mt-0.5">
    {@render config.icon()}
  </div>
  <div class="flex-1 min-w-0">
    <p class="font-semibold">{config.title}</p>
    <p class="text-sm opacity-90">{config.message}</p>
    {#if showRetry}
      <button
        onclick={onRetry}
        class="mt-3 px-4 py-2 rounded-md text-sm font-medium
               bg-red-600 text-white hover:bg-red-700
               dark:bg-red-500 dark:hover:bg-red-600
               transition-colors"
      >
        Retry
      </button>
    {/if}
  </div>
</div>
