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
  class="block p-4 rounded-lg border border-gh-border
         bg-gh-card hover:shadow-md transition-shadow"
>
  <div class="flex items-start justify-between gap-4">
    <div class="min-w-0 flex-1">
      <h3 class="text-lg font-semibold text-gh-link truncate">
        {repo.name}
      </h3>
      {#if repo.description}
        <p class="text-gh-muted text-sm mt-1 line-clamp-2">
          {repo.description}
        </p>
      {/if}
    </div>
    <div class="flex items-center gap-3 text-sm text-gh-muted flex-shrink-0">
      <span class="flex items-center gap-1">⭐ {repo.stargazersCount.toLocaleString()}</span>
      <span class="flex items-center gap-1">🍴 {repo.forksCount.toLocaleString()}</span>
    </div>
  </div>
  {#if repo.language}
    <div class="mt-3">
      <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium
                   bg-gh-badge text-gh-text">
        {repo.language}
      </span>
    </div>
  {/if}
</a>
