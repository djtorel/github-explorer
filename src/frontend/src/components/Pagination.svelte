<script lang="ts">
  interface Props {
    page: number;
    perPage: number;
    totalCount: number;
    onPageChange: (page: number) => void;
  }

  let { page, perPage, totalCount, onPageChange }: Props = $props();

  const totalPages = $derived(Math.max(1, Math.ceil(totalCount / perPage)));
  const canGoPrev = $derived(page > 1);
  const canGoNext = $derived(page < totalPages);

  function goToPage(p: number) {
    if (p >= 1 && p <= totalPages && p !== page) {
      onPageChange(p);
    }
  }

  function pageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;
    let start = Math.max(1, page - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);

    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }
</script>

<div class="flex items-center justify-between">
  <div class="text-sm text-gray-500 dark:text-gray-400">
    Page {page} of {totalPages}
  </div>
  <div class="flex items-center gap-1">
    <button
      onclick={() => goToPage(page - 1)}
      disabled={!canGoPrev}
      class="px-3 py-1.5 rounded-md text-sm font-medium
             bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700
             text-gray-700 dark:text-gray-300
             hover:bg-gray-50 dark:hover:bg-gray-700
             disabled:opacity-40 disabled:cursor-not-allowed"
    >
      ← Prev
    </button>

    {#each pageNumbers() as p}
      <button
        onclick={() => goToPage(p)}
        class="px-3 py-1.5 rounded-md text-sm font-medium min-w-[2.5rem]
               {p === page
                 ? 'bg-blue-600 text-white border border-blue-600'
                 : 'bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700'}"
      >
        {p}
      </button>
    {/each}

    <button
      onclick={() => goToPage(page + 1)}
      disabled={!canGoNext}
      class="px-3 py-1.5 rounded-md text-sm font-medium
             bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700
             text-gray-700 dark:text-gray-300
             hover:bg-gray-50 dark:hover:bg-gray-700
             disabled:opacity-40 disabled:cursor-not-allowed"
    >
      Next →
    </button>
  </div>
</div>
