<script>
  import Router from "svelte-spa-router";
  import Home from "./routes/Home.svelte";
  import Login from "./routes/Login.svelte";
  import Upload from "./routes/Upload.svelte";
  import Drive from "./routes/Drive.svelte";
  import {
    Navbar,
    NavBrand,
    NavHamburger,
    NavUl,
    NavLi,
  } from "flowbite-svelte";

  let routes = {
    "/": Home,
    "/drive": Drive,
    "/upload": Upload,
    "/login": Login,
    "*": Login,
  };
</script>

<Navbar class="px-2 sm:px-4 py-2.5 fixed w-full z-20 top-0 start-0 border-b">
  <NavBrand href="/">
    <img src="/nimbus-logo.png" class="me-3 h-6 sm:h-9" alt="Nimbus" />
  </NavBrand>
  <NavHamburger />
  <NavUl>
    <NavLi href="/">Home</NavLi>
    <NavLi href="/#/drive">Drive</NavLi>
    <NavLi href="/#/upload">Upload</NavLi>
    <NavLi href="/">About</NavLi>
    <NavLi href="/#/login">Log In</NavLi>
  </NavUl>
</Navbar>

<Router {routes} />
