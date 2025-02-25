# FortBackend

<div align=center>
  <img src="/assets/FORTBACKEND.png" alt="worst image ever">
</div>

## FortBackend is a Universal Fortnite Private Server written in C#.

## About
- Discord server [here](https://discord.gg/8kCu6PDvPd)
- Supports HTTP and HTTPS!
- Support Seasons 3 to 15 (~ season shop for season 1)
- 1:1 Quests (currently some seasons support less / or just not at all)
- Arena UI/Playlists! Seasons 8 to 23 (UNFINISHED, NOT WORKED ON ~ not proper scores on seasons 8 and 11)
- Ban Assist!
- You may use added/unfinished config stuff [FortLibrary/ConfigHelpers/FortConfig.cs](https://github.com/zinx28/FortBackend/blob/main/FortLibrary/ConfigHelpers/FortConfig.cs)

## Setup FortBackend
- Requires VisualStudio To Build
- [Info Here](https://github.com/zinx28/FortServer/blob/main/Setup.md)

## Discord Bot
- Make sure you set up the bot in the configs, and the /test command works
- Discord Bot Disabled On "DEVELOPMENT"
- ActivityType max is 5 (check in config cs file!! for more details)
- More info to setup and commands [here](https://github.com/zinx28/FortBackend/blob/main/DiscordBotSetup.md)

## Added/Not Added 
- MCP [!]
- OAUTH [+]
- Timeline [+]
- Cloudstorage, ClientSaving [+]
- 24/7 Shop (WIP) [!]
- XMPP [!]
- PartyV2 [-]
- Friends [+]
- Matchmaker (WIP) [-]

## Forthoster
This is unfinished.

## Admin Panel (WIP)
I've created an admin page to make it easier to manage JSON configs.

### About
- The admin account is essential and cannot be deleted or edited.
- Admins can grant other "FortBackend" users admin or moderator roles.
- Admins have the ability to edit user roles, but moderators cannot edit users or add new ones (work in progress).
- INI Manager on the dashboard is a mess and i will redo it in the future

### Setup and Login
- Start the FortBackend application.
- Access the login page [here](http://127.0.0.1:2222/login) (right-click to copy the link).
- Use the default email (Admin@gmail.com) and password (AdminPassword123).
- Upon first login, you will be prompted to change your email and password. 

