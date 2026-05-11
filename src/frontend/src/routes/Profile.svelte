<script lang="ts">
  import { fetchUser, fetchRepos } from '../lib/api.js';
  import { currentUser } from '../lib/state.svelte.js';
  import ProfileCard from '../components/ProfileCard.svelte';
  import SearchBar from '../components/SearchBar.svelte';

  interface Props {
    params?: { username?: string };
  }

  let { params = {} }: Props = $props();

  $effect(() => {
    const username = params.username;
    if (!username) return;

    currentUser.username = username;
    currentUser.loading = true;
    currentUser.error = null;
    currentUser.profile = null;
    currentUser.repos = [];
    currentUser.totalCount = 0;
    currentUser.page = 1;
    currentUser.perPage = 30;

    loadProfile(username);
  });

  async function loadProfile(username: string) {
    const userResult = await fetchUser(username);
    if (userResult.error) {
      currentUser.error = userResult.error;
      currentUser.loading = false;
      return;
    }
    currentUser.profile = userResult.data ?? null;

    const reposResult = await fetchRepos(username, 1, 30);
    if (reposResult.data) {
      currentUser.repos = reposResult.data.items;
      currentUser.totalCount = reposResult.data.totalCount;
    }
    currentUser.loading = false;
  }
</script>

<div class="min-h-screen bg-white dark:bg-gray-900">
  <!-- Compact header with search -->
  <header class="sticky top-0 z-10 border-b border-gray-200 dark:border-gray-700 bg-white/80 dark:bg-gray-900/80 backdrop-blur-sm">
    <div class="max-w-4xl mx-auto px-4 py-3 flex items-center gap-4">
      <a href="/" class="text-xl font-bold text-gray-900 dark:text-gray-100 hover:opacity-80">
        GitHub Explorer
      </a>
      <div class="flex-1 max-w-md">
        <SearchBar value={currentUser.username} loading={currentUser.loading} />
      </div>
    </div>
  </header>

  <main class="max-w-4xl mx-auto px-4 py-6 space-y-6">
    {#if currentUser.error}
      <div class="p-4 rounded-lg bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-300">
        <p class="font-medium">{currentUser.error.code}</p>
        <p class="text-sm">{currentUser.error.message}</p>
      </div>
    {:else}
      <ProfileCard profile={currentUser.profile} loading={currentUser.loading} />

      {#if !currentUser.loading && currentUser.profile}
        <!-- Repo list and pagination will go here in 03-04 -->
        <div class="text-gray-500 dark:text-gray-400 text-center py-8">
          Repository list coming in next step...
        </div>
      {/if}
    {/if}
  </main>
</div>
