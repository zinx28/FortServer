﻿namespace FortLibrary.EpicResponses.Friends
{
    public class PartiesC
    {
        public Dictionary<string, object> config { get; set; } = new Dictionary<string, object>();
        public JoinInfo join_info { get; set; } = new JoinInfo();
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
    }

    public class ConfigObject
    {
        public string discoverability { get; set; } = string.Empty;
        public bool join_confirmation { get; set; } = true;
        public string joinability { get; set; } = string.Empty;
        public int max_size { get; set; } = 0;
    }

    public class JoinInfo 
    {
        public JoinInfoConnection connection { get; set; } = new JoinInfoConnection();
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
    }

    public class JoinInfoConnection
    {
        public string id { get; set; } = string.Empty;
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public bool yield_leadership { get; set; } = false;
    }
}
