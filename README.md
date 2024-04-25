# FortBackend (RECODE REQUIRED)

FortBackend is a Universal Fortnite Private Server Backend written in C#.

## About
- Supports HTTP and HTTPS!
- Custom OAuth System (🔥) - requires [LunaLauncher](https://github.com/ProjectLunaMP/Launcher)
- Aiming to support Seasons 3 to 15
- Arena UI/Playlists! Seasons 8 to 23 (UNFINISHED, NOT WORKED ON ~ not proper scores on seasons 8 and 11)
- Save the World (STW) is aimed at the very end and might never actually be implemented
- You may use added/unfinished config stuff [FortBackend/src/App/Utilities/Saved/Config.cs](https://github.com/zinx28/FortBackend/blob/main/FortBackend/src/App/Utilities/Saved/Config.cs)

## Discord Bot
- Make sure you set up the bot in the configs, and the /test command works

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