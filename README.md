# FortBackend (RECODE IS REQUIRED)

FortBackend is a Universal Fortnite Private Server Backend writen in C#

# Some features may break over time while when i have a chance these issues will be fixed

# About
- Supports HTTP and HTTPS!
- Custom OAUTH SYSTEM (🔥) - requires [LunaLauncher](https://github.com/ProjectLunaMP/Launcher)
- Aiming to support S3 - S15
- Arena UI/Playlists! S8 - S23 (UNFINISHED NOT WORKED ON ~ not proper scores on season 8 and 11 >)
- Save the world (STW) is aimed at the very end and might never actually be implemented
- You may use added/unfished config stuff [FortBackend/src/App/Utilities/Saved/Config.cs](https://github.com/zinx28/FortBackend/blob/main/FortBackend/src/App/Utilities/Saved/Config.cs)

# Discord Bot
- Make sure you setup the bot in the configs... and /test command works

### Commands
- /test
- /who (this command only works if the user has a role... lets say helpers.. ig)

# HTTP
- Just build on Release or Debug

# HTTPS
- First go to [FortBackend/src/resources/certificates](https://github.com/zinx28/FortBackend/tree/main/FortBackend/src/Resources/Certificates)
- Remove .temp from the file then replace it with your .pfx (If you have cert and the key just look up how to make it a pfx)
- Before you run the project make sure you change the build type from (Debug or Release) to HTTPS

# Added/Not Added 
- MCP (QueryProfle, Equiping and useless things) [!]
- Timeline [+]
- Cloudstorage, ClientSaving [+]
- 24/7 Shop (WIP - could be so random, items in wrong places) [!]
- XMPP (WIP PartyV2) [!]
- Friends [-]
- Matchmaker (Won't connect as i havent coded the ws server) [-]
