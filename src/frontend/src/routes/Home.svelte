<script lang="ts">
  import SearchBar from '../components/SearchBar.svelte';
  import ThemeToggle from '../components/ThemeToggle.svelte';
</script>

<div class="flex flex-col items-center justify-center min-h-screen px-4 relative">
  <div class="absolute top-4 right-4">
    <ThemeToggle />
  </div>
  <h1 class="text-4xl font-bold text-gh-text mb-8">
    GitHub Explorer
  </h1>
  <SearchBar />
  <p class="mt-4 text-sm text-gh-muted">
    Enter a GitHub username to explore their profile and repositories.
  </p>
</div>
