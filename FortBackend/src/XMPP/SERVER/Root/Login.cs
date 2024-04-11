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

namespace FortBackend.src.App.SERVER.Root
{
    public class Login
    {
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, DataSaved_XMPP dataSaved)
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
                    Console.WriteLine(token);

                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.GetAsync($"{Saved.DeserializeConfig.DefaultProtocol}127.0.0.1:{Saved.DeserializeConfig.BackendPort}/PRIVATE/DEVELOPER/DATA/TOKEN/{token}");

                    if (response.IsSuccessStatusCode)
                    {
                        //ProfileCacheEntry
                        var datareturned = await response.Content.ReadAsStringAsync();
                        if (datareturned != null)
                        {
                            ProfileCacheEntry Data = JsonConvert.DeserializeObject<ProfileCacheEntry>(datareturned)!;
                            if (Data.AccountData != null)
                            {
                                User UserDataParsed = Data.UserData;
                                if (UserDataParsed == null)
                                {
                                    await Client.CloseClient(webSocket);
                                    return;
                                }

                                var DeviceID = Hex.GenerateRandomHexString(16);

                                // WE WILL GENERATE A exchange_code TOKEN
                                string RefreshToken = JWT.GenerateJwtToken(new[]
                                {
                                    new Claim("sub", Data.AccountData.AccountId),
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
                                        new Claim("sub",  Data.AccountData.AccountId),
                                        new Claim("dvid", DeviceID),
                                        new Claim("mver", "false"),
                                        new Claim("clid", clientId),
                                        new Claim("dn",  Data.UserData.Username),
                                        new Claim("am", "exchange_code"),
                                        new Claim("sec", "1"),
                                        new Claim("p", Hex.GenerateRandomHexString(256)),
                                        new Claim("iai",  Data.AccountData.AccountId),
                                        new Claim("clsvc", "fortnite"),
                                        new Claim("t", "s"),
                                        new Claim("ic", "true"),
                                        new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 480 * 480).ToString()),
                                        new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                                        new Claim("jti", Hex.GenerateRandomHexString(32)),
                                }, 8);

                                httpClient.DefaultRequestHeaders.Add("Authorization", AccessToken);
                                httpClient.DefaultRequestHeaders.Add("RefreshToken", RefreshToken);

                                response = await httpClient.GetAsync($"{Saved.DeserializeConfig.DefaultProtocol}127.0.0.1:{Saved.DeserializeConfig.BackendPort}/account/api/oauth/websocket/addnew/token");
                                Console.WriteLine(response);
                                if (response.IsSuccessStatusCode)
                                {
                                    //ProfileCacheEntry
                                    datareturned = await response.Content.ReadAsStringAsync();
                                    if (datareturned != null)
                                    {
                                        if (UserDataParsed.banned == false)
                                        {
                                            dataSaved.DisplayName = UserDataParsed.Username;
                                            dataSaved.AccountId = UserDataParsed.AccountId;
                                            dataSaved.Token = AccessToken; // used wrong token all this time
                                            if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "")
                                            {
                                                dataSaved.DidUserLoginNotSure = true;
                                                Console.WriteLine($"New Xmpp Client Logged In User Name Is As {dataSaved.DisplayName} with account id {dataSaved.AccountId}");

                                                // LOGS THE USER IN IF THEY ARE NOT BANNED
                                                ClientFix.Init(webSocket, dataSaved, clientId);
                                            }
                                        }


                                    }
                                }
                                        // /account/api/oauth/websocket/addnew/token

                                      
                              
                            }

                        }
                    }else
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
