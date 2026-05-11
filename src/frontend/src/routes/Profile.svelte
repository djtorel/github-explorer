<script lang="ts">
  export let params: { username?: string } = {};
</script>

<div class="max-w-4xl mx-auto p-4">
  <h1 class="text-2xl font-bold">Profile: {params.username ?? 'unknown'}</h1>
</div>
