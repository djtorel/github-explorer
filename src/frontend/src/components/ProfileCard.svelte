<script lang="ts">
  import type { UserProfile } from '../lib/types.js';
  import Skeleton from './Skeleton.svelte';

  interface Props {
    profile: UserProfile | null;
    loading: boolean;
  }

  let { profile, loading }: Props = $props();
</script>

{#if loading}
  <div class="flex flex-col sm:flex-row items-center sm:items-start gap-4 sm:gap-6 p-6 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
    <Skeleton variant="avatar" />
    <div class="flex-1 w-full space-y-3 text-center sm:text-left">
      <Skeleton variant="text" />
      <Skeleton variant="text" count={2} />
    </div>
  </div>
{:else if profile}
  <div class="flex flex-col sm:flex-row items-center sm:items-start gap-4 sm:gap-6 p-6 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
    <img
      src={profile.avatarUrl}
      alt={`${profile.login}'s avatar`}
      class="w-20 h-20 sm:w-24 sm:h-24 rounded-full object-cover flex-shrink-0"
    />
    <div class="flex-1 min-w-0 text-center sm:text-left">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">
        {profile.name || profile.login}
      </h2>
      <p class="text-gray-500 dark:text-gray-400 mb-2">
        @{profile.login}
      </p>
      {#if profile.bio}
        <p class="text-gray-700 dark:text-gray-300 mb-3 line-clamp-3">{profile.bio}</p>
      {/if}
      <div class="flex flex-wrap justify-center sm:justify-start gap-4 text-sm text-gray-600 dark:text-gray-400">
        <span>⭐ {profile.followers.toLocaleString()} followers</span>
        <span>📦 {profile.publicRepos} public repos</span>
        <a
          href={profile.htmlUrl}
          target="_blank"
          rel="noopener noreferrer"
          class="text-blue-600 dark:text-blue-400 hover:underline"
        >
          View on GitHub →
        </a>
      </div>
    </div>
  </div>
{/if}
