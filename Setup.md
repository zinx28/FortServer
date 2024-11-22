# Instructions for installation (ill redo this)

## Prerequisites

Before you start make sure you downloaded/extracted FortServer

### Step 1: Install Visual Studio

Download and install [Visual Studio](https://visualstudio.microsoft.com/downloads/) if you have not already installed it.

### Step 2: Select workload

Once Visual Studio is installed, open the Visual Studio installer, then under the **Workload** tab, select:

- **ASP.NET and web development**
- **.NET Desktop Development**

### Step 3: Install each component

Switch to tab **Individual components** and make sure you have:

- **.NET 7.0 Runtime**

Choose above and install!

### Step 4: Install MongoDB Server

Download and install [Mongodb Server](https://www.mongodb.com/try/download/community) if you have not already installed it.

## Run the project

Once you have installed all the necessary components:

1. Double-click or run the `build.bat` file.

This will build the project for you. And you're all set!

## More Info
### Launch in Https (if https causes issues then create a issue)
- First go to the built location
- Then go to FortBackend/Resources/Config.json 
- Change HTTPS from false to true
- After, go to [FortBackend/resources/certificates](https://github.com/zinx28/FortBackend/tree/main/FortBackend/Resources/Certificates)
- Remove .temp from the file then replace it with your .pfx (If you have cert and the key just look up how to make it a pfx)

### Custom Matchmaker Setup
- In the `config.json` file, set `MatchmakerIP` to your Matchmaker IP, with `"127.0.0.1"` as the default value.
- Set `MatchmakerPort` to your matchmaker port.
- Set `CustomMatchmaker` to `true` to enable the custom matchmaker.
- Add `GameServerIP` to the config and set it to your game server's IP, with `"127.0.0.1"` as the default.
- Optional! add `GameServerPort` to the config and set it to your game server's port. The default is `"7777"`, which is commonly used unless a custom port is set.

## Application URI Setup (Launcher and Backend)

1. Open the [Discord Developer Portal](https://discord.com/developers/applications) and navigate to the **OAuth2** tab.
2. In the **Redirects** section, add a new redirect URL:
   - Use `http://127.0.0.1:1111/launcher/api/v1/callback` as the redirect (port `1111` is the default for the backend).

3. Scroll down to the **OAuth2 URL Generator**:
   - Check these scopes: `Identity`, `email`, and `guilds`.
   - Select the redirect URL you just added.

4. Copy the generated OAuth2 link and paste it into your launcher's source code (this is located at [LauncherConfig.cs](https://github.com/zinx28/FortServer/blob/main/FortLauncher/FortLauncher/Services/Globals/LauncherConfig.cs)).

5. For your backend configuration, use these values:
   - **ApplicationURI**: `http://127.0.0.1:1111/launcher/api/v1/callback`
   - **ApplicationClientID**: The "Client ID" from your application (this should be at the top of the discord dev page!!).
   - **ApplicationSecret**: The "Client Secret" from your application (Same place).

That's it!!!!
