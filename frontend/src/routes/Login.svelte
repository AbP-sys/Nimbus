<script>
  import { Label, Input, GradientButton, Button } from "flowbite-svelte";
  import { ChevronRightOutline } from "flowbite-svelte-icons";

  let apiID = "";
  let apiHash = "";
  let phoneno = "";
  let verificationCode = "";
  let phonenoEntered = false;
  const { ipcRenderer } = require("electron");
  function handleSubmit() {
    ipcRenderer.send("login", verificationCode);
  }

  function SendVerificationCode() {
    phonenoEntered = true;
    const config = {
      API_ID: apiID,
      API_HASH: apiHash,
      PHONE_NUMBER: phoneno,
    };
    const handleSubmit = async () => {
      const response = await fetch("/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json", // Specify the content type
        },
        body: JSON.stringify(config), // Data to be sent in the request body
      });
      if (response.ok) {
      } else {
      }
      const data = await response.json();
    };
    handleSubmit();
  }
</script>

<div class="mb-6 input-box">
  <p class="text-xl text-gray-600 font-bold p-6">
    Connect to your Telegram Account
  </p>
  <div class="p-2">
    <Input bind:value={apiID} id="API_ID" placeholder="API ID" />
  </div>
  <div class="p-2">
    <Input bind:value={apiHash} id="API_HASH" placeholder="API Hash" />
  </div>
  <div class="p-2">
    <Input bind:value={phoneno} id="PHONE_NUMBER" placeholder="Phone number" />
  </div>
  <div class="p-2">
    <Button on:click={SendVerificationCode}>Send verification code</Button>
  </div>

  {#if phonenoEntered}
    <div class="p-2">
      <Input
        bind:value={verificationCode}
        id="VERTIFCATION_CODE"
        placeholder="Verification Code"
      />
    </div>
    <div class="p-2">
      <GradientButton on:click={handleSubmit} color="cyanToBlue">
        Connect<ChevronRightOutline class="w-3.5 h-3.5 me-2" />
      </GradientButton>
    </div>
  {/if}
</div>

<style>
</style>
