using FortLauncher.Services.Classes;
using FortLauncher.Services.Utils.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils
{
    public class UpdateJSON
    {

        public static async Task<string> WriteToConfig(string buildPath, string VersionID, string buildID, string FileName)
        {
            try
            {
                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
                Directory.CreateDirectory(DataFolder);
                string FilePath = Path.Combine(DataFolder, FileName);

                List<BuildConfig> jsonArray;
                if (File.Exists(FilePath))
                {

                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    jsonArray = JsonConvert.DeserializeObject<List<BuildConfig>>(await File.ReadAllTextAsync(FilePath))!;
                }
                else
                {
                    jsonArray = new List<BuildConfig>();
                    File.Create(FilePath).Dispose();
                    Loggers.Log("Created File Path -> " + FilePath);
                }

                BuildConfig existingEntry = jsonArray.FirstOrDefault(item => item.buildID == buildID)!;

                if (existingEntry != null)
                {
                    Loggers.Log("BUILD ID IS" + existingEntry.buildID);
                    Loggers.Log("BUILD NAME IS" + existingEntry.VersionID);

                    existingEntry.buildPath = buildPath;
                    existingEntry.played = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    return "already~build";
                }
                else
                {
                    Loggers.Log("BUILD ID IS" + buildID);
                    BuildConfig buildConfig = new BuildConfig()
                    {
                        VersionID = VersionID,
                        buildPath = buildPath,
                        buildID = buildID,
                        played = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    };

                    jsonArray.Add(buildConfig);
                }

                await File.WriteAllTextAsync(FilePath, JsonConvert.SerializeObject(jsonArray));

                return "added";
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message + "~UpdateJson");
                MessageBox.Show("Please Check Launcher Logs", "FortLauncher");
            }
            return "unknown";
        }

        public async static Task<string> ReadValue(string buildID, string SEARCh, string FileName)
        {
            try
            {
                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
                string FilePath = Path.Combine(DataFolder, FileName);
                Directory.CreateDirectory(DataFolder);
                List<BuildConfig> jsonArray;

                if (File.Exists(FilePath))
                {
                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    jsonArray = jsonArray = JsonConvert.DeserializeObject<List<BuildConfig>>(jsonData);

                    BuildConfig existingEntry = jsonArray.FirstOrDefault(item => item.buildID == buildID);

                    if (existingEntry != null)
                    {
                        PropertyInfo property = existingEntry.GetType().GetProperty(SEARCh);
                        if (property != null)
                        {
                            return property.GetValue(existingEntry)?.ToString() ?? "NULL";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Loggers.Log($"{ex.Message} ~ strange");
                MessageBox.Show("Please Check Launcher Logs", "FortLauncher");
            }
            return "NONE";
        }
    }
}
