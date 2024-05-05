# FortBackend (RECODE REQUIRED)

<div align=center>
  <img src="/assets/FORTBACKEND.png" alt="worst image ever">
</div>

# FortBackend is a Universal Fortnite Private Server written in C#.

## About
- Supports HTTP and HTTPS!
- Custom OAuth System (🔥) - requires [LunaLauncher](https://github.com/ProjectLunaMP/Launcher)
- Aiming to support Seasons 3 to 15
- Arena UI/Playlists! Seasons 8 to 23 (UNFINISHED, NOT WORKED ON ~ not proper scores on seasons 8 and 11)
- Save the World (STW) is aimed at the very end and might never actually be implemented
- You may use added/unfinished config stuff [FortBackend/src/App/Utilities/Saved/Config.cs](https://github.com/zinx28/FortBackend/blob/main/FortBackend/src/App/Utilities/Saved/Config.cs)

## Discord Bot
- Make sure you set up the bot in the configs, and the /test command works
- Discord Bot Disabled On "DEVELOPMENT"

### Commands
- /test
- /who (this command only works if the user has a role, let's say moderator.. ig)

## HTTP
- Just build on Release or Debug

## HTTPS
- First, go to [FortBackend/src/resources/certificates](https://github.com/zinx28/FortBackend/tree/main/FortBackend/src/Resources/Certificates)
- Remove .temp from the file then replace it with your .pfx (If you have cert and the key just look up how to make it a pfx)
- Before you run the project, make sure you change the build type from (Debug or Release) to HTTPS

## Added/Not Added 
- MCP (Season 10 login is skunked.. i will need to look at this) [!]
- Timeline [+]
- Cloudstorage, ClientSaving [+]
- 24/7 Shop (WIP - could be so random, items in wrong places) [!]
- XMPP (WIP PartyV2) [!]
- Friends [-]
- Matchmaker (Won't connect as I haven't coded the WS server) [-]

## Admin Panel
I've created an admin page to make it easier to manage JSON configs.

### About
- The admin account is essential and cannot be deleted or edited.
- Currently, the admin panel is styled using [Bootstrap](https://getbootstrap.com). However, this may change in the future.
- Admins can grant other "FortBackend" users admin or moderator roles.
- Admins have the ability to edit user roles, but moderators cannot edit users or add new ones (work in progress).

### Setup and Login
- Start the FortBackend application.
- Access the login page [here](http://127.0.0.1:1111/admin/login) (right-click to copy the link).
- Use the default email (Admin@gmail.com) and password (AdminPassword123).
- Upon first login, you will be prompted to change your email and password.

