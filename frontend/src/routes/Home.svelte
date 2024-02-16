<script>
    import {
    Input,
    Button,
    GradientButton,
    Alert
  } from "flowbite-svelte";
  import { ChevronRightOutline, FolderOutline, HomeOutline } from "flowbite-svelte-icons";
    const { ipcRenderer } = require("electron");
let inputPath = "";
  let invalidPath = false;
  function openFolderDialog() {
    ipcRenderer.send("open-folder");
  }

  function handleFolderSelected(event, selectedFolderPath) {
    inputPath = selectedFolderPath[0];
  }
  ipcRenderer.on("folder-selected", handleFolderSelected);

  const handleSubmit = async () => {
    const response = await fetch("/upload", {
      method: "POST",
      headers: {
        "Content-Type": "application/json", // Specify the content type
      },
      body: JSON.stringify(inputPath), // Data to be sent in the request body
    });
    if(response.ok) {
        invalidPath = false;
    }
    else {
      invalidPath = true;
    }
    const data = await response.json();
  };
</script>

<div class="flex">
  <Input id="folder-input" placeholder="Enter folder path" bind:value={inputPath} />
  <Button
    color="alternative"
    class="bg-transparent !p-1 border-transparent"
    on:click={openFolderDialog}
  >
    <FolderOutline class="w-5 h-5" />
  </Button>
</div>
<GradientButton on:click={handleSubmit} color="cyanToBlue">
  <ChevronRightOutline class="w-3.5 h-3.5 me-2" /> Upload
</GradientButton>

{#if invalidPath}
<Alert>
  <span class="font-medium">Invalid path</span>
</Alert>
{/if}
