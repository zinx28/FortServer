using FortLibrary.MongoDB.Module;
using FortLibrary;
using System.Net.WebSockets;
using System.Xml.Linq;
using Newtonsoft.Json;
using FortLibrary.Encoders;
using System.Security.Claims;
using FortBackend.src.XMPP.Data;
using FortBackend.src.App.SERVER.Send;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.XMPP;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FortBackend.src.App.Utilities.Helpers;

namespace FortBackend.src.App.SERVER.Root
{
    public class Login
    {
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, string Ip)
        {
            try
            {
                string xmlMessage;
                byte[] buffer;
                if (clientId == null)
                {
                    await Client.CloseClient(webSocket);
                    return;
                }

               

                var token = xmlDoc.Root?.Element("token")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    ProfileCacheEntry profileCacheEntry = await GrabData.Profile("", true, token);
                    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId) && profileCacheEntry.UserData.banned != true)
                    {
                        if (profileCacheEntry.AccountData != null)
                        {
                            User UserDataParsed = profileCacheEntry.UserData;
                            if (UserDataParsed == null)
                            {
                                await Client.CloseClient(webSocket);
                                return;
                            }

                            var DeviceID = Hex.GenerateRandomHexString(16);

                            // WE WILL GENERATE A exchange_code TOKEN
                            string RefreshToken = JWT.GenerateJwtToken(new[]
                            {
                                new Claim("sub", profileCacheEntry.AccountData.AccountId),
                                new Claim("t", "r"),
                                new Claim("dvid", DeviceID),
                                new Claim("clid", clientId),
                                new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1920 * 1920).ToString()),
                                new Claim("am", "exchange_code"),
                                new Claim("jti", Hex.GenerateRandomHexString(32)),
                            }, 24);

                            string AccessToken = JWT.GenerateJwtToken(new[]
                            {
                                    new Claim("app", "fortnite"),
                                    new Claim("sub",  profileCacheEntry.AccountData.AccountId),
                                    new Claim("dvid", DeviceID),
                                    new Claim("mver", "false"),
                                    new Claim("clid", clientId),
                                    new Claim("dn",  profileCacheEntry.UserData.Username),
                                    new Claim("am", "exchange_code"),
                                    new Claim("sec", "1"),
                                    new Claim("p", Hex.GenerateRandomHexString(256)),
                                    new Claim("iai",  profileCacheEntry.AccountData.AccountId),
                                    new Claim("clsvc", "fortnite"),
                                    new Claim("t", "s"),
                                    new Claim("ic", "true"),
                                    new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 480 * 480).ToString()),
                                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                                    new Claim("jti", Hex.GenerateRandomHexString(32)),
                            }, 8);

                            if (UserDataParsed.banned == false)
                            {
                                DataSaved dataSaved = new DataSaved();
                                dataSaved.DiscordId = UserDataParsed.DiscordId;
                                dataSaved.DisplayName = UserDataParsed.Username;
                                dataSaved.AccountId = UserDataParsed.AccountId;
                                dataSaved.Token = AccessToken; // used wrong token all this time
                                if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "")
                                {
                                    dataSaved.DidUserLoginNotSure = true;

                                    string IPResponse = await CheckIP.Init(Ip, UserDataParsed);
                                    if (IPResponse == "ok")
                                    {
                                        var FindAccount = GlobalData.AccessToken.FirstOrDefault(e => e.accountId == profileCacheEntry.AccountId);
                                        if (FindAccount != null)
                                        {
                                            GlobalData.AccessToken.Remove(FindAccount);
                                            GlobalData.AccessToken.Add(new TokenData
                                            {
                                                token = $"eg1~{AccessToken}",
                                                creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                                accountId = FindAccount.accountId,
                                            });
                                        }
                                        else
                                        {
                                            GlobalData.AccessToken.Add(new TokenData
                                            {
                                                token = $"eg1~{AccessToken}",
                                                creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                                accountId = profileCacheEntry.AccountId,
                                            });
                                        }

                                        var RefreshAccount = GlobalData.RefreshToken.FirstOrDefault(e => e.accountId == profileCacheEntry.AccountId);
                                        if (RefreshAccount != null)
                                        {
                                            GlobalData.RefreshToken.Remove(RefreshAccount);
                                            GlobalData.RefreshToken.Add(new TokenData
                                            {
                                                token = $"eg1~{RefreshToken}",
                                                creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                                accountId = RefreshAccount.accountId,
                                            });
                                        }
                                        else
                                        {
                                            GlobalData.RefreshToken.Add(new TokenData
                                            {
                                                token = $"eg1~{RefreshToken}",
                                                creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                                accountId = profileCacheEntry.AccountId,
                                            });
                                        }
                                        ClientFix.Init(webSocket, dataSaved, clientId);
                                    }else
                                    {
                                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "banned", CancellationToken.None);
                                        return;
                                    }
                                    //Console.WriteLine($"New Xmpp Client Logged In User Name Is As {dataSaved.DisplayName} with account id {dataSaved.AccountId}");

                                    // LOGS THE USER IN IF THEY ARE NOT BANNED
                                   
                                }
                            }else
                            {
                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "banned", CancellationToken.None);
                                return;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("FAILED");
                    }                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "LOGIN");
            }
        }
    }
}
