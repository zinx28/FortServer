using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Helpers.UserManagement;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB.Module;
using FortLibrary.XMPP;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using static FortLibrary.DiscordAuth;

namespace FortBackend.src.App.Utilities.Discord.Helpers.command
{
    public class Who
    {

        public static async void CheckAccount(SocketSlashCommand command, SocketGuildUser user, string ign, string id)
        {
            try
            {
                string UserId = string.Empty;
                string UserName = string.Empty;
                if (user == null)
                {
                    if (ign == null)
                    {
                        if (ulong.TryParse(id, out ulong userId))
                        {
                            var userfound = await DiscordBot.Client.Rest.GetUserAsync(userId);
                            UserId = id;
                            UserName = userfound.Username;
                            Logger.Log($"Found user {UserName}", "DiscordBot");
                        }
                    }
                    else
                    {
                        UserName = ign;
                    }
                }
                else
                {
                    UserId = user.Id.ToString();
                    UserName = user.Username;
                }

                var FindDiscordID = string.Empty;
                if (UserId == null && ign != null)
                {
                    FindDiscordID = await Handlers.FindOne<User>("Username", ign);
                }
                else
                {
                    if (UserId == null)
                    {
                        return;
                    }
                    FindDiscordID = await Handlers.FindOne<User>("DiscordId", UserId);
                }

                if (FindDiscordID != "Error")
                {
                    User RespondBack = JsonConvert.DeserializeObject<User[]>(FindDiscordID)![0];
                    if (RespondBack == null)
                    {
                        await command.RespondAsync("Backend issue ;(((", ephemeral: true);
                        return;
                    }

                    var WhoIsField = new EmbedFieldBuilder()
                        .WithName("Banned")
                        .WithValue(RespondBack.banned.ToString())
                        .WithIsInline(false);

                    var MentionUserField = new EmbedFieldBuilder()
                        .WithName("Discord")
                        .WithValue($"<@{RespondBack.DiscordId}>")
                        .WithIsInline(false);

                    var embed = new EmbedBuilder()
                        .WithTitle("Player: " + RespondBack.Username)
                        .AddField(WhoIsField)
                        .AddField(MentionUserField)
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp();

                    // ill need to redo this line
                    var selectMenu = new SelectMenuBuilder()
                     .WithCustomId("select_action")
                     .WithPlaceholder("Choose an action")
                     .AddOption(RespondBack.banned ? "Unban" : "Ban", RespondBack.banned ? "unban" : "ban", RespondBack.banned ? "Ban the user" : "Unban the user", null, true)
                     .AddOption("Temp Ban", "temp-ban", "Temporarily ban the user, set days to 0 to unban")
                     .AddOption("Add VBucks", "add-vbucks", "Add VBucks to user account");

                    var confirmButton = new ButtonBuilder()
                        .WithLabel("Confirm")
                        .WithStyle(ButtonStyle.Success)
                        .WithCustomId("confirm_button");

                    var component = new ComponentBuilder()
                        .WithSelectMenu(selectMenu)
                        .AddRow(new ActionRowBuilder().WithButton(confirmButton));




                    await command.RespondAsync(embed: embed.Build(), ephemeral: true, components: component.Build());

                    bool Banned = RespondBack.banned;
                    bool InProgess = false;
                    string SelectedAction = RespondBack.banned ? "unban" : "ban";
                    DiscordBot.Client.InteractionCreated += async (interaction) =>
                    {
                        if (InProgess || interaction.User.Id != command.User.Id)
                        {
                            return;
                        }

                        if(interaction is SocketMessageComponent MessageComp && MessageComp.User.Id == command.User.Id)
                        {
                            if (MessageComp.Data.CustomId == "select_action")
                            {
                                await MessageComp.DeferAsync();
                                var selectedValue = MessageComp.Data.Values.First();
                                SelectedAction = selectedValue;
                            }
                            else if (MessageComp.Data.CustomId == "confirm_button")
                            {
                                ulong messageId = MessageComp.Message.Id;
                                InProgess = true;
                                Console.WriteLine(SelectedAction);
                                if (SelectedAction == "ban")
                                {
                                    var modalBuilder = new ModalBuilder()
                                    .WithTitle("Reason")
                                    .WithCustomId("reasontoban")
                                    .AddTextInput("Reason to ban the user", "reasontoban", placeholder: "Reason to ban the user")
                                    .AddTextInput("Ban Assist", "banAssist", placeholder: "Place other person discords id (who reported)", required: false);
                                    await interaction.RespondWithModalAsync(modalBuilder.Build());
                                }
                                else if (SelectedAction == "unban")
                                {
                                    var modalBuilder = new ModalBuilder()
                                    .WithTitle("Reason")
                                    .WithCustomId("reasontouban")
                                    .AddTextInput("Reason to unban the user", "reasontouban", placeholder: "Reason to unban the user");

                                    await interaction.RespondWithModalAsync(modalBuilder.Build());
                                }
                                else if (SelectedAction == "temp-ban")
                                {
                                    var modalBuilder = new ModalBuilder()
                                    .WithTitle("Reason")
                                    .WithCustomId("reasontotempban")
                                    .AddTextInput("How long (days)", "reasontotempbanDays", TextInputStyle.Short, "Enter how many days", minLength: 1, maxLength: 3);
                                    await interaction.RespondWithModalAsync(modalBuilder.Build());
                                }
                                else if (SelectedAction == "add-vbucks")
                                {
                                    var modalBuilder = new ModalBuilder()
                                    .WithTitle("Reason")
                                    .WithCustomId("reasontoaddvbucks")
                                    .AddTextInput("Add Vbucks", "Vbucks", TextInputStyle.Short, "How Many Vbucks", minLength: 1, maxLength: 10);

                                    await interaction.RespondWithModalAsync(modalBuilder.Build());
                                }
                                else
                                {
                                    var modalBuilder = new ModalBuilder()
                                      .WithTitle("GUlp")
                                      .WithCustomId("sadsa")
                                      .AddTextInput("?????", "sa", TextInputStyle.Short, "bypassed?", minLength: 1, maxLength: 3);
                                    await interaction.RespondWithModalAsync(modalBuilder.Build());
                                }
                            }
                        }

                        if (interaction is SocketModal SocketComp && SocketComp.User.Id == command.User.Id)
                        {

                            if (SocketComp.Message != null)
                            {
                                InProgess = true;

                                List<SocketMessageComponentData> components = SocketComp.Data.Components.ToList();
                                if(components.Count == 0)
                                {
                                    return; // shouldn't work
                                }

                                if (SocketComp.Data.CustomId == "reasontoaddvbucks")
                                {
                                    if (int.TryParse(components.First(e => e.CustomId == "Vbucks").Value, out int Vbucks))
                                    {
                                        ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(RespondBack.DiscordId);

                                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                                        {
                                            int GrabPlacement3 = profileCacheEntry.AccountData.commoncore.Items.Select((pair, index) => (pair.Key, pair.Value, index))
                                             .TakeWhile(pair => !pair.Key.Equals("Currency")).Count();

                                            if (GrabPlacement3 != -1)
                                            {
                                                var Value = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                                                if (Value.templateId != null || Value != null)
                                                {
                                                    if (Value.templateId == "Currency:MtxPurchased")
                                                    {
                                                        Value.quantity += Vbucks;
                                                    }
                                                }
                                            }
                                            var RandomOfferId = Guid.NewGuid().ToString();
                                            profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                            {
                                                templateId = "GiftBox:GB_RMTOffer", // use gb_default instead if giftbox doesnt work
                                                attributes = new GiftCommonCoreItemAttributes {
                                                    lootList = new List<NotificationsItemsClassOG>()
                                                    {
                                                        new NotificationsItemsClassOG
                                                        {
                                                            itemGuid = "Currency:MtxPurchased",
                                                            itemType = "Currency:MtxPurchased",
                                                            quantity =  Vbucks
                                                        }
                                                    }
                                                },
                                                quantity = 1
                                            });
                                            profileCacheEntry.AccountData.commoncore.RVN += 1;
                                            profileCacheEntry.AccountData.commoncore.CommandRevision += 1;

                                            Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                                            if (Client != null)
                                            {
                                                string xmlMessage;
                                                byte[] buffer;
                                                WebSocket webSocket = Client.Game_Client;

                                                if (webSocket != null && webSocket.State == WebSocketState.Open)
                                                {
                                                    XNamespace clientNs = "jabber:client";

                                                    var message = new XElement(clientNs + "message",
                                                        new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                        new XAttribute("to", profileCacheEntry.AccountId),
                                                        new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                                                        {
                                                            payload = new { },
                                                            type = "com.epicgames.gift.received",
                                                            timestamp = DateTime.UtcNow.ToString("o")
                                                        }))
                                                    );

                                                    xmlMessage = message.ToString();
                                                    buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                                    //  Console.WriteLine(xmlMessage);

                                                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                                }

                                            }

                                            await interaction.RespondAsync($"Gave {Vbucks} to {profileCacheEntry.UserData.Username}", ephemeral: true);
                                        }
                                        else
                                        {
                                            await interaction.RespondAsync($"Failed To Grab Users Acc Info", ephemeral: true);
                                        }
                                    }
                                }
                                else if (SocketComp.Data.CustomId == "reasontoban" && !Banned)
                                {
                                    Banned = true;

                                    //banAssist
                                    var BanAssistUser = "";
                                    if (components.First(e => e.CustomId == "banAssist").Value != null)
                                    {
                                        BanAssistUser = components.First(e => e.CustomId == "banAssist").Value;
                                        Console.WriteLine(BanAssistUser);

                                        ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(BanAssistUser);

                                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                                        {
                                            var RandomOfferId = Guid.NewGuid().ToString();

                                            profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                            {
                                                templateId = "GiftBox:gb_banassist_athena",
                                                attributes = new GiftCommonCoreItemAttributes { },
                                                quantity = 1
                                            });
                                            profileCacheEntry.AccountData.commoncore.RVN += 1;
                                            profileCacheEntry.AccountData.commoncore.CommandRevision += 1;

                                            Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                                            if (Client != null)
                                            {
                                                string xmlMessage;
                                                byte[] buffer;
                                                WebSocket webSocket = Client.Game_Client;

                                                if (webSocket != null && webSocket.State == WebSocketState.Open)
                                                {
                                                    XNamespace clientNs = "jabber:client";

                                                    var message = new XElement(clientNs + "message",
                                                        new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                        new XAttribute("to", profileCacheEntry.AccountId),
                                                        new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                                                        {
                                                            payload = new { },
                                                            type = "com.epicgames.gift.received",
                                                            timestamp = DateTime.UtcNow.ToString("o")
                                                        }))
                                                    );

                                                    xmlMessage = message.ToString();
                                                    buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                                    Console.WriteLine(xmlMessage);

                                                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                                }

                                            }
                                        }
                                    }

                                    bool FoundAccount = GlobalData.AccessToken.Any(e => e.accountId == RespondBack.AccountId);

                                    if (FoundAccount)
                                    {
                                        var AccessTokenIndex = GlobalData.AccessToken.FindIndex(i => i.accountId == RespondBack.AccountId);

                                        try
                                        {
                                            if (AccessTokenIndex != -1)
                                            {
                                                var AccessToken = GlobalData.AccessToken[AccessTokenIndex];
                                                if (AccessToken != null)
                                                {
                                                    GlobalData.AccessToken.RemoveAt(AccessTokenIndex);

                                                    var RefreshTokenIndex = GlobalData.RefreshToken.FindIndex(i => i.accountId == RespondBack.AccountId);
                                                    if (RefreshTokenIndex != -1)
                                                    {
                                                        GlobalData.RefreshToken.RemoveAt(RefreshTokenIndex);
                                                    }

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error(ex.Message, "Who command ban");
                                        }

                                        try
                                        {
                                            await MongoSaveData.SaveToDB(RespondBack.AccountId);
                                            CacheMiddleware.GlobalCacheProfiles.Remove(RespondBack.AccountId);
                                        }
                                        catch { } // idfk
                                    }

                                    // Ban user after deleting token and killing cached data
                                    await Handlers.UpdateOne<User>("DiscordId", RespondBack.DiscordId, new Dictionary<string, object>()
                                    {
                                       { "banned", true }
                                    });

                                    // do this last
                                    IMongoCollection<StoreInfo> StoreInfocollection = MongoDBStart.Database!.GetCollection<StoreInfo>("StoreInfo");
                                    StoreInfo storeinfo = new StoreInfo
                                    {
                                        UserIds = new string[] { RespondBack.AccountId },
                                        UserIps = RespondBack.UserIps,
                                        InitialBanReason = components.First(e => e.CustomId == "reasontoban").Value
                                    };
                                    await StoreInfocollection.InsertOneAsync(storeinfo);

                                    await BanAndWebHooks.Init(Saved.Saved.DeserializeConfig, new UserInfo()
                                    {
                                        id = RespondBack.DiscordId,
                                        username = RespondBack.Username
                                    }, components.First(e => e.CustomId == "reasontoban").Value, $"<@{command.User.Id}>");

                                    await interaction.RespondAsync($"Banned :)", ephemeral: true);
                                }
                                else if (SocketComp.Data.CustomId == "reasontotempban" && !Banned) // why temp ban while banned
                                {
                                    if (float.TryParse(components.First(e => e.CustomId == "reasontotempbanDays").Value, out float Days))
                                    {
                                        ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(RespondBack.DiscordId);

                                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                                        {
                                            if (profileCacheEntry.AccountData.commoncore.ban_status.bBanHasStarted)
                                            {
                                                //profileCacheEntry.AccountData.commoncore.ban_history.Add(new BanHistory()
                                                //{
                                                //    BanCount = 1
                                                //});
                                            }

                                            profileCacheEntry.UserData.temp_banned = true;

                                            await Handlers.UpdateOne<User>("DiscordId", RespondBack.DiscordId, new Dictionary<string, object>()
                                            {
                                                { "temp_banned", true }
                                            });


                                            profileCacheEntry.AccountData.commoncore.ban_status = new BanStatus()
                                            {
                                                bRequiresUserAck = true,
                                                banReasons = new List<string>() { "Exploiting" },
                                                bBanHasStarted = true,
                                                banStartTimeUtc = DateTime.UtcNow,
                                                banDurationDays = Days,
                                                exploitProgramName = "Exploiting",
                                                additionalInfo = "",
                                                competitiveBanReason = "Exploiting"
                                            };


                                            profileCacheEntry.AccountData.commoncore.RVN += 1;
                                            profileCacheEntry.AccountData.commoncore.CommandRevision += 1;

                                            Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                                            if (Client != null)
                                            {
                                                string xmlMessage;
                                                byte[] buffer;
                                                WebSocket webSocket = Client.Game_Client;

                                                if (webSocket != null && webSocket.State == WebSocketState.Open)
                                                {
                                                    XNamespace clientNs = "jabber:client";

                                                    var message = new XElement(clientNs + "message",
                                                        new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                                                        new XAttribute("to", profileCacheEntry.AccountId),
                                                        new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                                                        {
                                                            payload = new { },
                                                            type = "com.epicgames.ban_status.received",
                                                            timestamp = DateTime.UtcNow.ToString("o")
                                                        }))
                                                    );

                                                    xmlMessage = message.ToString();
                                                    buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                                    //  Console.WriteLine(xmlMessage);

                                                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                                }

                                            }
                                        }

                                        //// do this last
                                        //IMongoCollection<StoreInfo> StoreInfocollection = MongoDBStart.Database!.GetCollection<StoreInfo>("StoreInfo");
                                        //StoreInfo storeinfo = new StoreInfo
                                        //{
                                        //    UserIds = new string[] { RespondBack.AccountId },
                                        //    UserIps = RespondBack.UserIps,
                                        //    InitialBanReason = components.First(e => e.CustomId == "reasontoban").Value
                                        //};
                                        //await StoreInfocollection.InsertOneAsync(storeinfo);



                                        await TempBanAndWebHooks.Init(Saved.Saved.DeserializeConfig, new UserInfo()
                                        {
                                            id = RespondBack.DiscordId,
                                            username = RespondBack.Username
                                        }, components.First(e => e.CustomId == "reasontotempbanDays").Value, $"<@{command.User.Id}>");

                                        await interaction.RespondAsync($"Temp Banned For {Days} Days", ephemeral: true);
                                    }
                                }
                                else if (SocketComp.Data.CustomId == "reasontouban" && Banned)
                                {
                                    try
                                    {
                                        Banned = false; // temp valiue

                                        try { CacheMiddleware.GlobalCacheProfiles.Remove(RespondBack.AccountId); } catch { }

                                        await Handlers.UpdateOne<User>("DiscordId", RespondBack.DiscordId, new Dictionary<string, object>()
                                        {
                                            { "banned", false }
                                        });


                                        IMongoCollection<StoreInfo> StoreInfocollection = MongoDBStart.Database?.GetCollection<StoreInfo>("StoreInfo")!;
                                        var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIds, RespondBack.AccountId);
                                        var count = await StoreInfocollection.CountDocumentsAsync(filter);
                                        Console.WriteLine(count.ToString());
                                        if (count > 0)
                                        {
                                            await StoreInfocollection.DeleteOneAsync(filter);
                                        }

                                        await unbanAndWebhooks.Init(Saved.Saved.DeserializeConfig, new UserInfo()
                                        {
                                            id = RespondBack.DiscordId,
                                            username = RespondBack.Username
                                        }, components.First(e => e.CustomId == "reasontouban").Value, $"<@{command.User.Id}>");
                                        await interaction.RespondAsync($"Unbanned User :)", ephemeral: true);
                                    }
                                    catch (Exception ex) { Logger.Error(ex.Message, "Couldnt unban user"); }
                                }
                            }
                        }


                        InProgess = false;
                    };
                }
                else
                {
                    var embed = new EmbedBuilder()
                        .WithDescription(UserName + " is not found")
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp();

                    await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CheckAccount");
            }
        }
        public static async Task Respond(SocketSlashCommand command)
        {
            try
            {
                if (command.Data.Options.FirstOrDefault(o => o.Name == "mention")?.Value != null)
                {
                    var username = command.Data.Options.FirstOrDefault(o => o.Name == "mention")?.Value as SocketGuildUser;
                    if (username != null)
                    {
                        CheckAccount(command, username, null!, null!);
                    }
                    else
                    {
                        await command.RespondAsync("The user is not found!", ephemeral: true);
                    }
                }
                else if (command.Data.Options.FirstOrDefault(o => o.Name == "id")?.Value != null)
                {
                    CheckAccount(command, null!, null!, command.Data.Options.FirstOrDefault(o => o.Name == "id")?.Value?.ToString()!);
                }
                else if (command.Data.Options.FirstOrDefault(o => o.Name == "ign")?.Value != null)
                {
                    CheckAccount(command, null!, command.Data.Options.FirstOrDefault(o => o.Name == "ign")?.Value?.ToString()!, null!);
                }
                else
                {
                    await command.RespondAsync("Whoops make sure you selected atleast one thing", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                await command.RespondAsync("error!", ephemeral: true);
            }
        }
    }
}
