<script>
  import { Spinner } from "flowbite-svelte";
  import { onMount } from "svelte";
  import Gallery from "svelte-image-gallery";

  function handleClick(e) {
    console.log(e.detail.src);
  }
  let images = [];
  let fetchFurther = false;
  let results = [];
  let offset = 0;

  const fetchPhotos = async () => {
    try {
      const response = await fetch(`/home&${offset}`);
      const data = await response.json();
      results = data.map((item) => ({ src: String(item) }));
      images = images.concat(results);
      offset += 10;
    } catch (error) {
      console.error("Error fetching data:", error);
    }
    fetchFurther = true;
  };

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

    onMount(fetchPhotos);
  };
</script>

{#await fetchPhotos()}
  <Spinner />
{:then data}
  <Gallery loading="eager" on:click={handleClick}>
    {#each images as img}
      <img src={img["src"]} alt="pic" />
    {/each}
  </Gallery>
{:catch error}
  <p>Unable to fetch photos</p>
{/await}

<style>
  img {
    border-radius: 1rem;
  }
</style>
