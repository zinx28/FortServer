using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Utils
{
    public class FortniteDetect
    {
        public static string Init(string BuildString)
        {
            if (BuildString.Contains("3724489"))
            {
                BuildString = "1.8";
            }
            else if (BuildString.Contains("3807424"))
            {
                BuildString = "1.11";
            }
            else if (BuildString.Contains("3870737"))
            {
                BuildString = "2.4.2";
            }
            else if (BuildString.Contains("3741772"))
            {
                BuildString = "1.8.2";
            }
            else if (BuildString.Contains("3240987"))
            {
                BuildString = "Alpha";
            }
            else
            {
                if (BuildString.Contains("-"))
                {
                    var BuildAdding = BuildString.Split("-");
                    if (BuildAdding.Length >= 1)
                    {
                        BuildString = BuildAdding[1];
                    }
                    else
                    {
                        BuildString = "Unknown";
                    }
                }
                else
                {
                    BuildString = "Unknown?";
                }
            }
            return BuildString;
        }
    }
}
