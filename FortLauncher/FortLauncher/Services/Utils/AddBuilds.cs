using FortLauncher.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils
{
    public class AddBuilds
    {
        public static async Task<bool> AddBuild(string SearchPath, string FoundPath)
        {
            string result = await VersionSearcher.GetBuildVersion(FoundPath);

            if (result == "ERROR")
            {
                MessageBox.Show("ERROR");
                return false;
            }

            Loggers.Log("Adding Build with this config...");
            Loggers.Log(result);
            Loggers.Log(Convert.ToBase64String(Encoding.UTF8.GetBytes(result)));

            string CheckResponse = await UpdateJSON.WriteToConfig(SearchPath, result, Convert.ToBase64String(Encoding.UTF8.GetBytes(result)), "builds.json");
            if (CheckResponse == "already~build")
            {
                return false;
            }

            return true;
        }
    }
}
