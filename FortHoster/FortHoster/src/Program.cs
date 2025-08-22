using FortHoster;
using FortHoster.src.Classes;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;

Console.WriteLine("FortHoster... Please note this project isnt fully tested and may come with big issues");
Console.WriteLine("REDIRECT: if you havent you have to remove console logs on the dll");
Console.WriteLine("DLL: do the same");

// This hoster may have issues but this should help!

// Goals...
// Open this then this should connect to the matchmaker
// matchmaker will now basically talk to this + each region will have different data
// this and matchmaker will need a key to talk to each other since we cant allow random people connecting to this!

// i need to cleanup this project and not make everything inside program

var ReadConfig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Config.json"));

if (ReadConfig == null)
{
    Console.WriteLine("Couldn't find config (config.json)");
    throw new Exception($"Couldn't find config\n{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Config.json")}");
}

Saved.ConfigC = JsonConvert.DeserializeObject<ConfigC>(ReadConfig)!;

// just check files

if (!File.Exists(Saved.ConfigC.RedirectDLL))
{
    Console.WriteLine("Couldn't find redirect");
    throw new Exception($"Couldn't find redirect");
}

if (!File.Exists(Saved.ConfigC.GameServerDLL))
{
    Console.WriteLine("Couldn't find game server");
    throw new Exception($"Couldn't find game server");
}

Console.WriteLine("You have headless " + (Saved.ConfigC.Headless ? " enabled" : "disabled"));
Console.WriteLine("MOST GAMESERVERS USE THE DEFAULT NETDRIVER PORT (7777)");

// need to add the wss for https
using var client = new ClientWebSocket();
client.Options.SetRequestHeader("Authorization", Saved.ConfigC.Key);
await client.ConnectAsync(new Uri($"ws://{Saved.ConfigC.IP}:{Saved.ConfigC.Port}//"), CancellationToken.None);

var buffer = new byte[1024];

// Now we should really send a message back!
var DataForMM = new
{
    // this is for the matchmaker to know... lets say NAE is up and EU isnt you cant create a EU match
    Playlist = Saved.ConfigC.Playlist.ToLower(), // playlist needs to be the same!
    Region = Saved.ConfigC.Region,
    IP = Saved.ConfigC.GameServerIP,
    Port = Saved.ConfigC.GameServerPort,
};
await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DataForMM))), WebSocketMessageType.Text, true, CancellationToken.None);


while (client.State == WebSocketState.Open)
{
    try
    {
        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        MessageClass messageClass = JsonConvert.DeserializeObject<MessageClass>(Encoding.UTF8.GetString(buffer, 0, result.Count))!;
        
        if(messageClass != null && !string.IsNullOrEmpty(messageClass.ID))
        {
            if(messageClass.Message == "HOST")
            {
                // now we host the server!
                Task.Run(async () => await Launch.Start(client, messageClass.ID));
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        break;
    }
}