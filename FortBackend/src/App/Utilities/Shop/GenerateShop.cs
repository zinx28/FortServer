using FortBackend.src.App.Utilities.Shop.Helpers;
using SkiaSharp;
using System.Drawing;

namespace FortBackend.src.App.Utilities.Shop
{
    public class GenerateShop
    {
        public static async Task Init()
        {
            Logger.Log("Generating Shop", "ItemShop");
            //var OutPutFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "output.png");
            //using (var bitmap = new SKBitmap(400, 200))
            //{

            //    using (var canvas = new SKCanvas(bitmap))
            //    {
            //        canvas.Clear(new SKColor(25, 25, 27));
            //        //canvas.DrawRect(50, 50, 150, 150, new SKPaint { Color = SKColors.Blue });

            //        var textPaint = new SKPaint
            //        {
            //            Color = SKColors.Red,
            //            TextSize = 16
            //        };
            //        canvas.DrawText("charles cutie", 150, 100, textPaint);
            //    }

            //    using (var image = SKImage.FromBitmap(bitmap))
            //    using (var data = image.Encode(SKEncodedImageFormat.Png, 1000))
            //    using (var stream = System.IO.File.OpenWrite(OutPutFile))
            //    {
            //        Console.WriteLine("SAVED DATA");
            //        data.SaveTo(stream);
            //    }

            //}
            //DiscordWebsocket.SendEmbed();

        }
    }
}
