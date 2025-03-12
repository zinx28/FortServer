using FortLibrary.Encoders;
using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Fortnite;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using System.Security.Cryptography;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class IniManager
    {
        /*
         FORTBACKEND INI MANAGER
        */
        public static IniConfig IniConfigData { get; set; } = new IniConfig();

        public static string GetFileHashfrfr(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
                byte[] hashBytes = sha256.ComputeHash(fileNameBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static string GetFileHashBasicversionfrfrfr(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
                byte[] hashBytes = md5.ComputeHash(fileNameBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static string GrabIniFile(string FileName)
        {
            StringBuilder iniBuilder = new StringBuilder();

         
            string PlaylistData = System.IO.File.ReadAllText(Path.Combine(PathConstants.CloudStorage.PlaylistData));

            if (string.IsNullOrEmpty(PlaylistData))
            {
                return "NotFound";
            }

            string QosRegionManagerData = System.IO.File.ReadAllText(Path.Combine(PathConstants.CloudStorage.QosRegionManager));

            if (string.IsNullOrEmpty(QosRegionManagerData))
            {
                return "NotFound";
            }

            RegionManagerConfig RegionManagerconfig = JsonConvert.DeserializeObject<RegionManagerConfig>(QosRegionManagerData)!;
            List<PlaylistDataClass> PlaylistDataconfig = JsonConvert.DeserializeObject<List<PlaylistDataClass>>(PlaylistData)!;

            if (IniManager.IniConfigData != null)
            {
                iniBuilder.AppendLine($";{IniManager.IniConfigData.Info} // Generated {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffffZ")} - {FileName}");

                int FindIndex = IniManager.IniConfigData.FileData.FindIndex(e => e.Name == FileName);
                if (FindIndex != -1)
                {
                    IniConfigFiles iniConfigFiles = IniManager.IniConfigData.FileData[FindIndex];

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
                            iniBuilder.AppendLine($"+RegionDefinitions=(DisplayName=\"{e.Region}\", RegionId=\"{e.RegionId}\", bEnabled={e.bEnabled.ToString().ToLower()}, bVisible={e.bVisible}, bAutoAssignable={e.bAutoAssignable})");
                        });

                        iniBuilder.AppendLine($"!DatacenterDefinitions=ClearArray");

                        RegionManagerconfig.DatacenterDefinitions.ForEach(e =>
                        {
                            iniBuilder.AppendLine($"+DatacenterDefinitions=(Id=\"{e.Id}\", RegionId=\"{e.RegionId}\", bEnabled={e.bEnabled.ToString().ToLower()}, Servers[0]=(Address=\"{e.Address}\", Port={e.Port}))");
                        });
                    }

                    foreach (IniConfigData file in iniConfigFiles.Data)
                    {
                        iniBuilder.AppendLine($"");
                        iniBuilder.AppendLine($"[{file.Title}]");

                        foreach (IniConfigValues filedata in file.Data)
                        {
                            if (filedata.Value is bool boolValue)
                                filedata.Value = filedata.Value.ToString().ToLower();

                            iniBuilder.AppendLine($"{filedata.Name}={filedata.Value}");
                        }
                    }

                    // could've been better
                    if (iniConfigFiles.Name == "DefaultGame.ini")
                    {
                        iniBuilder.AppendLine($"");
                        iniBuilder.AppendLine($"[/Script/FortniteGame.FortGameInstance]");
                        iniBuilder.AppendLine($"!FrontEndPlaylistData=ClearArray");

                        if(PlaylistDataconfig != null)
                        {
                            PlaylistDataconfig.ForEach(file =>
                            {
                                var PlaylistAccess = file.PlaylistAccess;
                                iniBuilder.AppendLine($"+FrontEndPlaylistData=(PlaylistName={file.PlaylistName}, PlaylistAccess=(bEnabled={PlaylistAccess.bEnabled.ToString().ToLower()}, bIsDefaultPlaylist={PlaylistAccess.bIsDefaultPlaylist}, bVisibleWhenDisabled={PlaylistAccess.bVisibleWhenDisabled.ToString().ToLower()}, bDisplayAsNew={PlaylistAccess.bDisplayAsNew.ToString().ToLower()}, CategoryIndex={PlaylistAccess.CategoryIndex}, bDisplayAsLimitedTime={PlaylistAccess.bDisplayAsLimitedTime.ToString().ToLower()}, DisplayPriority={PlaylistAccess.DisplayPriority}))");
                            });
                        }
                    }

                   
                }else
                {
                    return "NotFound";
                }
            }

            return iniBuilder.ToString();
        }


        public static List<CloudstorageFileShort> CloudStorageArrayData()
        {
            var list = new List<CloudstorageFileShort>();
            string filePath = System.IO.File.ReadAllText(Path.Combine(PathConstants.CloudStorage.IniConfig));

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

                    list.Add(new CloudstorageFileShort
                    {
                        uniqueFilename = file.Name,
                        filename = file.Name,
                        hash = Hex.MakeHexWithString(file.Name),
                        hash256 = Hex.MakeHexWithString2(file.Name),
                        length = retardedfilecal,
                        contentType = "application/octet-stream",
                        uploaded = file.UploadedTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        storageType = "S3",
                        doNotCache = true
                    });
                });
            }


            return list;
        }
    }
}
