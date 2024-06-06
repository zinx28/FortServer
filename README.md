# FortBackend

<div align=center>
  <img src="/assets/FORTBACKEND.png" alt="worst image ever">
</div>

## FortBackend is a Universal Fortnite Private Server written in C#.

## About
- Supports HTTP and HTTPS!
- Aiming to support Seasons 3 to 15
- 1:1 Quests (currently some seasons support less / or just not at all)
- Arena UI/Playlists! Seasons 8 to 23 (UNFINISHED, NOT WORKED ON ~ not proper scores on seasons 8 and 11)
- Save the World (STW) is aimed at the very end and might never actually be implemented
- Ban Assist!
- You may use added/unfinished config stuff [FortBackend/src/App/Utilities/Saved/Config.cs](https://github.com/zinx28/FortBackend/blob/main/FortBackend/src/App/Utilities/Saved/Config.cs)

## Discord Bot
- Make sure you set up the bot in the configs, and the /test command works
- Discord Bot Disabled On "DEVELOPMENT"
- ActivityType max is 5 (check in config cs file!! for more details)
- More info [here](https://github.com/zinx28/FortBackend/blob/main/DiscordBotSetup.md)

### Commands
- /test
- /who (this command only works if the user has a role, let's say moderator.. ig)
- /register
- /change_password

## HTTP
- Just build on Release

## HTTPS
- First, go to [FortBackend/resources/certificates](https://github.com/zinx28/FortBackend/tree/main/FortBackend/Resources/Certificates)
- Remove .temp from the file then replace it with your .pfx (If you have cert and the key just look up how to make it a pfx)
- Before you run the project, make sure you change the build type from Release to HTTPS

## Added/Not Added 
- MCP [!]
- OAUTH [+]
- Timeline [+] (Doesnt support custom events yet... well just fortnite events)
- Cloudstorage, ClientSaving [+]
- 24/7 Shop (WIP - could be so random, items in wrong places - THIS WILL CRASH ON OLDER VERSIONS AS IT TRIES TO GENERATE ITEMS ON SEASONS ITS NOT FOR!) [!]
- XMPP [!]
- PartyV2 [-]
- Friends [-]
- Matchmaker (WIP) [-]

## Admin Panel
I've created an admin page to make it easier to manage JSON configs.

### About
- The admin account is essential and cannot be deleted or edited.
- Currently, the admin panel is styled using [Bootstrap](https://getbootstrap.com). However, this may change in the future.
- Admins can grant other "FortBackend" users admin or moderator roles.
- Admins have the ability to edit user roles, but moderators cannot edit users or add new ones (work in progress).
- INI Manager on the dashboard is a mess and i will redo it in the future

### Setup and Login
- Start the FortBackend application.
- Access the login page [here](http://127.0.0.1:1111/admin/login) (right-click to copy the link).
- Use the default email (Admin@gmail.com) and password (AdminPassword123).
- Upon first login, you will be prompted to change your email and password. 

