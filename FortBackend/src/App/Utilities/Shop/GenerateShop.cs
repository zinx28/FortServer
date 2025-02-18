using FortBackend.src.App.Utilities.Shop.Helpers;
using SkiaSharp;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson.Serialization.Serializers;
using FortLibrary.Shop;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary;

namespace FortBackend.src.App.Utilities.Shop
{
    public class GenerateShop
    {
        public static async Task Init()
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Warn("SHOP WILL BE SKUNKED / MAY HAVE ISSUES");
            Logger.Log("Generating Shop", "ItemShop");

            SavedData savedData = new SavedData();

            Random RandomNumber = new Random();
          //  savedData.WeeklyItems = RandomNumber.Next(1, 2);
         //   DailyItems = RandomNumber.Next(1, 2);

            if (Saved.Saved.DeserializeGameConfig.ForceSeason)
            {
                savedData.Season = Saved.Saved.DeserializeGameConfig.Season;
                
                if(savedData.Season <= 14)
                {
                    savedData.WeeklyItems = RandomNumber.Next(2, 3);
                    savedData.DailyItems = RandomNumber.Next(5, 7);

                    if (savedData.DailyItems > 7) { savedData.DailyItems = 7; };
                    if (savedData.WeeklyItems > 4) { savedData.WeeklyItems = 4; };
                }
                else
                {
                    savedData.WeeklyItems = RandomNumber.Next(1, 2);
                    savedData.DailyItems = RandomNumber.Next(1, 2);

                    if (savedData.WeeklyItems > 3) { savedData.WeeklyItems = 3; }
                    if (savedData.DailyItems > 3) { savedData.DailyItems = 3; }
                }
            }
            else
            {
                Logger.Error("WARN WARN WARN", "ItemShop");
                Logger.Warn("Force Season is off ~", "ItemShop");
                Logger.Error("STOPPING", "ItemShop");
                return;
            }

            Logger.Log($"Generating shop for season {savedData.Season}");

            savedData = await Generator.Start(savedData);
           
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Logger.Log($"Shop Generated in {elapsedMilliseconds}ms", "ItemShop");

            await DiscordWebsocket.SendEmbed(savedData);
        }
    }
}
