<script>
  import { Spinner } from "flowbite-svelte";
  import { onMount } from "svelte";
  import Gallery from "svelte-image-gallery";

  function handleClick(e) {
    console.log(e.detail.src);
  }
  let images = [];
  let isLoading = false;
  let fetchFurther = false;
  let results = [];

  const fetchPhotos = async () => {
    try {
      isLoading = true;
      const response = await fetch("/home");
      const data = await response.json();
      results = data.map((item) => ({ src: String(item) }));
      images = images.concat(results);
    } catch (error) {
      console.error("Error fetching data:", error);
    }
    isLoading = false;
    fetchFurther = true;
  };

  onMount(fetchPhotos);

  window.onscroll = () => {
    if (
      window.innerHeight + document.documentElement.scrollTop >=
        document.documentElement.offsetHeight &&
      fetchFurther
    ) {
      console.log("here");
      fetchFurther = false;
      fetchPhotos();
    }
  };
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
