using Discord;
using Discord.WebSocket;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Management;
using FortLibrary;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB;
using FortLibrary.MongoDB.Module;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace FortBackend.src.App.Utilities.Discord.Helpers.command
{
    public class Create
    {
        public static async Task Respond(SocketSlashCommand command)
        {
            try
            {
                var displayNameOption = command.Data.Options.FirstOrDefault(o => o.Name == "displayname");
                var emailOption = command.Data.Options.FirstOrDefault(o => o.Name == "email");
                var passwordOption = command.Data.Options.FirstOrDefault(o => o.Name == "password");

                if (displayNameOption?.Value is string displayName &&
                    emailOption?.Value is string email &&
                    passwordOption?.Value is string password)
                {
                    var FindDiscordId = await Handlers.FindOne<User>("DiscordId", command.User.Id.ToString());
                    if (FindDiscordId != "Error")
                    {
                        var embed = new EmbedBuilder()
                        .WithTitle("Account Creation Failed")
                        .WithDescription("You already have an existing account.")
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp();

                        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                    }
                    else
                    {
                        if (displayName.Length < 3 || displayName.Length > 25)
                        {
                            var embed = new EmbedBuilder()
                            .WithTitle("Account Creation Failed")
                            .WithDescription("Display name must be between 3 and 25 characters.")
                            .WithColor(Color.Blue)
                            .WithCurrentTimestamp();

                            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                            return;
                        }
                        else if (!Regex.IsMatch(displayName, @"^[\p{L}0-9_\s]+$"))
                        {
                            var embed = new EmbedBuilder()
                           .WithTitle("Account Creation Failed")
                           .WithDescription("Display name can only contain letters, numbers, underscores, and spaces.")
                           .WithColor(Color.Blue)
                           .WithCurrentTimestamp();

                            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                            return;

                        }
                        else
                        {
                            var FindDisplayName = await Handlers.FindOne<User>("Username", displayName);
                            if (FindDisplayName == "Error")
                            {
                                if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                                {
                                    var embed = new EmbedBuilder()
                                    .WithTitle("Account Creation Failed")
                                    .WithDescription("Invaild Email Fortmat")
                                    .WithColor(Color.Blue)
                                    .WithCurrentTimestamp();

                                    await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                                    return;

                                }
                                else
                                {
                                    if (password.Length >= 7)
                                    {
                                        // Creates a FortBackend Account
                                        if (MongoDBStart.Database != null)
                                        {
                                            await MongoDBCreateAccount.Init(new CreateAccountArg
                                            {
                                                DiscordId = command.User.Id.ToString(),
                                                DisplayName = displayName,
                                                Email = email,
                                                Password = password,
                                            });

                                            var embed = new EmbedBuilder()
                                           .WithTitle("Account Created")
                                           .WithDescription($"You have created a account named {displayName}!")
                                           .WithColor(Color.Blue)
                                           .WithCurrentTimestamp();

                                            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                                            return;
                                        }
                                        else
                                        {
                                            await command.RespondAsync("Database is null! :D", ephemeral: true);
                                        }

                                    }
                                    else
                                    {
                                        var embed = new EmbedBuilder()
                                        .WithTitle("Account Creation Failed")
                                        .WithDescription("Min Password Length Is 7")
                                        .WithColor(Color.Blue)
                                        .WithCurrentTimestamp();

                                        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                                        return;

                                    }
                                 
                                }
                                  
                            }
                            else
                            {
                                var embed = new EmbedBuilder()
                                .WithTitle("Account Creation Failed")
                                .WithDescription("There's already a account named this")
                                .WithColor(Color.Blue)
                                .WithCurrentTimestamp();

                                await command.RespondAsync(embed: embed.Build(), ephemeral: true);
                            }
                        }   
                    }
                }
                else
                {
                    await command.RespondAsync("AH Crap! There's a Error!! terrible one ofc!! fortbackend couldnt create you a account ! please try again Honey", ephemeral: true);
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
