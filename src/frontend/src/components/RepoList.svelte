<script lang="ts">
  import type { Repository } from '../lib/types.js';
  import RepoCard from './RepoCard.svelte';
  import Skeleton from './Skeleton.svelte';

  interface Props {
    repos: Repository[];
    loading: boolean;
  }

  let { repos, loading }: Props = $props();
</script>

<div class="space-y-3">
  {#if loading}
    <Skeleton variant="repo" count={5} />
  {:else if repos.length === 0}
    <div class="text-center py-12 text-gh-muted">
      <p class="text-lg font-medium">No public repositories</p>
      <p class="text-sm mt-1">This user hasn't published any repositories yet.</p>
    </div>
  {:else}
    {#each repos as repo (repo.name)}
      <RepoCard {repo} />
    {/each}
  {/if}
</div>
