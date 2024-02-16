<script>
  import { Label, Input } from "flowbite-svelte";

    let username = '';
    let password = '';
    function handleLogin() {
      // Add your login logic here
      // For simplicity, you can just log the username for now
      console.log(`Logged in as ${username}`);
    }
</script>
  
  <style>
    /* Add your styling here */
  </style>
  
  <div class="mb-6">
    <Label class="block mb-2">Connect to your Telegram Account</Label>
    <Input id="API_KEY" placeholder="API Key" />
    <Input id="API_HASH" placeholder="API Hash" />
    <Input id="PHONE_NUMBER" placeholder="Phone number" />
    <Input id="VERTIFCATION_CODE" placeholder="Verification Code" />
  </div>
  