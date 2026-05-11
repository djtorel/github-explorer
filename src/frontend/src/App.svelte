<script lang="ts">
  import Router from 'svelte-spa-router';
  import Home from './routes/Home.svelte';
  import Profile from './routes/Profile.svelte';
  import NotFound from './routes/NotFound.svelte';

  const routes = {
    '/': Home,
    '/user/:username': Profile,
    '*': NotFound,
  };
</script>

<main class="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100">
  <Router {routes} />
</main>
