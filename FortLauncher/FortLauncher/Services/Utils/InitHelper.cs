using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Utils
{
    public class IniHelper
    {
        public static void WriteToConfig(string SectionName, string PathKey, string NewValue)
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
            Directory.CreateDirectory(DataFolder);
            string FilePath = Path.Combine(DataFolder, "Settings.ini");


            FileIniDataParser parser = new FileIniDataParser();

            IniData iniData;
            if (File.Exists(FilePath))
            {
                iniData = parser.ReadFile(FilePath);
            }
            else
            {
                iniData = new IniData();
            }

            iniData[SectionName][PathKey] = NewValue;
            parser.WriteFile(FilePath, iniData, null);
        }

        public static string ReadValue(string SectionName, string PathKey)
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
            Directory.CreateDirectory(DataFolder);
            string FilePath = Path.Combine(DataFolder, "Settings.ini");

            FileIniDataParser parser = new FileIniDataParser();

            if (File.Exists(FilePath))
            {
                IniData iniData = parser.ReadFile(FilePath);

                return iniData[SectionName][PathKey];
            }
            else
            {
                return "NONE";
            }
        }

        public static void DeleteConfig()
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
            string FilePath = Path.Combine(DataFolder, "Settings.ini");

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
