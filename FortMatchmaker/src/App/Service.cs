using FortMatchmaker.src.App.Utilities;

namespace FortMatchmaker.src.App
{
    public class Service
    {
        public static void Intiliazation(string[] args)
        {
            Console.WriteLine(@"  ______         _   __  __       _       _                     _             
 |  ____|       | | |  \/  |     | |     | |                   | |            
 | |__ ___  _ __| |_| \  / | __ _| |_ ___| |__  _ __ ___   __ _| | _____ _ __ 
 |  __/ _ \| '__| __| |\/| |/ _` | __/ __| '_ \| '_ ` _ \ / _` | |/ / _ \ '__|
 | | | (_) | |  | |_| |  | | (_| | || (__| | | | | | | | | (_| |   <  __/ |   
 |_|  \___/|_|   \__|_|  |_|\__,_|\__\___|_| |_|_| |_| |_|\__,_|_|\_\___|_|   
                                                                              
                                                                              ");

            Logger.Log("MARVELCO MATCHMAKER IS LOADING (marcellowmellow");


            var builder = WebApplication.CreateBuilder(args);


            var app = builder.Build();
            app.UseRouting();



            app.Run();

        }
    }
}
