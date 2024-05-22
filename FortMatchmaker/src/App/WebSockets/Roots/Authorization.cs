using FortLibrary;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Matchmaker;
using FortLibrary.MongoDB.Module;
using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.Utilities.MongoDB.Helpers;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace FortMatchmaker.src.App.WebSockets.Roots
{
    public class Authorization
    {
        public static async Task<MatchmakerTicket> Init(WebSocket websocket, IHeaderDictionary Headers)
        {
            try
            {
                if(Headers.TryGetValue("Authorization", out var AuthValues))
                {
                    if (!string.IsNullOrEmpty(AuthValues)){
                        int StartIndex = AuthValues.ToString().IndexOf(' ');

                        if (StartIndex != -1)
                        {
                            string AuthToken = AuthValues.ToString().Substring(StartIndex);
                            string[] parts = AuthToken.Split(' ');

                            if(parts.Length >= 4)
                            {
                                string ValidToken = parts[3];
                                string SignatureJson = GenerateAES.DecryptAES256(ValidToken, Saved.DeserializeConfig.JWTKEY);

                                // Trying to prevent fake tokens
                                if (!string.IsNullOrEmpty(SignatureJson))
                                {
                                    MatchmakerTicket matchmakerTicket = JsonConvert.DeserializeObject<MatchmakerTicket>(SignatureJson);

                                    if (matchmakerTicket != null)
                                    {
                                        //Console.WriteLine(ValidToken);
                                        //Console.WriteLine(SignatureJson);
                                        
                                        if (DateTime.TryParse(matchmakerTicket.timestamp, out DateTime timestamp))
                                        {
                                            DateTime currentUtcTime = DateTime.UtcNow;
                                            TimeSpan threshold = TimeSpan.FromDays(1);

                                          
                                            if (currentUtcTime - timestamp <= threshold)
                                            {
                                                var FindUser = await Handlers.FindOne<User>("accountId", matchmakerTicket.accountId);
                                                if(FindUser != "Error")
                                                {
                                                    User UserDataParsed = JsonConvert.DeserializeObject<User[]>(FindUser)![0];

                                                    if(UserDataParsed != null)
                                                    {
                                                        // Valid Account IG?

                                                        if (!UserDataParsed.banned)
                                                        {
                                                            return matchmakerTicket;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                websocket.Dispose();
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message, "Authorization");
                websocket.Dispose();
            }

            return new MatchmakerTicket();
        }
    }
}
