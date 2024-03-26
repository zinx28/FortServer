using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Routes.Development;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.XMPP.Root;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.Http;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;

namespace FortBackend.src.App.Utilities.Discord.Helpers.command
{
    public class who
    {

        public static async void CheckAccount(SocketSlashCommand command, SocketGuildUser user, string ign, string id)
        {
            string UserId = string.Empty;
            string UserName = string.Empty;
            if(user == null)
            {
                if(ign == null)
                {
                    if (ulong.TryParse(id, out ulong userId))
                    {
                        var userfound = await DiscordBot.Client.Rest.GetUserAsync(userId);
                        UserId = id;
                        UserName = userfound.Username;
                    }
                }else
                {
                    UserName = ign;
                }
            }else
            {
                UserId = user.Id.ToString();
                UserName = user.Username;
            }
            var FindDiscordID = string.Empty;
            if (UserId == null || ign != null)
            {
                FindDiscordID = await Handlers.FindOne<User>("Username", ign);
            }
            else
            {
                FindDiscordID = await Handlers.FindOne<User>("DiscordId", UserId);
            }

            if (FindDiscordID != "Error")
            {
                User RespondBack = JsonConvert.DeserializeObject<User[]>(FindDiscordID)?[0];
                if (RespondBack == null)
                {
                    await command.RespondAsync("Backend issue ;(((", ephemeral: true);
                    return;
                }

                var banButton = new ComponentBuilder().WithButton("Ban", "ban", ButtonStyle.Danger).Build();
                var unbanButton = new ComponentBuilder().WithButton("Unban", "unban", ButtonStyle.Danger).Build();

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
                if (!RespondBack.banned)
                {
                    await command.RespondAsync(embed: embed.Build(), ephemeral: true, components: banButton);
                }else
                {
                    await command.RespondAsync(embed: embed.Build(), ephemeral: true, components: unbanButton);
                }
                    //var messages = await command.Channel.GetMessagesAsync(1).FlattenAsync();
                DiscordBot.Client.InteractionCreated += async (interaction) =>
                {
                    if (interaction is SocketMessageComponent componentInteraction &&
                        componentInteraction.Data.CustomId == "ban" &&
                        componentInteraction.User.Id == command.User.Id)
                    {
                        ulong messageId = componentInteraction.Message.Id;

                        if(!RespondBack.banned)
                        {
                            var modalBuilder = new ModalBuilder()
                            .WithTitle("Reason")
                            .WithCustomId("reasontoban")
                            .AddTextInput("Reason to ban the user", "reasontoban", placeholder: "Reason to ban the user");
                            await interaction.RespondWithModalAsync(modalBuilder.Build());
                        }else
                        {
                            await interaction.RespondAsync($"Ill add this at some point.. tomorrow prob", ephemeral: true);
                        }
                    }
                    else if (interaction is SocketMessageComponent componentInteraction69 &&
                        componentInteraction69.Data.CustomId == "unban" &&
                        componentInteraction69.User.Id == command.User.Id)
                    {
                        ulong messageId = componentInteraction69.Message.Id;

                      
                        var modalBuilder = new ModalBuilder()
                        .WithTitle("Reason")
                        .WithCustomId("reasontouban")
                        .AddTextInput("Reason to uban the user", "reasontouban", placeholder: "Reason to uban the user");
                        await interaction.RespondWithModalAsync(modalBuilder.Build());
                       
                    }
                    else if (interaction is SocketModal componentInteraction1 &&
                    componentInteraction1.Data.CustomId == "reasontoban" &&
                    componentInteraction1.User.Id == command.User.Id)
                    {
                        if(componentInteraction1.Message != null)
                        {
                            List<SocketMessageComponentData> components = componentInteraction1.Data.Components.ToList();
                            if (components.First(e => e.CustomId == "reasontoban").Value != null)
                            {
                                IMongoCollection<StoreInfo> StoreInfocollection = MongoDBStart.Database.GetCollection<StoreInfo>("StoreInfo");
                                StoreInfo storeinfo = new StoreInfo
                                {
                                    UserIds = new string[] { RespondBack.AccountId },
                                    UserIps = RespondBack.UserIps,
                                    InitialBanReason = components.First(e => e.CustomId == "reasontoban").Value
                                };
                                StoreInfocollection.InsertOne(storeinfo);

                                await Handlers.UpdateOne<User>("DiscordId", RespondBack.DiscordId, new Dictionary<string, object>()
                                {
                                   { "banned", true }
                                });

                                await BanAndWebHooks.Init(Saved.Saved.DeserializeConfig, new UserInfo()
                                {
                                    id = RespondBack.DiscordId,
                                    username = RespondBack.Username
                                }, components.First(e => e.CustomId == "reasontoban").Value, $"<@{command.User.Id}>");
                                await interaction.RespondAsync($"Banned :)", ephemeral: true);
                            }
                        }
                    }else if  (interaction is SocketModal unbanInteraction &&
                    unbanInteraction.Data.CustomId == "reasontouban" &&
                    unbanInteraction.User.Id == command.User.Id)
                    {
                            if (unbanInteraction.Message != null)
                            {
                                    List<SocketMessageComponentData> components = unbanInteraction.Data.Components.ToList();
                                    if (components.First(e => e.CustomId == "reasontouban").Value != null)
                                    {
                                        IMongoCollection<StoreInfo> StoreInfocollection = MongoDBStart.Database.GetCollection<StoreInfo>("StoreInfo");
                                        var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIds, RespondBack.AccountId);
                                        var count = await StoreInfocollection.CountDocumentsAsync(filter);
                                        bool BanUser = false;
                                        if (count > 0)
                                        {
                                            await StoreInfocollection.DeleteOneAsync(filter);
                                        }
                                        await Handlers.UpdateOne<User>("DiscordId", RespondBack.DiscordId, new Dictionary<string, object>()
                                        {
                                           { "banned", false }
                                        });

                                        await unbanAndWebhooks.Init(Saved.Saved.DeserializeConfig, new UserInfo()
                                        {
                                            id = RespondBack.DiscordId,
                                            username = RespondBack.Username
                                        }, components.First(e => e.CustomId == "reasontouban").Value, $"<@{command.User.Id}>");
                                        await interaction.RespondAsync($"Unbanned User :)", ephemeral: true);
                                    }
                            }
                     }
                };

                //await command.RespondAsync(embed: embed.Build(), component: banButton, ephemeral: true);
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
        public static async Task Respond(SocketSlashCommand command)
        {
            try
            {
                if (command.Data.Options.FirstOrDefault(o => o.Name == "mention")?.Value != null)
                {
                    var username = command.Data.Options.FirstOrDefault(o => o.Name == "mention")?.Value as SocketGuildUser;
                    if (username != null)
                    {
                        CheckAccount(command, username, null, null);
                    }
                    else
                    {
                        await command.RespondAsync("The user is not found!", ephemeral: true);
                    }
                }
                else if (command.Data.Options.FirstOrDefault(o => o.Name == "id")?.Value != null)
                {
                    CheckAccount(command, null, null, command.Data.Options.FirstOrDefault(o => o.Name == "id")?.Value?.ToString());
                }
                else if (command.Data.Options.FirstOrDefault(o => o.Name == "ign")?.Value != null)
                {
                    CheckAccount(command, null, command.Data.Options.FirstOrDefault(o => o.Name == "ign")?.Value?.ToString(), null);
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
