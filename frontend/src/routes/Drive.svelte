<script>
  import { Breadcrumb, BreadcrumbItem, Card } from "flowbite-svelte";
  import { onMount } from "svelte";
  import { FileOutline, FolderOutline } from "flowbite-svelte-icons";

  let fileList = [];
  let children = [];

  // Fetch data from the endpoint
  async function fetchData() {
    const response = await fetch("/drive/");
    const data = await response.json();
    fileList = data;
    children = fileList.children;
    console.log(children);
  }

  // Call fetchData when the component is mounted
  onMount(() => {
    fetchData();
  });
</script>

<Breadcrumb aria-label="Default breadcrumb example">
  <BreadcrumbItem href="/" home>Home</BreadcrumbItem>
  <BreadcrumbItem href="/">Projects</BreadcrumbItem>
  <BreadcrumbItem>Flowbite Svelte</BreadcrumbItem>
</Breadcrumb>

<div class="flex p-4">
  {#each children as item (item.name)}
    <Card class="items-center">
      {#if item.isFolder}
        <FolderOutline size="xl" />
      {:else}
        <FileOutline size="xl" />
      {/if}
      <p class="text-md dark:text-white">{item.name}</p>
    </Card>
  {/each}
</div>
