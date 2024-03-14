# FortBackend

FortBackend is a Universal Fortnite Private Server Backend writen in C#

# About
- Supports HTTP and HTTPS!
- Aiming to support S3 - S15
- Arena UI/Playlists! S8 - S23 (Working.... Not For Prod)
- Save the world (STW) is aimed at the very end and might never actually be implemented

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