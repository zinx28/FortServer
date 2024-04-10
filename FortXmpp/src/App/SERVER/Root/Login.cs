using FortLibrary.MongoDB.Module;
using FortLibrary;
using FortXmpp.src.App.SERVER.Send;
using System.Net.WebSockets;
using System.Xml.Linq;
using Newtonsoft.Json;
using FortXmpp.src.App.Globals.Data;

namespace FortXmpp.src.App.SERVER.Root
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

                                if (UserDataParsed.banned == false)
                                {
                                    dataSaved.DisplayName = UserDataParsed.Username;
                                    dataSaved.AccountId = UserDataParsed.AccountId;
                                    dataSaved.Token = token;
                                    if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "")
                                    {
                                        dataSaved.DidUserLoginNotSure = true;
                                        Console.WriteLine($"New Xmpp Client Logged In User Name Is As {dataSaved.DisplayName}");

                                        // LOGS THE USER IN IF THEY ARE NOT BANNED

                                        ClientFix.Init(webSocket, dataSaved, clientId);
                                    }
                                }

                              
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
