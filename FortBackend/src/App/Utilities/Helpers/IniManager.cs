using FortBackend.src.App.Utilities.Classes.ConfigHelpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Fortnite;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text;
using ThirdParty.Json.LitJson;
using static System.Collections.Specialized.BitVector32;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class IniManager
    {
        /*
         FORTBACKEND INI MANAGER
        */

        public static string GrabIniFile(string FileName)
        {
            StringBuilder iniBuilder = new StringBuilder();

           

            string filePath = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\ini\\IniConfig.json"));

            if (string.IsNullOrEmpty(filePath))
            {
                return "NotFound";
            }

            string PlaylistData = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\ini\\PlaylistData.json"));

            if (string.IsNullOrEmpty(PlaylistData))
            {
                return "NotFound";
            }

            string QosRegionManagerData = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\ini\\QosRegionManager.json"));

            if (string.IsNullOrEmpty(QosRegionManagerData))
            {
                return "NotFound";
            }

            IniConfig contentconfig = JsonConvert.DeserializeObject<IniConfig>(filePath)!;
            RegionManagerConfig RegionManagerconfig = JsonConvert.DeserializeObject<RegionManagerConfig>(QosRegionManagerData)!;
            List<PlaylistDataClass> PlaylistDataconfig = JsonConvert.DeserializeObject<List<PlaylistDataClass>>(PlaylistData)!;

            if (contentconfig != null && PlaylistDataconfig != null)
            {
                iniBuilder.AppendLine($";{contentconfig.Info} // Generated {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffffZ")} - {FileName}");

                int FindIndex = contentconfig.FileData.FindIndex(e => e.Name == FileName);
                if (FindIndex != -1)
                {
                    IniConfigFiles iniConfigFiles = contentconfig.FileData[FindIndex];

                    if (iniConfigFiles.Name == "DefaultEngine.ini")
                    {
                        // not even proper and forced run times values
                        iniBuilder.AppendLine($"");
                        iniBuilder.AppendLine($"[/Script/Qos.QosRegionManager]");
                        iniBuilder.AppendLine($"NumTestsPerRegion=1");
                        iniBuilder.AppendLine($"PingTimeout=3.0");
                        iniBuilder.AppendLine($"!RegionDefinitions=ClearArray");

                        // updates live
                        RegionManagerconfig.RegionDefinitions.ForEach(e =>
                        {
                            iniBuilder.AppendLine($"+RegionDefinitions=(DisplayName=NSLOCTEXT(\"MMRegion\", \"{e.Region}\", \"{e.Region}\"), RegionId=\"{e.RegionId}\", bEnabled={e.bEnabled}, bVisible={e.bVisible}, bAutoAssignable={e.bAutoAssignable})");
                        });

                        iniBuilder.AppendLine($"!DatacenterDefinitions=ClearArray");

                        RegionManagerconfig.DatacenterDefinitions.ForEach(e =>
                        {
                            iniBuilder.AppendLine($"+DatacenterDefinitions=(Id=\"{e.Id}\", RegionId=\"{e.RegionId}\", bEnabled={e.bEnabled}, Servers[0]=(Address=\"{e.Address}\", Port={e.Port}))");
                        });
                    }

                    foreach (IniConfigData file in iniConfigFiles.Data)
                    {
                        iniBuilder.AppendLine($"");
                        iniBuilder.AppendLine($"[{file.Title}]");
    
                        foreach(IniConfigValues filedata in file.Data)
                        {
                            iniBuilder.AppendLine($"{filedata.Name}={filedata.Value}");
                        }
                    }

                    // could've been better
                    if (iniConfigFiles.Name == "DefaultGame.ini")
                    {
                        iniBuilder.AppendLine($"");
                        iniBuilder.AppendLine($"[/Script/FortniteGame.FortGameInstance]");
                        iniBuilder.AppendLine($"!FrontEndPlaylistData=ClearArray");

                        PlaylistDataconfig.ForEach(file =>
                        {
                            var PlaylistAccess = file.PlaylistAccess;
                            iniBuilder.AppendLine($"+FrontEndPlaylistData=(PlaylistName={file.PlaylistName}, PlaylistAccess=(bEnabled={PlaylistAccess.bEnabled}, bIsDefaultPlaylist={PlaylistAccess.bIsDefaultPlaylist}, bVisibleWhenDisabled={PlaylistAccess.bVisibleWhenDisabled}, bDisplayAsNew={PlaylistAccess.bDisplayAsNew}, CategoryIndex={PlaylistAccess.CategoryIndex}, bDisplayAsLimitedTime={PlaylistAccess.bDisplayAsLimitedTime}, DisplayPriority={PlaylistAccess.DisplayPriority}))");
                        });
                    }

                   
                }else
                {
                    return "NotFound";
                }
            }

            return iniBuilder.ToString();
        }


        public static List<CloudstorageFile> CloudStorageArrayData()
        {
            var list = new List<CloudstorageFile>();
            string filePath = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\ini\\IniConfig.json"));

            if (string.IsNullOrEmpty(filePath))
            {
                return list;
            }

            IniConfig contentconfig = JsonConvert.DeserializeObject<IniConfig>(filePath)!;

            if (contentconfig != null)
            {
                contentconfig.FileData.ForEach(file =>
                {
                    long retardedfilecal = 0;
                    foreach(IniConfigData iniConfigFiles in file.Data)
                    {
                        retardedfilecal += iniConfigFiles.Title.Length;
                        foreach(IniConfigValues e in iniConfigFiles.Data)
                        {
                            retardedfilecal += e.Value.ToString().Length;
                            retardedfilecal += e.Name.Length;
                        }
                    }

                    list.Add(new CloudstorageFile
                    {
                        uniqueFilename = file.Name,
                        filename = file.Name,
                        hash = Hex.MakeHexWithString(file.Name),
                        hash256 = Hex.MakeHexWithString2(file.Name),
                        length = retardedfilecal,
                        contentType = "text/plain",
                        uploaded = file.UploadedTime,
                        storageType = "S3",
                        doNotCache = false
                    });
                });
            }


            return list;
        }
    }
}
