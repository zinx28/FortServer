using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Classes
{
    public class BuildConfig
    {
        // public string buildName { get; set; } = string.Empty;
        public string VersionID { get; set; } = string.Empty;
        public string buildID { get; set; } = string.Empty;
        public string buildPath { get; set; } = string.Empty;
        public string played { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}
