//using Discord;
//using Discord.WebSocket;
//using System;

//namespace FortBackend.src.App.Utilities.Discord.Helpers.command
//{
//    public class TourEvents
//    {
//        public static async Task Respond(SocketSlashCommand command)
//        {
//            try
//            {
//                var embed = new EmbedBuilder()
//                    .WithTitle("Create a event")
//                    .WithDescription("Any Issues tell us! ~ main is required before price pools")
//                    .WithColor(Color.Blue)
//                    .WithCurrentTimestamp();
//                var selectMenu = new SelectMenuBuilder()
//                 .WithCustomId("select_action")
//                 .WithPlaceholder("Choose an action")
//                 .AddOption("Main", "temp-ban", "This is for the main data", null, true);
//                 //.AddOption("Price", "add-vbucks", "This is for price pools");

//                var confirmButton = new ButtonBuilder()
//                    .WithLabel("Open")
//                    .WithStyle(ButtonStyle.Success)
//                    .WithCustomId("confirm_button");

//                var component = new ComponentBuilder()
//                    .WithSelectMenu(selectMenu)
//                    .AddRow(new ActionRowBuilder().WithButton(confirmButton));

//                await command.RespondAsync(embed: embed.Build(), ephemeral: true, components: component.Build());

//                bool InProgess = false;
//                string SelectedAction = "Main";
//                DiscordBot.Client.InteractionCreated += async (interaction) =>
//                {
//                    if (InProgess || interaction.User.Id != command.User.Id)
//                    {
//                        return;
//                    }

//                    if (interaction is SocketMessageComponent MessageComp && MessageComp.User.Id == command.User.Id)
//                    {
//                        if (MessageComp.Data.CustomId == "select_action")
//                        {
//                            await MessageComp.DeferAsync();
//                            var selectedValue = MessageComp.Data.Values.First();
//                            SelectedAction = selectedValue;
//                        }
//                        else if (MessageComp.Data.CustomId == "confirm_button")
//                        {
//                            if (SelectedAction == "Main")
//                            {
//                                var modalBuilder = new ModalBuilder()
//                                .WithTitle("Reason")
//                                .WithCustomId("reasontoban")
//                                .AddTextInput("Title", "reasontoban", placeholder: "Cup Title")
//                                .AddTextInput("Description", "banAssist", placeholder: "Cup Description")
//                                .AddTextInput("When To Begin Event", "time", placeholder: "YYYY-MM-DD HH:mm")
//                                //.AddTextInput("When To End Event", "timetoendevent", placeholder: "This is minutes (60 ~ 1 hour)")
//                                //.AddTextInput("Item To Give", "ggschat", value: "Currency:MtxPurchased", placeholder: "ID:ITEM")
//                                //.AddTextInput("The Amount", "faga", value: "1000", placeholder: "if the item isnt vbucks SET to 1")
//                                //.AddTextInput("Givent Placemnt", "fsa", value: "50", placeholder: "TOP NUMBER")
//                                ;
                                
//                                await interaction.RespondWithModalAsync(modalBuilder.Build());
//                            }
//                            //else if(SelectedAction == "Price")
//                            //{
//                            //    var modalBuilder = new ModalBuilder()
//                            //    .WithTitle("Reason")
//                            //    .WithCustomId("reasontoban")
//                            //    .AddTextInput("Item To Give", "reasontoban", placeholder: "Cup Title")
//                            //    .AddTextInput("Description", "banAssist", placeholder: "Cup Description")
//                            //    .AddTextInput("When To Begin Event", "time", placeholder: "YYYY-MM-DD HH:mm")
//                            //    .AddTextInput("When To End Event", "timetoendevent", placeholder: "This is minutes (60 ~ 1 hour)");
//                            //}
//                        }
//                    }
//                };
//           }
//            catch (Exception ex)
//            {

//            }
//        }
//    }
//}
