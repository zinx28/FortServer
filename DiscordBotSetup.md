# Setting Up FortBackend with Discord Developer Portal (Discord Bot)

*Note: These steps may be updated in the future.*

### Step 1: Create a New Discord Application
1. Visit [Discord Developer Portal](https://discord.com/developers/applications).
2. Click **New Application**.
3. Name your application (you can choose any name).

*(Video not included)*

### Step 2: Configure the Bot
1. Go to the **Bot** tab.
2. Enable all the **Privileged Gateway Intents**.
3. Reset the **Token** and add it to the `DiscordToken` field in your [Config.Json](https://github.com/zinx28/FortServer/blob/main/FortBackend/Resources/Config.json).

*(Video/Screenshot not included)*

### Step 3: Set Up OAuth2
1. In the **OAuth2** tab:
   - The **Client ID** goes into the `ApplicationClientID` field in your [Config.Json](https://github.com/zinx28/FortServer/blob/main/FortBackend/Resources/Config.json).
   - The **Client Secret** goes into the `ApplicationSecret` field in the same file.
2. To invite the bot:
   - Scroll down to the **OAuth2 URL Generator** section.
   - Check **bot** and **application.commands**.
   - Under **Bot Permissions**, select **Administrator**.
   - Copy and paste the generated link into your browser to invite the bot to your server.

*(Video/Screenshot not included)*

### Step 4: Configure Discord Server
1. Enable [Developer Mode](https://discord.com/developers/docs/activities/building-an-activity#enable-developer-mode-in-your-client) in Discord to copy IDs.
2. Right-click your Discord server and copy the **Server ID** (this goes into `ServerID` in your [Config.Json](https://github.com/zinx28/FortServer/blob/main/FortBackend/Resources/Config.json)).
3. In your server settings, go to **Roles** and move the bot's role above the member roles (this is necessary for the bot to ban members).
4. Create or identify an admin/moderator role, click the three dots, and copy the **Role ID**. Place this ID in the `RoleID` field in your `Config.json`.

*(Video/Screenshot not included)*

---

*Note: This guide does not cover setting up `ApplicationURI` or Discord webhooks. Only the `DetectedWebhookUrl` is required for bans.*
*If the discord slash commands randomly break... then sync your pc clock*
---

## Bot Commands
- /test
- /register
- /change_password
- /details

### Admin commands
- required to have "RoleID" (the role in config.json)
- /who (this command only works if the user has a role, let's say moderator.. ig)
   - Ban (Perm ban, w/ ban assits)
   - Temp Ban (season 7 and above get a ban message in lobby)
   - Vbucks
