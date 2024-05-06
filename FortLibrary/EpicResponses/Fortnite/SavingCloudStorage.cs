using FortLibrary.ConfigHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Fortnite
{
    public class SavingCloudStorage
    {
        public string Title { get; set; } = string.Empty;
        public SavingCloudStorageObject Body { get; set; } = new SavingCloudStorageObject();
    }

    public class SavingCloudStorageObject
    {
        public string Name { get; set; } = string.Empty;
        public List<IniConfigValues> CachedData { get; set; } = new List<IniConfigValues>();
    }
}
