using FortLauncher.Pages;
using FortLauncher.Services.Classes;
using FortLauncher.Services.Globals;
using FortLauncher.Services.Utils.Helpers;
using FortLauncher.Services.Utils.Launch.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils.Launch
{
    public class Launch
    {
        private Home MainNav;

        public Launch(Home MainNav)
        {
            this.MainNav = MainNav;
        }


        public async Task Start(BuildConfig config, CancellationToken cancellationToken)
        {
            try
            {
                MainNav.LaunchingUI.Visibility = Visibility.Visible;

                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DllPath = System.IO.Path.Combine(BaseFolder, "FortLauncher", LauncherConfig.CurlDll);

                /*
                 * I'll add this in the future 
                 */

               /* DownloadCurl downloadCurl = new DownloadCurl(MainNav);
                if (File.Exists(DllPath))
                {
                    using (var stream = new BufferedStream(File.OpenRead(DllPath), 1200000))
                    {
                        var sha256 = SHA256.Create();
                        byte[] hashBytes = sha256.ComputeHash(stream);

                        if (BitConverter.ToString(hashBytes).Replace("-", "") != Globals.curl)
                        {
                            stream.Close(); // close it !!! BEFORE
                            MainNav.LaunchTitle.Text = "Downloading Content";
                            await downloadCurl.Init();
                        }
                    }
                }
                else
                {
                    MainNav.LaunchTitle.Text = "Downloading Content";
                    await downloadCurl.Init();
                } */

                // PSBasics.test.IsEnabled = false;
                cancellationToken.ThrowIfCancellationRequested();
                //RPC.Update("Playing " + FortniteDetect.Init(config.VersionID));


                await Task.Run(async () =>
                {
                    try
                    {
                        //Loggers.Log(config.buildPath);
                       // Loggers.Log(FortniteDetect.Init(config.VersionID));
                        if (float.Parse(FortniteDetect.Init(config.VersionID).Split('.')[0]) > 15)
                        {
                            MessageBox.Show("Attemping to launch.. mostly unsupported");

                            PSBasics.Start(config.buildPath, "-obfuscationid=TgUPsnbBaa5S1RTN7Ueu0BOtakEY_w -EpicPortal -epicapp=Fortnite -epiclocale=en -epicsandboxid=fn -epicenv=Prod -epicportal -noeac -noeaceos -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiOWM1MDY1MTEwYzdhNGQ3MDk1ODYyZGE1ZWU4MTU5NjIiLCJnZW5lcmF0ZWQiOjE3MTQ2MDQzOTgsImNhbGRlcmFHdWlkIjoiZWQ3NGZhNWMtZmNmYy00YWM1LWJlMTktZmY1ODQ1MDI0MzU0IiwiYWNQcm92aWRlciI6IkJhdHRsRXllIiwibm90ZXMiOiIiLCJwcmUiOmZhbHNlLCJmYWxsYmFjayI6ZmFsc2V9.YCfBeSzOKM6vMZpe7SXiOxlTdnbFd2Ve2WQv8SAUDHnaDDpSvmZhQKIsjHFNAIYL5t65Pl7zb2yVDVspE6pQ-g  ", "unused", UserData.Token);
                            FakeAC.Start(config.buildPath, "FortniteClient-Win64-Shipping_EAC_EOS.exe", $"");
                            FakeAC.Start(config.buildPath, "FortniteLauncher.exe", $"", true);
                        }
                        else
                        {
                            //Loggers.Log(config.buildPath);
                            PSBasics.Start(config.buildPath, "-obfuscationid=2oaCH8bqszvAC9LdV0CSZoTD5AxLCQ -EpicPortal -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -nobe -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiOWM1MDY1MTEwYzdhNGQ3MDk1ODYyZGE1ZWU4MTU5NjIiLCJnZW5lcmF0ZWQiOjE3MTM3MDUwODUsImNhbGRlcmFHdWlkIjoiZWIzYzM4YjctM2U4Yy00NDZiLWE4NTYtMjM5OTUzMTc3ZDRiIiwiYWNQcm92aWRlciI6IkJhdHRsRXllIiwibm90ZXMiOiIiLCJwcmUiOmZhbHNlLCJmYWxsYmFjayI6ZmFsc2V9.QD69Oonbv7SIcXhYHewzQvh4ymnESQ1LRFQh8_Ufvj8M9bwpl_AbY9wpVRX2lz0x-xslYS40Rf4L34GZJpb20A -skippatchcheck ", "unused", UserData.Token);
                            FakeAC.Start(config.buildPath, "FortniteClient-Win64-Shipping_EAC.exe", $" ");
                            FakeAC.Start(config.buildPath, "FortniteLauncher.exe", $"", true);
                        }

                        PSBasics._FortniteProcess.WaitForInputIdle();
                    }
                    catch (Exception ex)
                    {
                        Loggers.Log(ex.Message);
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                });

                MainNav.LaunchingUI.Visibility = Visibility.Collapsed;

                cancellationToken.ThrowIfCancellationRequested();
                await Task.Run(async () =>
                {
                    Inject.InjectDll(PSBasics._FortniteProcess.Id, DllPath); // redirect!
                    PSBasics._FortniteProcess?.WaitForExit();
                });
                //RPC.Update("Logged in as " + UserData.UserName);
                // PSBasics.test.IsEnabled = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message + "~Start~");
            }
        }
    }
}
