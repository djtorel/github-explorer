<script lang="ts">
  import type { Repository } from '../lib/types.js';

  interface Props {
    repo: Repository;
  }

  let { repo }: Props = $props();
</script>

<a
  href={repo.htmlUrl}
  target="_blank"
  rel="noopener noreferrer"
  class="block p-4 rounded-lg border border-gray-200 dark:border-gray-700
         bg-white dark:bg-gray-800 hover:shadow-md hover:border-gray-300
         dark:hover:border-gray-600 transition-shadow"
>
  <div class="flex items-start justify-between gap-4">
    <div class="min-w-0 flex-1">
      <h3 class="text-lg font-semibold text-blue-600 dark:text-blue-400 truncate">
        {repo.name}
      </h3>
      {#if repo.description}
        <p class="text-gray-600 dark:text-gray-400 text-sm mt-1 line-clamp-2">
          {repo.description}
        </p>
      {/if}
    </div>
    <div class="flex items-center gap-3 text-sm text-gray-500 dark:text-gray-400 flex-shrink-0">
      <span class="flex items-center gap-1">⭐ {repo.stargazersCount.toLocaleString()}</span>
      <span class="flex items-center gap-1">🍴 {repo.forksCount.toLocaleString()}</span>
    </div>
  </div>
  {#if repo.language}
    <div class="mt-3">
      <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium
                   bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300">
        {repo.language}
      </span>
    </div>
  {/if}
</a>
