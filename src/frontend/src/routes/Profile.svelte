<script lang="ts">
  import { fetchUser, fetchRepos } from '../lib/api.js';
  import { currentUser, createUserState } from '../lib/state.svelte.js';
  import ProfileCard from '../components/ProfileCard.svelte';
  import SearchBar from '../components/SearchBar.svelte';
  import RepoList from '../components/RepoList.svelte';
  import Pagination from '../components/Pagination.svelte';
  import PageSizeSelector from '../components/PageSizeSelector.svelte';
  import SortSelector from '../components/SortSelector.svelte';
  import ErrorBanner from '../components/ErrorBanner.svelte';
  import ThemeToggle from '../components/ThemeToggle.svelte';

  interface Props {
    params?: { username?: string };
  }

  let { params = {} }: Props = $props();

  function resetUserState(username: string) {
    Object.assign(currentUser, createUserState(), { username, loading: true });
  }

  $effect(() => {
    const username = params.username;
    if (!username) return;

    resetUserState(username);
    loadProfile(username);
  });

  async function loadProfile(username: string) {
    const userResult = await fetchUser(username);
    if (!userResult.ok) {
      currentUser.error = userResult.error;
      currentUser.loading = false;
      return;
    }
    currentUser.profile = userResult.value;

    await loadRepos(1, 30);
  }

  async function loadRepos(page: number, perPage: number) {
    currentUser.loading = true;
    const result = await fetchRepos(currentUser.username, page, perPage, currentUser.sortBy);
    if (result.ok) {
      currentUser.repos = result.value.items;
      currentUser.totalCount = result.value.totalCount;
    } else {
      currentUser.error = result.error;
    }
    currentUser.loading = false;
  }

  function handlePageChange(newPage: number) {
    currentUser.page = newPage;
    loadRepos(newPage, currentUser.perPage);
  }

  function handlePageSizeChange(newSize: number) {
    currentUser.perPage = newSize;
    currentUser.page = 1;
    loadRepos(1, newSize);
  }

  function handleSortChange(newSort: typeof currentUser.sortBy) {
    currentUser.sortBy = newSort;
    currentUser.page = 1;
    loadRepos(1, currentUser.perPage);
  }

  function handleRetry() {
    currentUser.error = null;
    currentUser.loading = true;
    loadProfile(currentUser.username);
  }
</script>

<div class="min-h-screen bg-gh-page">
  <!-- Compact header with search -->
  <header class="sticky top-0 z-10 border-b border-gh-border bg-gh-page/80 backdrop-blur-sm">
    <div class="max-w-4xl mx-auto px-4 py-3 flex items-center gap-4">
      <a href="/" class="text-lg sm:text-xl font-bold text-gh-text hover:opacity-80 flex-shrink-0">
        GitHub Explorer
      </a>
      <div class="flex-1 max-w-none sm:max-w-md">
        <SearchBar value={currentUser.username} loading={currentUser.loading} />
      </div>
      <ThemeToggle />
    </div>
  </header>

  <main class="max-w-4xl mx-auto px-4 py-6 space-y-6">
    {#if currentUser.error}
      <ErrorBanner error={currentUser.error} onRetry={handleRetry} />
    {:else}
      <ProfileCard profile={currentUser.profile} loading={currentUser.loading} />

      {#if currentUser.profile}
        <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <h2 class="text-lg font-semibold text-gh-text">
            Repositories
          </h2>
          <div class="flex items-center gap-3">
            <SortSelector
              value={currentUser.sortBy}
              onChange={handleSortChange}
            />
            <PageSizeSelector
              value={currentUser.perPage}
              onChange={handlePageSizeChange}
            />
          </div>
        </div>

        <RepoList repos={currentUser.repos} loading={currentUser.loading} />

        {#if currentUser.totalCount > 0}
          <Pagination
            page={currentUser.page}
            perPage={currentUser.perPage}
            totalCount={currentUser.totalCount}
            onPageChange={handlePageChange}
          />
        {/if}
      {/if}
    {/if}
  </main>
</div>
