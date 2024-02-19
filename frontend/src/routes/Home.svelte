<script>
  import { Spinner } from "flowbite-svelte";
  import { onMount } from "svelte";
  import Gallery from "svelte-image-gallery";

  function handleClick(e) {
    console.log(e.detail.src);
  }
  let images = [];
  let images1 = [];
  let images2 = [];
  let images3 = [];
  let images4 = [];
  let isLoading = false;

  onMount(async () => {
    try {
      isLoading = true;
      const response = await fetch("/home");
      const data = await response.json();
      console.log(data);
      images = data.map((item) => ({ src: String(item) }));
    } catch (error) {
      console.error("Error fetching data:", error);
    }
    for (let i = 0; i < images.length; i++) {
      if (i % 4 === 0) {
        images1.push(images[i]);
      } else if (i % 4 === 1) {
        images2.push(images[i]);
      } else if (i % 4 === 2) {
        images3.push(images[i]);
      } else {
        images4.push(images[i]);
      }
    }
    isLoading = false;
  });
</script>

{#if isLoading}
  <Spinner />
{/if}

<Gallery loading="eager" on:click={handleClick}>
  {#each images as img}
    <img src={img["src"]} alt="pic" />
  {/each}
</Gallery>

<style>
  img {
    border-radius: 1rem;
  }
</style>
