<script>
  import { Breadcrumb, BreadcrumbItem, Button, Card } from "flowbite-svelte";
  import { onMount } from "svelte";
  import { FileOutline, FolderOutline } from "flowbite-svelte-icons";

  let fileList = [];
  let children = [];
  let path = "";
  let segments = [];
  $: segments = path.split("/").filter(Boolean);

  async function fetchData(folder) {
    path += folder;
    console.log(path);
    const response = await fetch("/drive" + path);
    const data = await response.json();
    fileList = data;
    children = fileList.children;
  }

  onMount(() => {
    fetchData(path);
  });

  function handleBackClick(index) {
    if (index < 0) {
      path = "";
    } else {
      const newPath = segments.slice(0, index + 1).join("/");
      path = "/" + newPath;
    }
    fetchData("");
  }
</script>

<Breadcrumb aria-label="Default breadcrumb">
  <BreadcrumbItem home
    ><Button
      color="alternative"
      class="bg-transparent !p-1 border-transparent"
      on:click={() => handleBackClick(-1)}
    >
      root
    </Button></BreadcrumbItem
  >
  {#each segments as segment, index (segment)}
    <BreadcrumbItem
      ><Button
        color="alternative"
        class="bg-transparent !p-1 border-transparent"
        on:click={() => handleBackClick(index)}
      >
        {segment}
      </Button>
    </BreadcrumbItem>
  {/each}
</Breadcrumb>

<div class="flex p-4">
  {#each children as item (item.name)}
    <Card on:click={() => fetchData("/" + item.name)} class="items-center">
      {#if item.isFolder}
        <FolderOutline size="xl" />
      {:else}
        <FileOutline size="xl" />
      {/if}
      <p class="text-md dark:text-white">{item.name}</p>
    </Card>
  {/each}
</div>
