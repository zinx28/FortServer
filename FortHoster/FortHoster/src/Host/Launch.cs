using FortHoster.src.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// this is pretty much just FortLauncher
namespace FortHoster
{
    public class Launch
    {
        public static async Task Start(WebSocket websocekt, string ID)
        {
            try
            {
                Process _FortniteProcess = null!;

                await Task.Run(async () =>
                {
                    try
                    {
                        if ((int)Saved.ConfigC.Season > 15)
                        {
                            
                            if (File.Exists(System.IO.Path.Combine(Saved.ConfigC.GamePath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC_EOS.exe"))) 
                            {

                                _FortniteProcess = PSBasics.Start(Saved.ConfigC.GamePath, "-obfuscationid=TgUPsnbBaa5S1RTN7Ueu0BOtakEY_w -EpicPortal -epicapp=Fortnite -epiclocale=en -epicsandboxid=fn -epicenv=Prod -epicportal -noeac -noeaceos -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiOWM1MDY1MTEwYzdhNGQ3MDk1ODYyZGE1ZWU4MTU5NjIiLCJnZW5lcmF0ZWQiOjE3MTQ2MDQzOTgsImNhbGRlcmFHdWlkIjoiZWQ3NGZhNWMtZmNmYy00YWM1LWJlMTktZmY1ODQ1MDI0MzU0IiwiYWNQcm92aWRlciI6IkJhdHRsRXllIiwibm90ZXMiOiIiLCJwcmUiOmZhbHNlLCJmYWxsYmFjayI6ZmFsc2V9.YCfBeSzOKM6vMZpe7SXiOxlTdnbFd2Ve2WQv8SAUDHnaDDpSvmZhQKIsjHFNAIYL5t65Pl7zb2yVDVspE6pQ-g  ", Saved.ConfigC.Email, Saved.ConfigC.Password);
                                FakeAC.Start(Saved.ConfigC.GamePath, "FortniteClient-Win64-Shipping_EAC_EOS.exe", $"");
                            } 
                            else
                            {
                                string[] FortniteVersionConv = Saved.ConfigC.Season.ToString().Split('.');
                                float TempVal = float.Parse(FortniteVersionConv[0]);
                             
                                if (TempVal > 15 && TempVal <= 26) // uh gulp
                                {
                                    string fltoken = "3db3ba5dcbd2e16703f3978d";
                                    var fltokensDictionary = new Dictionary<string, string>
                                    {
                                        // pretty sure it's normally with eac
                                        { "16.00", "5dh74c635862g575778132fb " }, // i checked this
                                        { "16.10", "b234a7cc879f663d845c54f9" }, // these are prob wrong
                                        { "16.20", "c98d8b7a7239f78e234a9abc" },
                                        { "16.30", "f45a67dd982a77b9538d1e4f" },
                                        { "16.40", "a12b789c45f67e8912d3c4ff" }
                                    };

                                    if(fltokensDictionary.TryGetValue($"{FortniteVersionConv[0]}.{FortniteVersionConv[1]}", out string ?sigma)) {
                                        if(!string.IsNullOrEmpty(sigma))
                                        {
                                            fltoken = sigma;
                                        }
                                    }

                                    PSBasics.Start(Saved.ConfigC.GamePath, $"-obfuscationid=PM3G4r-oF6DIG-fbJUON8rECU73FWA -epicapp=Fortnite -epicenv=Prod -EpicPortal -epiclocale=en -epicsandboxid=fn -noeac -fromfl=be  -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiOWM1MDY1MTEwYzdhNGQ3MDk1ODYyZGE1ZWU4MTU5NjIiLCJnZW5lcmF0ZWQiOjE3MTQ2MDQzOTgsImNhbGRlcmFHdWlkIjoiZWQ3NGZhNWMtZmNmYy00YWM1LWJlMTktZmY1ODQ1MDI0MzU0IiwiYWNQcm92aWRlciI6IkJhdHRsRXllIiwibm90ZXMiOiIiLCJwcmUiOmZhbHNlLCJmYWxsYmFjayI6ZmFsc2V9.YCfBeSzOKM6vMZpe7SXiOxlTdnbFd2Ve2WQv8SAUDHnaDDpSvmZhQKIsjHFNAIYL5t65Pl7zb2yVDVspE6pQ-g -fltoken={fltoken} -skippatchcheck ", Saved.ConfigC.Email, Saved.ConfigC.Password);
                                    FakeAC.Start(Saved.ConfigC.GamePath, "FortniteClient-Win64-Shipping_BE.exe", $"");
                                }
                                else
                                {
                                    Console.WriteLine("UNSUPPORTED");
                                  
                                    return;
                                }
                                  
                            }
                            FakeAC.Start(Saved.ConfigC.GamePath, "FortniteLauncher.exe", $"", true);
                        }
                        else
                        {
                            PSBasics.Start(Saved.ConfigC.GamePath, "-obfuscationid=2oaCH8bqszvAC9LdV0CSZoTD5AxLCQ -EpicPortal -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -nobe -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiOWM1MDY1MTEwYzdhNGQ3MDk1ODYyZGE1ZWU4MTU5NjIiLCJnZW5lcmF0ZWQiOjE3MTM3MDUwODUsImNhbGRlcmFHdWlkIjoiZWIzYzM4YjctM2U4Yy00NDZiLWE4NTYtMjM5OTUzMTc3ZDRiIiwiYWNQcm92aWRlciI6IkJhdHRsRXllIiwibm90ZXMiOiIiLCJwcmUiOmZhbHNlLCJmYWxsYmFjayI6ZmFsc2V9.QD69Oonbv7SIcXhYHewzQvh4ymnESQ1LRFQh8_Ufvj8M9bwpl_AbY9wpVRX2lz0x-xslYS40Rf4L34GZJpb20A -skippatchcheck ", Saved.ConfigC.Email, Saved.ConfigC.Password);
                            FakeAC.Start(Saved.ConfigC.GamePath, "FortniteClient-Win64-Shipping_EAC.exe", $" ");
                            FakeAC.Start(Saved.ConfigC.GamePath, "FortniteLauncher.exe", $"", true);
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                await Task.Run(async () =>
                {
                    // this could be a list of logs that the game could call
                    bool DidGameStart = false;
                    string[] StartGameLogs = { "UFortReplicationGraph is enabled for"/*, "Match State Changed from WaitingToStart to InProgress"*/ }; // uncomment it out if its a very old version and attempt to join
                    if (_FortniteProcess != null && _FortniteProcess.Id != 0)
                    {
                        Console.WriteLine("Game Has Launched"); // should be removbed in the future

                        _FortniteProcess.OutputDataReceived += (sender, args) =>
                        {
                            if(Saved.ConfigC.GameLogs)
                                Console.WriteLine(args.Data);
                            
                            if (!string.IsNullOrEmpty(args.Data))
                            {
                                if (args.Data.Contains("[UOnlineAccountCommon::ContinueLoggingOut]"))
                                {
                                    Console.WriteLine("Check your redirect");
                                    _FortniteProcess.Kill();
                                }

                                if(args.Data.Contains("Region ")) // soooo fortnite translate this
                                {
                                    Inject.InjectDll(_FortniteProcess.Id, Saved.ConfigC.GameServerDLL);

                                    var DataForMM = new
                                    {
                                        ID = ID,
                                        Message = "LAUNCHING"
                                    };
                                    
                                    websocekt.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DataForMM))), WebSocketMessageType.Text, true, CancellationToken.None);
                                }

                                if(StartGameLogs.Any(log => args.Data.Contains(log)) && !DidGameStart)
                                {
                                    DidGameStart = true;
                                    var DataForMM = new
                                    {
                                        ID = ID,
                                        Message = "JOINABLE"
                                    };

                                    websocekt.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DataForMM))), WebSocketMessageType.Text, true, CancellationToken.None);
                                }

                                //todo aircraft log and end of game log (that couldj ust be on process close)
                            }


                        };


                        _FortniteProcess.BeginOutputReadLine(); // call this first!
                        _FortniteProcess.WaitForInputIdle();
                        Inject.InjectDll(_FortniteProcess.Id, Saved.ConfigC.RedirectDLL); // redirect!
                        _FortniteProcess?.WaitForExit();

                        // Game Closed!
                        var DataForMMfr = new
                        {
                            ID = ID,
                            Message = "CLOSED"
                        };

                        await websocekt.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(DataForMMfr))), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
