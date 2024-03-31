namespace FortBackend.src.App.Utilities.Classes.ConfigHelpers
{
    public class RegionManagerConfig
    {
        public List<Region_Definitions> RegionDefinitions { get; set; } = new List<Region_Definitions>();
        public List<Datacenter_Definitions> DatacenterDefinitions { get; set; } = new List<Datacenter_Definitions>();
    }

    public class Region_Definitions
    {
        public string Region { get; set; } = string.Empty;
        public string RegionId { get; set; } = string.Empty;
        public bool bEnabled { get; set; }
        public bool bVisible { get; set; }
        public bool bAutoAssignable { get; set; }
    }

    public class Datacenter_Definitions
    {
        public string Id { get; set; } = string.Empty;
        public string RegionId { get; set; } = string.Empty;
        public bool bEnabled { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
