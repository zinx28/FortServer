# How to setup FortBackend With "Dicord Stuff"

- These steps will need to be redone.. but shouldn't be hard to setup!

# Step 1
- First go to https://discord.com/developers/applications
- Click New Application
- Name the application (duh?)

-- VIDEO NOT ADDED

# Step 2

- On the "Bot" Tab
- Enable all the "Privileged Gateway Intents"
- The "Token" will need to be reset then after "DiscordToken" that's in the Config.json

- Video/Screen shot not ADDED

# Step 3

- On the "OAuth2" Tab
- The CLIENT ID is the "ApplicationClientID" that's in Config.Json
- The CLIENT SECRET is the "ApplicationSecret" that's in Config.Json

- To Invite the Bot to your discord server theres a "OAuth2 URL Generator" with this i would click on "bot" and "application.commands" after that "Bot Permissions" just click on "Administrator" copy the link and just paste it in your borwser

- Video/Screen shot not ADDED

# Step 4
- Enable [Developer Mode](https://discord.com/developers/docs/game-sdk/store) to copy ids <- it's very out of topic so just search up how (ill how in the future)
- Right click your discord server then click discord server id (this is ServerID in Config.json)
- In your discord server settings go in Roles and move the "discord bot" above the member roles (this will allow you to ban members with the who command) 
- Also Create a admin/moderator role (or if you already have a role then your fine) click the 3 dots and copy role id.. Put this id in "RoleID" that's in config.json

- Video/Screen shot not ADDED

# This doesn't include ApplicationURI, How to setup a discord webhook (from doing this readme only DetectedWebhookUrl is actually required for bans)









